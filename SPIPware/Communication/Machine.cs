﻿using System;
using System.Collections;
using System.Threading.Tasks;
using SPIPware.Util;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Text.RegularExpressions;
using SPIPware.GCode;
using SPIPware.GCode.GCodeCommands;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using log4net;
using SPIPware.Communication.Experiment_Parts; //for experiment parts may to another file set later

namespace SPIPware.Communication
{
    enum ConnectionType
    {
        Serial
    }

    public sealed class Machine
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Machine instance = new Machine();
        static Machine()
        {

        }
        public static Machine Instance
        {
            get
            {
                return instance;
            }
        }
        //Operating Mode for the machine
        public enum OperatingMode
        {
            Manual,
            SendFile,
            Probe,
            Disconnected,
            SendMacro
        }

        #region Properties
        public event Action<Vector3, bool> ProbeFinished;
        public event Action<string> NonFatalException;
        public event Action<string> Info;
        public event Action<string> LineReceived;
        public event Action<string> StatusReceived;
        public event Action<string> LineSent;
        public event Action ConnectionStateChanged;
        public event Action PositionUpdateReceived;
        public event Action StatusChanged;
        public event Action DistanceModeChanged;
        public event Action UnitChanged;
        public event Action PlaneChanged;
        public event Action BufferStateChanged;
        public event Action PinStateChanged;
        public event Action OperatingModeChanged;
        public event Action FileChanged;
        public event Action FilePositionChanged;
        public event Action OverrideChanged;
     //   public event Action NumRowsChanged; //event for number of plate rows changing
     //   public event Action NumColumnsChanged;

        public Vector3 MachinePosition { get; private set; } = new Vector3();   //No events here, the parser triggers a single event for both
        public Vector3 WorkOffset { get; private set; } = new Vector3();
        public Vector3 WorkPosition { get { return MachinePosition - WorkOffset; } } //Can change work position from w = machineP - offset to w = machineP +offset to put things into positive space

        public Vector3 LastProbePosMachine { get; private set; }
        public Vector3 LastProbePosWork { get; private set; }

        public int FeedOverride { get; private set; } = 100;
        public int RapidOverride { get; private set; } = 100;
        public int SpindleOverride { get; private set; } = 100;

        public bool PinStateProbe { get; private set; } = false;
        public bool PinStateLimitX { get; private set; } = false;
        public bool PinStateLimitY { get; private set; } = false;
        public bool PinStateLimitZ { get; private set; } = false;

        public double FeedRateRealtime { get; private set; } = 0;
        public double SpindleSpeedRealtime { get; private set; } = 0;

        public double CurrentTLO { get; private set; } = 0;
        #endregion

        #region BugbearUpdates (Tray Array)
        private Tray[] trays;//hard coded number of trays for later
        public Tray[] Trays
        {
            get { return trays; }
            set
            {
                trays = new Tray[3];
            }
        }
        
        /* may or may not keep, we will see
        public Tray[] Trays
        {
            get { return trays;}
            set
            {
                for(int i = 0; i < 3; i++)
                {
                    trays[i] = new Tray();
                }
            }
        }*/
       
        #endregion 

        private Calculator _calculator;
		private Calculator Calculator { get { return _calculator; } }

		private ReadOnlyCollection<bool> _pauselines = new ReadOnlyCollection<bool>(new bool[0]);
		public ReadOnlyCollection<bool> PauseLines
		{
			get { return _pauselines; }
			private set { _pauselines = value; }
		}

		private ReadOnlyCollection<string> _file = new ReadOnlyCollection<string>(new string[0]);
		public ReadOnlyCollection<string> File
		{
			get { return _file; }
			private set
			{
				_file = value;
				FilePosition = 0;

				RaiseEvent(FileChanged);
			}
		}

		private int _filePosition = 0;
		public int FilePosition
		{
			get { return _filePosition; }
			private set
			{
				_filePosition = value;
			}
		}

		private OperatingMode _mode = OperatingMode.Disconnected;
		public OperatingMode Mode
		{
			get { return _mode; }
			private set
			{
				if (_mode == value)
					return;

				_mode = value;
				RaiseEvent(OperatingModeChanged);
			}
		}

		#region Status and other settings
		private string _status = "Disconnected";
		public string Status
		{
			get { return _status; }
			private set
			{
				if (_status == value)
					return;
				_status = value;

				RaiseEvent(StatusChanged);
			}
		}

		private ParseDistanceMode _distanceMode = ParseDistanceMode.Absolute;
		public ParseDistanceMode DistanceMode
		{
			get { return _distanceMode; }
			private set
			{
				if (_distanceMode == value)
					return;
				_distanceMode = value;

				RaiseEvent(DistanceModeChanged);
			}
		}

		private ParseUnit _unit = ParseUnit.Metric;
		public ParseUnit Unit
		{
			get { return _unit; }
			private set
			{
				if (_unit == value)
					return;
				_unit = value;

				RaiseEvent(UnitChanged);
			}
		}

		private ArcPlane _plane = ArcPlane.XY;
		public ArcPlane Plane
		{
			get { return _plane; }
			private set
			{
				if (_plane == value)
					return;
				_plane = value;

				RaiseEvent(PlaneChanged);
			}
		}

		private bool _connected = false;
		public bool Connected
		{
			get { return _connected; }
			private set
			{
				if (value == _connected)
					return;

				_connected = value;

				if (!Connected)
					Mode = OperatingMode.Disconnected;

				RaiseEvent(ConnectionStateChanged);
			}
		}

		private int _bufferState;
		public int BufferState
		{
			get { return _bufferState; }
			private set
			{
				if (_bufferState == value)
					return;

				_bufferState = value;

				RaiseEvent(BufferStateChanged);
			}
		}
        #endregion Status

        #region Misc threads and Buffer
        public bool SyncBuffer { get; set; }

		private Stream Connection;
		private Thread WorkerThread;

		private StreamWriter Log;

		private void RecordLog(string message)
		{
			if (Log != null)
			{
				try
				{
					Log.WriteLine(message);
                    _log.Debug(message);
				}
				catch { throw; }
			}
		}
		private Machine()
		{
			_calculator = new Calculator(this);
		}

		Queue Sent = Queue.Synchronized(new Queue());
		Queue ToSend = Queue.Synchronized(new Queue());
		Queue ToSendPriority = Queue.Synchronized(new Queue()); //contains characters (for soft reset, feed hold etc)
		Queue ToSendMacro = Queue.Synchronized(new Queue());
        #endregion

        #region Work() Function
        private void Work()
		{
			try
			{
				StreamReader reader = new StreamReader(Connection);
				StreamWriter writer = new StreamWriter(Connection);

				int StatusPollInterval = Properties.Settings.Default.StatusPollInterval;

				int ControllerBufferSize = Properties.Settings.Default.ControllerBufferSize;
				BufferState = 0;

				TimeSpan WaitTime = TimeSpan.FromMilliseconds(0.5);
				DateTime LastStatusPoll = DateTime.Now + TimeSpan.FromSeconds(0.5);
				DateTime StartTime = DateTime.Now;

				DateTime LastFilePosUpdate = DateTime.Now;
				bool filePosChanged = false;

				bool SendMacroStatusReceived = false;

				writer.Write("\n$G\n");
				writer.Write("\n$#\n");
				writer.Flush();

				while (true)
				{
					Task<string> lineTask = reader.ReadLineAsync();

					while (!lineTask.IsCompleted)
					{
						if (!Connected)
						{
							return;
						}

						while (ToSendPriority.Count > 0)
						{
							writer.Write((char)ToSendPriority.Dequeue());
							writer.Flush();
						}
						if (Mode == OperatingMode.SendFile)
						{
							if (File.Count > FilePosition && (File[FilePosition].Length + 1) < (ControllerBufferSize - BufferState))
							{
								string send_line = File[FilePosition].Replace(" ", "");	// don't send whitespace to machine

								writer.Write(send_line);
								writer.Write('\n');
								writer.Flush();

								RecordLog("> " + send_line);

								RaiseEvent(UpdateStatus, send_line);
								RaiseEvent(LineSent, send_line);

								BufferState += send_line.Length + 1;

								Sent.Enqueue(send_line);

								if (PauseLines[FilePosition] && Properties.Settings.Default.PauseFileOnHold)
								{
									Mode = OperatingMode.Manual;
								}

								if (++FilePosition >= File.Count)
								{
									Mode = OperatingMode.Manual;
								}

								filePosChanged = true;

								continue;
							}
						}
						else if (Mode == OperatingMode.SendMacro)
						{
							switch(Status)
							{
								case "Idle":
									if (BufferState == 0 && SendMacroStatusReceived)
									{
										SendMacroStatusReceived = false;

										string send_line = (string)ToSendMacro.Dequeue();

										send_line = Calculator.Evaluate(send_line, out bool success);
										_log.Debug(DateTime.Now.ToLongTimeString());

										if (!success)
										{
											ReportError("Error while evaluating macro!");
											ReportError(send_line);

											ToSendMacro.Clear();
										}
										else
										{
											send_line = send_line.Replace(" ", "");

											writer.Write(send_line);
											writer.Write('\n');
											writer.Flush();

											RecordLog("> " + send_line);

											RaiseEvent(UpdateStatus, send_line);
											RaiseEvent(LineSent, send_line);

											BufferState += send_line.Length + 1;

											Sent.Enqueue(send_line);
										}
									}
									break;
								case "Run":
								case "Hold":
									break;
								default:	// grbl is in some kind of alarm state
									ToSendMacro.Clear();
									break;
							}

							if (ToSendMacro.Count == 0)
								Mode = OperatingMode.Manual;
						}
						else if (ToSend.Count > 0 && (((string)ToSend.Peek()).Length + 1) < (ControllerBufferSize - BufferState))
						{
							string send_line = ((string)ToSend.Dequeue()).Replace(" ", "");

							writer.Write(send_line);
							writer.Write('\n');
							writer.Flush();

							RecordLog("> " + send_line);

							RaiseEvent(UpdateStatus, send_line);
							RaiseEvent(LineSent, send_line);

							BufferState += send_line.Length + 1;

							Sent.Enqueue(send_line);
						}


						DateTime Now = DateTime.Now;

						if ((Now - LastStatusPoll).TotalMilliseconds > StatusPollInterval)
						{
							writer.Write('?');
							writer.Flush();
							LastStatusPoll = Now;
						}

						//only update file pos every X ms
						if (filePosChanged && (Now - LastFilePosUpdate).TotalMilliseconds > 500)
						{
							RaiseEvent(FilePositionChanged);
							LastFilePosUpdate = Now;
							filePosChanged = false;
						}

						Thread.Sleep(WaitTime);
					}

					string line = lineTask.Result;

					RecordLog("< " + line);

					if (line == "ok")
					{
						if (Sent.Count != 0)
						{
							BufferState -= ((string)Sent.Dequeue()).Length + 1;
						}
						else
						{
							_log.Info("Received OK without anything in the Sent Buffer");
							BufferState = 0;
						}
					}
					else
					{
						if (line.StartsWith("error:"))
						{
							if (Sent.Count != 0)
							{
								string errorline = (string)Sent.Dequeue();

								RaiseEvent(ReportError, $"{line}: {errorline}");
								BufferState -= errorline.Length + 1;
							}
							else
							{
								if ((DateTime.Now - StartTime).TotalMilliseconds > 200)
									RaiseEvent(ReportError, $"Received <{line}> without anything in the Sent Buffer");

								BufferState = 0;
							}

							Mode = OperatingMode.Manual;
						}
						else if (line.StartsWith("<"))
						{
							RaiseEvent(ParseStatus, line);
							SendMacroStatusReceived = true;
						}
						else if (line.StartsWith("[PRB:"))
						{
							RaiseEvent(ParseProbe, line);
							RaiseEvent(LineReceived, line);
						}
						else if (line.StartsWith("["))
						{
							RaiseEvent(UpdateStatus, line);
							RaiseEvent(LineReceived, line);
						}
						else if (line.StartsWith("ALARM"))
						{
							RaiseEvent(ReportError, line);
							Mode = OperatingMode.Manual;
						}
						else if (line.StartsWith("grbl"))
						{
							RaiseEvent(LineReceived, line);
							RaiseEvent(ParseStartup, line);
						}
						else if (line.Length > 0)
							RaiseEvent(LineReceived, line);
					}
				}
			}
			catch (Exception ex)
			{
				RaiseEvent(ReportError, $"Fatal Error in Work Loop: {ex.Message}");
				Disconnect();
			}
		}
        #endregion

        #region Connect() and Disconnection() Functions
        public void Connect()
		{
			if (Connected)
				throw new Exception("Can't Connect: Already Connected");

			switch (Properties.Settings.Default.ConnectionType)
			{
				case ConnectionType.Serial:
					SerialPort port = new SerialPort(Properties.Settings.Default.P, Properties.Settings.Default.SerialPortBaud);
					port.Open();
					Connection = port.BaseStream;
					break;
				default:
					throw new Exception("Invalid Connection Type");
			}

			if (Properties.Settings.Default.LogTraffic)
			{
				try
				{
					Log = new StreamWriter(Constants.LogFile);
				}
				catch (Exception e)
				{
					NonFatalException("could not open logfile: " + e.Message);
				}
			}

			Connected = true;

			ToSend.Clear();
			ToSendPriority.Clear();
			Sent.Clear();
			ToSendMacro.Clear();

			Mode = OperatingMode.Manual;

			if (PositionUpdateReceived != null)
				PositionUpdateReceived.Invoke();

			WorkerThread = new Thread(Work);
			WorkerThread.Priority = ThreadPriority.AboveNormal;
			WorkerThread.Start();
		}

		public void Disconnect()
		{
			if (!Connected)
				throw new Exception("Can't Disconnect: Not Connected");

			if (Log != null)
				Log.Close();
			Log = null;

			Connected = false;

			WorkerThread.Join();

			Connection.Close();
			Connection.Dispose();
			Connection = null;

			Mode = OperatingMode.Disconnected;

			MachinePosition = new Vector3();
			WorkOffset = new Vector3();
			FeedRateRealtime = 0;
			CurrentTLO = 0;

			if (PositionUpdateReceived != null)
				PositionUpdateReceived.Invoke();

			Status = "Disconnected";
			DistanceMode = ParseDistanceMode.Absolute;
			Unit = ParseUnit.Metric;
			Plane = ArcPlane.XY;
			BufferState = 0;

			FeedOverride = 100;
			RapidOverride = 100;
			SpindleOverride = 100;

			if (OverrideChanged != null)
				OverrideChanged.Invoke();

			PinStateLimitX = false;
			PinStateLimitY = false;
			PinStateLimitZ = false;
			PinStateProbe = false;

			if (PinStateChanged != null)
				PinStateChanged.Invoke();

			ToSend.Clear();
			ToSendPriority.Clear();
			Sent.Clear();
			ToSendMacro.Clear();
		}
        #endregion

        #region Position and Movement Command Functions

        //this struct is a data type that holds three doubles as a XYZ point location
        public struct machinePos
        { //have to put public in the members of the struct because default by private with C#
            public double currentLocationX;
            public double currentLocationY;
            public double currentLocationZ;
        };
        public machinePos homeMachinePos;

        private String buildCommandX(double distance) //where command is built for gcode
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append("G90");
            sb.Append(Properties.Settings.Default.PrimaryAxis); //primary axis is labeled as x so could be overload to be given y
            sb.Append(distance + " "); 
            sb.Append("F" + Properties.Settings.Default.Speed);
            _log.Info("Generated Command: "+ sb.ToString());
            return sb.ToString();
        }

        //The below functions emulates the similar X axis, perhaps by seperating the two axis, the commands can be seperated by axes
       private String buildCommandY(double distance) //where command is built for gcode
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("G90");
            //Landon added code
            sb.Append(Properties.Settings.Default.SecondaryAxis); //this is the same as appending "Y" to the string
            sb.Append(distance + " "); //should move as same distance FOR NOW
            //end of landon added code
            sb.Append("F" + Properties.Settings.Default.Speed);
            _log.Info("Generated Command: " + sb.ToString());
            return sb.ToString();
        }


        public double sendMotionCommandX(int position, double offset)
        {
            double distance = homeMachinePos.currentLocationX + Properties.Settings.Default.WellXOffset + (Properties.Settings.Default.XBetweenDistance * position) + offset;
            _log.Debug("Calculated X Distance: " + distance);
            SendLine(buildCommandX(distance));
            return distance;

        }

        public double sendMotionCommandX(int position)
        {
           return sendMotionCommandX(position, 0);
        }

        //y-axis version
        public double sendMotionCommandY(int position, double offset) // could be important method to send y coordinates to sunbear
        {
            double distance = homeMachinePos.currentLocationY + Properties.Settings.Default.WellYOffset + (Properties.Settings.Default.YBetweenDistance * position) + offset;
            _log.Debug("Calculated Y Distance: " + distance);
            //here a for loop could be add for each line of code per row then iterate to next y coordinate
            SendLine(buildCommandY(distance));
            return distance;

        }

        public double sendMotionCommandY(int position)
        {
            return sendMotionCommandY(position, 0);
        }

        //z-axis command
        private String buildCommandZ(double distance) //where command is built for gcode
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("G90");
            sb.Append(Properties.Settings.Default.TertiaryAxis); //tertiary axis is z
            sb.Append(distance + " ");
            sb.Append("F" + Properties.Settings.Default.Speed);
            _log.Info("Generated Command: " + sb.ToString());
            return sb.ToString();
        }

        public double sendMotionCommandZ(int position, double offset)
        {
            double distance = homeMachinePos.currentLocationZ + Properties.Settings.Default.PlateXOffset + (Properties.Settings.Default.XBetweenDistance * position) + offset;
            _log.Debug("Calculated Distance: " + distance);
            SendLine(buildCommandZ(distance));
            return distance;
        }

        public double sendMotionCommandZ(int position)
        {
            return sendMotionCommandZ(position, 0);
        }
        #endregion

        #region GRBL related functions
        public void SendLine(string line)
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			if (Mode != OperatingMode.Manual && Mode != OperatingMode.Probe)
			{
				RaiseEvent(Info, "Not in Manual Mode");
				return;
			}
            _log.Debug("Sending Line: " + line);
			ToSend.Enqueue(line);
		}

		public void SoftReset()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			Mode = OperatingMode.Manual;

			ToSend.Clear();
			ToSendPriority.Clear();
			Sent.Clear();
			ToSendMacro.Clear();
			ToSendPriority.Enqueue((char)0x18);

			BufferState = 0;

			FeedOverride = 100;
			RapidOverride = 100;
			SpindleOverride = 100;

			if (OverrideChanged != null)
				OverrideChanged.Invoke();

			SendLine("$G");
			SendLine("$#");
		}

		public void SendMacroLines(params string[] lines)
		{
			if(Mode != OperatingMode.Manual)
			{
				RaiseEvent(Info, "Not in Manual Mode");
				return;
			}

			foreach (string line in lines)
				ToSendMacro.Enqueue(line.Trim());

			Mode = OperatingMode.SendMacro;
		}

		//probably shouldn't expose this, but adding overrides would be much more effort otherwise
		public void SendControl(byte controlchar)
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			ToSendPriority.Enqueue((char)controlchar);
		}

		public void FeedHold()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			ToSendPriority.Enqueue('!');
		}

		public void CycleStart()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			ToSendPriority.Enqueue('~');
		}

		public void JogCancel()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			ToSendPriority.Enqueue((char)0x85);
		}

		public void SetFile(IList<string> file)
		{
			if (Mode == OperatingMode.SendFile)
			{
				RaiseEvent(Info, "Can't change file while active");
				return;
			}

			bool[] pauselines = new bool[file.Count];

			for (int line = 0; line < file.Count; line++)
			{
				var matches = GCodeParser.GCodeSplitter.Matches(file[line]);

				foreach (Match m in matches)
				{
					if (m.Groups[1].Value == "M")
					{
						int code = int.MinValue;

						if (int.TryParse(m.Groups[2].Value, out code))
						{
							if (code == 0 || code == 1 || code == 2 || code == 30 || code == 6)
								pauselines[line] = true;
						}
					}
				}
			}

			File = new ReadOnlyCollection<string>(file);
			PauseLines = new ReadOnlyCollection<bool>(pauselines);

			FilePosition = 0;

			RaiseEvent(FilePositionChanged);
		}
        #endregion

        #region File and Probe Functions
        public void ClearFile()
		{
			if (Mode == OperatingMode.SendFile)
			{
				RaiseEvent(Info, "Can't change file while active");
				return;
			}

			File = new ReadOnlyCollection<string>(new string[0]);
			FilePosition = 0;
			RaiseEvent(FilePositionChanged);
		}

		public void FileStart()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			if (Mode != OperatingMode.Manual)
			{
				RaiseEvent(Info, "Not in Manual Mode");
				return;
			}

			Mode = OperatingMode.SendFile;
		}

		public void FilePause()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			if (Mode != OperatingMode.SendFile)
			{
				RaiseEvent(Info, "Not in SendFile Mode");
				return;
			}

			Mode = OperatingMode.Manual;
		}

		public void ProbeStart()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			if (Mode != OperatingMode.Manual)
			{
				RaiseEvent(Info, "Can't start probing while running!");
				return;
			}

			Mode = OperatingMode.Probe;
		}

		public void ProbeStop()
		{
			if (!Connected)
			{
				RaiseEvent(Info, "Not Connected");
				return;
			}

			if (Mode != OperatingMode.Probe)
			{
				RaiseEvent(Info, "Not in Probe mode");
				return;
			}

			Mode = OperatingMode.Manual;
		}

		public void FileGoto(int lineNumber)
		{
			if (Mode == OperatingMode.SendFile)
				return;

			if (lineNumber >= File.Count || lineNumber < 0)
			{
				RaiseEvent(NonFatalException, "Line Number outside of file length");
				return;
			}

			FilePosition = lineNumber;

			RaiseEvent(FilePositionChanged);
		}
        #endregion

        public void ClearQueue()
		{
			if (Mode != OperatingMode.Manual)
			{
				RaiseEvent(Info, "Not in Manual mode");
				return;
			}

			ToSend.Clear();
		}

        #region Update Status info functions
        private static Regex GCodeSplitter = new Regex(@"([GZ])\s*(\-?\d+\.?\d*)", RegexOptions.Compiled);

		/// <summary>
		/// Updates Status info from each line sent
		/// </summary>
		/// <param name="line"></param>
		private void UpdateStatus(string line)
		{
			if (!Connected)
				return;

			if (line.Contains("$J="))
				return;

			if (line.StartsWith("[TLO:"))
			{
				try
				{
					CurrentTLO = double.Parse(line.Substring(5, line.Length - 6), Constants.DecimalParseFormat);
					RaiseEvent(PositionUpdateReceived);
				}
				catch { RaiseEvent(NonFatalException, "Error while Parsing Status Message"); }
				return;
			}

			try
			{
				//we use a Regex here so G91.1 etc don't get recognized as G91
				MatchCollection mc = GCodeSplitter.Matches(line);
				for (int i = 0; i < mc.Count; i++)
				{
					Match m = mc[i];

					if (m.Groups[1].Value != "G")
						continue;

					double code = double.Parse(m.Groups[2].Value, Constants.DecimalParseFormat);

					if (code == 17)
						Plane = ArcPlane.XY;
					if (code == 18)
						Plane = ArcPlane.YZ;
					if (code == 19)
						Plane = ArcPlane.ZX;

					if (code == 20)
						Unit = ParseUnit.Imperial;
					if (code == 21)
						Unit = ParseUnit.Metric;

					if (code == 90)
						DistanceMode = ParseDistanceMode.Absolute;
					if (code == 91)
						DistanceMode = ParseDistanceMode.Incremental;

					if (code == 49)
						CurrentTLO = 0;

					if (code == 43.1)
					{
						if (mc.Count > (i + 1))
						{
							if (mc[i + 1].Groups[1].Value == "Z")
							{
								CurrentTLO = double.Parse(mc[i + 1].Groups[2].Value, Constants.DecimalParseFormat);
								RaiseEvent(PositionUpdateReceived);
							}

							i += 1;
						}
					}
				}
			}
			catch { RaiseEvent(NonFatalException, "Error while Parsing Status Message"); }
		}

		private static Regex StatusEx = new Regex(@"(?<=[<|])(\w+):?([^|>]*)?(?=[|>])", RegexOptions.Compiled);
		/// <summary>
		/// Parses a recevied status report (answer to '?')
		/// </summary>
		private void ParseStatus(string line)
		{
			MatchCollection statusMatch = StatusEx.Matches(line);

			if (statusMatch.Count == 0)
			{
				NonFatalException.Invoke(string.Format("Received Bad Status: '{0}'", line));
				return;
			}

			bool posUpdate = false;
			bool overrideUpdate = false;
			bool pinStateUpdate = false;
			bool resetPins = true;

			foreach (Match m in statusMatch)
			{
				if (m.Index == 1)
				{
					Status = m.Groups[1].Value;
					continue;
				}

				if (m.Groups[1].Value == "Ov")
				{
					try
					{
						string[] parts = m.Groups[2].Value.Split(',');
						FeedOverride = int.Parse(parts[0]);
						RapidOverride = int.Parse(parts[1]);
						SpindleOverride = int.Parse(parts[2]);
						overrideUpdate = true;
					}
					catch { NonFatalException.Invoke(string.Format("Received Bad Status: '{0}'", line)); }
				}

				else if (m.Groups[1].Value == "WCO")
				{
					try
					{
						WorkOffset = Vector3.Parse(m.Groups[2].Value);
						posUpdate = true;
					}
					catch { NonFatalException.Invoke(string.Format("Received Bad Status: '{0}'", line)); }
				}

				else if (SyncBuffer && m.Groups[1].Value == "Bf")
				{
					try
					{
						int availableBytes = int.Parse(m.Groups[2].Value.Split(',')[1]);
						int used = Properties.Settings.Default.ControllerBufferSize - availableBytes;

						if (used < 0)
							used = 0;

						BufferState = used;
						RaiseEvent(Info, $"Buffer State Synced ({availableBytes} bytes free)");
					}
					catch { NonFatalException.Invoke(string.Format("Received Bad Status: '{0}'", line)); }
				}

				else if (m.Groups[1].Value == "Pn")
				{
					resetPins = false;

					string states = m.Groups[2].Value;

					bool stateX = states.Contains("X");
					if (stateX != PinStateLimitX)
						pinStateUpdate = true;
					PinStateLimitX = stateX;

					bool stateY = states.Contains("Y");
					if (stateY != PinStateLimitY)
						pinStateUpdate = true;
					PinStateLimitY = stateY;

					bool stateZ = states.Contains("Z");
					if (stateZ != PinStateLimitZ)
						pinStateUpdate = true;
					PinStateLimitZ = stateZ;

					bool stateP = states.Contains("P");
					if (stateP != PinStateProbe)
						pinStateUpdate = true;
					PinStateProbe = stateP;
				}

				else if (m.Groups[1].Value == "F")
				{
					try
					{
						FeedRateRealtime = double.Parse(m.Groups[2].Value, Constants.DecimalParseFormat);
						posUpdate = true;
					}
					catch { NonFatalException.Invoke(string.Format("Received Bad Status: '{0}'", line)); }
				}

				else if (m.Groups[1].Value == "FS")
				{
					try
					{
						string[] parts = m.Groups[2].Value.Split(',');
						FeedRateRealtime = double.Parse(parts[0], Constants.DecimalParseFormat);
						SpindleSpeedRealtime = double.Parse(parts[1], Constants.DecimalParseFormat);
						posUpdate = true;
					}
					catch { NonFatalException.Invoke(string.Format("Received Bad Status: '{0}'", line)); }
				}
			}

			SyncBuffer = false; //only run this immediately after button press

			//run this later to catch work offset changes before parsing position
			Vector3 NewMachinePosition = MachinePosition;

			foreach (Match m in statusMatch)
			{
				if (m.Groups[1].Value == "MPos" || m.Groups[1].Value == "WPos")
				{
					try
					{
						NewMachinePosition = Vector3.Parse(m.Groups[2].Value);

						if (m.Groups[1].Value == "WPos")
							NewMachinePosition += WorkOffset;

						if (NewMachinePosition != MachinePosition)
						{
							posUpdate = true;
							MachinePosition = NewMachinePosition;
						}
					}
					catch { NonFatalException.Invoke(string.Format("Received Bad Status: '{0}'", line)); }
				}

			}

			if (posUpdate && Connected && PositionUpdateReceived != null)
				PositionUpdateReceived.Invoke();

			if (overrideUpdate && Connected && OverrideChanged != null)
				OverrideChanged.Invoke();

			if (resetPins)  //no pin state received in status -> all zero
			{
				pinStateUpdate = PinStateLimitX | PinStateLimitY | PinStateLimitZ | PinStateProbe;  //was any pin set before

				PinStateLimitX = false;
				PinStateLimitY = false;
				PinStateLimitZ = false;
				PinStateProbe = false;
			}

			if (pinStateUpdate && Connected && PinStateChanged != null)
				PinStateChanged.Invoke();

			if (Connected && StatusReceived != null)
				StatusReceived.Invoke(line);
		}

		private static Regex ProbeEx = new Regex(@"\[PRB:(?'Pos'[-0-9\.]*,[-0-9\.]*,[-0-9\.]*):(?'Success'0|1)\]", RegexOptions.Compiled);

		/// <summary>
		/// Parses a recevied probe report
		/// </summary>
		private void ParseProbe(string line)
		{
			if (ProbeFinished == null)
				return;

			Match probeMatch = ProbeEx.Match(line);

			Group pos = probeMatch.Groups["Pos"];
			Group success = probeMatch.Groups["Success"];

			if (!probeMatch.Success || !(pos.Success & success.Success))
			{
				NonFatalException.Invoke($"Received Bad Probe: '{line}'");
				return;
			}

			Vector3 ProbePos = Vector3.Parse(pos.Value);
			LastProbePosMachine = ProbePos;

			ProbePos -= WorkOffset;

			LastProbePosWork = ProbePos;

			bool ProbeSuccess = success.Value == "1";

			ProbeFinished.Invoke(ProbePos, ProbeSuccess);
		}

		private static Regex StartupRegex = new Regex("grbl v([0-9])\\.([0-9])([a-z])");
		private void ParseStartup(string line)
		{
			Match m = StartupRegex.Match(line);

			int major, minor;
			char rev;

			if (!m.Success ||
				!int.TryParse(m.Groups[1].Value, out major) ||
				!int.TryParse(m.Groups[2].Value, out minor) ||
				!char.TryParse(m.Groups[3].Value, out rev))
			{
				RaiseEvent(Info, "Could not parse startup message.");
				return;
			}

			Version v = new Version(major, minor, (int)rev);
			if (v < Constants.MinimumGrblVersion)
			{
				ReportError("Outdated version of grbl detected!");
				ReportError($"Please upgrade to at least grbl v{Constants.MinimumGrblVersion.Major}.{Constants.MinimumGrblVersion.Minor}{(char)Constants.MinimumGrblVersion.Build}");
			}

		}
        #endregion

        /// <summary>
        /// Reports error. This is there to offload the ExpandError function from the "Real-Time" worker thread to the application thread
        /// also used for alarms
        /// </summary>
        private void ReportError(string error)
		{
            _log.Error(GrblCodeTranslator.ExpandError(error));
            if (NonFatalException != null)
            {  
                NonFatalException.Invoke(GrblCodeTranslator.ExpandError(error));
            }   
		}

		private void RaiseEvent(Action<string> action, string param)
		{
			if (action == null)
				return;

			Application.Current.Dispatcher.BeginInvoke(action, param);
		}

		private void RaiseEvent(Action action)
		{
			if (action == null)
				return;

			Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}
