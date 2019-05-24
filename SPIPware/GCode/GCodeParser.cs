using System;
using System.Collections.Generic;
using System.Linq;
using SPIPware.GCode.GCodeCommands;
using System.IO;
using System.Text.RegularExpressions;
using SPIPware.Util;
using System.Reflection;
using log4net;

/*                                      *
 *      SPIPware.GCode.GCodeParser      *
 *                                      *
 *                                      *
 *  GCodeParser class that helps parse  *
 *      the code for Gcode              */

namespace SPIPware.GCode
{
    /// <summary>
    /// ParseDistanceMode determines whether the a G90 or G91 code will be sent
    /// </summary>
    /// <remarks>Absolute (the 0 position) is G90 while Incremental(1) is G91</remarks>
	public enum ParseDistanceMode
	{
		Absolute,
		Incremental
	}
    /// <summary>
    /// ParseUnit determines whether to use the metric system or Freedom units
    /// </summary>
	public enum ParseUnit
	{
		Metric,
		Imperial
	}
    /// <summary>
    /// Parser State is a class to describe the current state of the parsing
    /// </summary>
	class ParserState
	{
		public Vector3 Position;
		public ArcPlane Plane;
		public double Feed;
		public ParseDistanceMode DistanceMode;
		public ParseDistanceMode ArcDistanceMode;
		public ParseUnit Unit;
		public int LastMotionMode;
        /// <summary>
        /// ParserState() is current state parsing
        /// </summary>
        /// <remarks>Includes position, the plane that is being used, feed rate, distance mode, arc distance mode,
        /// unit system, and last motion mode.</remarks>
		public ParserState()
		{
			Position = new Vector3();
			Plane = ArcPlane.XY;
			Feed = 0;
			DistanceMode = ParseDistanceMode.Absolute;
			ArcDistanceMode = ParseDistanceMode.Incremental;
			Unit = ParseUnit.Metric;
			LastMotionMode = -1;
		}
	}
    /// <summary>
    /// Word is a struct with a char command and a double for parameter. Resprents a Gcode Command>
    /// </summary>
	struct Word
	{
		public char Command;
		public double Parameter;
	}
    /// <summary>
    /// GCodeParser class parses the GCode for the machine to interpret and use
    /// </summary>
	static class GCodeParser
	{
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static ParserState State;

		public static Regex GCodeSplitter = new Regex(@"([A-Z])\s*(\-?\d+\.?\d*)", RegexOptions.Compiled);
		private static double[] MotionCommands = new double[] { 0, 1, 2, 3 };
		private static string ValidWords = "GMXYZIJKFRSP"; //all the valid letter commands of grbl
		public static List<Command> Commands;

		public static void Reset()
		{
			State = new ParserState();
			Commands = new List<Command>(); //don't reuse, might be used elsewhere
		}

		static GCodeParser()
		{
			Reset();
		}

		public static void ParseFile(string path)
		{
			Parse(File.ReadLines(path));
		}

		public static void Parse(IEnumerable<string> file)
		{
			int i = 0;

			var sw = System.Diagnostics.Stopwatch.StartNew();

			foreach (string linei in file)
			{
				i++;
				string line = CleanupLine(linei, i);

				if (string.IsNullOrWhiteSpace(line))
					continue;

				Parse(line.ToUpper(), i);
			}

			sw.Stop();

			_log.Debug(String.Format("Parsing the GCode File took {0} ms", sw.ElapsedMilliseconds));
		}

		static string CleanupLine(string line, int lineNumber)
		{
			int commentIndex = line.IndexOf(';');

			if (commentIndex > -1)
				line = line.Remove(commentIndex);

			int start = -1;

			while ((start = line.IndexOf('(')) != -1)
			{
				int end = line.IndexOf(')');

				if (end < start)
					throw new ParseException("mismatched parentheses", lineNumber);

				line = line.Remove(start, end - start);
			}

			return line;
		}
        /// <summary>
        /// Parse() is a method that determines the GCode command and the parses it to an execution
        /// </summary>
        /// <param name="line">The string that is inputed for the command to be deciphered</param>
        /// <param name="lineNumber">The line number for the file</param>
		static void Parse(string line, int lineNumber)
		{
			MatchCollection matches = GCodeSplitter.Matches(line);

			List<Word> Words = new List<Word>(matches.Count);

			foreach (Match match in matches)
			{
				Words.Add(new Word() { Command = match.Groups[1].Value[0], Parameter = double.Parse(match.Groups[2].Value, Constants.DecimalParseFormat) });
			}

			for (int i = 0; i < Words.Count; i++)
			{
                //error check for non grbl commands
				if (!ValidWords.Contains(Words[i].Command))
				{
					throw new ParseException($"unknown word (letter): \"{Words[i].Command} {Words[i].Parameter}\"", lineNumber);
				}
                //if feed
				if (Words[i].Command != 'F')
					continue;

				State.Feed = Words[i].Parameter;
				if (State.Unit == ParseUnit.Imperial)
					State.Feed *= 25.4;
				Words.RemoveAt(i--);
				continue;
			}
            
			for(int i = 0; i < Words.Count; i++)
			{
                //marco commands
				if (Words[i].Command == 'M')
				{
					int param = (int)Words[i].Parameter;

					if (param != Words[i].Parameter || param < 0)
						throw new ParseException("MCode can only have integer parameters", lineNumber);

					Commands.Add(new MCode() { Code = param });

					Words.RemoveAt(i);
					i--;
					continue;
				}
                //spindle speed commands
				if (Words[i].Command == 'S')
				{
					double param = Words[i].Parameter;

					if (param < 0)
						throw new ParseException("Spindle Speed must be positive", lineNumber);

					Commands.Add(new Spindle() { Speed = param });

					Words.RemoveAt(i);
					i--;
					continue;
				}
                //motion commands
				if (Words[i].Command == 'G' && !MotionCommands.Contains(Words[i].Parameter))
				{
					#region UnitPlaneDistanceMode

					double param = Words[i].Parameter;
                    //absolute motion
					if (param == 90)
					{
						State.DistanceMode = ParseDistanceMode.Absolute;
						Words.RemoveAt(i);
						i--;
						continue;
					}
                    //incremental motion
					if (param == 91)
					{
						State.DistanceMode = ParseDistanceMode.Incremental;
						Words.RemoveAt(i);
						i--;
						continue;
					}
					if (param == 90.1)
					{
						State.ArcDistanceMode = ParseDistanceMode.Absolute;
						Words.RemoveAt(i);
						continue;
					}
					if (param == 91.1)
					{
						State.ArcDistanceMode = ParseDistanceMode.Incremental;
						Words.RemoveAt(i);
						i--;
						continue;
					}
                    //changes unit system to metric
					if (param == 21)
					{
						State.Unit = ParseUnit.Metric;
						Words.RemoveAt(i);
						i--;
						continue;
					}
                    //changes unit sysetm to imperial
					if (param == 20)
					{
						State.Unit = ParseUnit.Imperial;
						Words.RemoveAt(i);
						i--;
						continue;
					}
                    //The next three change the working arc plane to XY, ZX, and YZ respectively
					if (param == 17)
					{
						State.Plane = ArcPlane.XY;
						Words.RemoveAt(i);
						i--;
						continue;
					}
					if (param == 18)
					{
						State.Plane = ArcPlane.ZX;
						Words.RemoveAt(i);
						i--;
						continue;
					}
					if (param == 19)
					{
						State.Plane = ArcPlane.YZ;
						Words.RemoveAt(i);
						i--;
						continue;
					}
					if (param == 4)
					{
						if (Words.Count >= 2 && Words[i + 1].Command == 'P')
						{
							if (Words[i + 1].Parameter < 0)
								throw new ParseException("Negative dwell time", lineNumber);

							Commands.Add(new Dwell() { Seconds = Words[i + 1].Parameter });
							Words.RemoveAt(i + 1);
							Words.RemoveAt(i);
							i--;
							continue;
						}
					}

					if (param == 54 || param == 94 || param == 40)
					{
						// discard Gxx words
						Words.RemoveAt(i);
						i--;
						continue;
					}

					throw new ParseException($"G{param} is not supported", lineNumber);
					#endregion
				}
			}

			if (Words.Count == 0)
				return;

			int MotionMode = State.LastMotionMode;

			if (Words.First().Command == 'G')
			{
				MotionMode = (int)Words.First().Parameter;
				State.LastMotionMode = MotionMode;
				Words.RemoveAt(0);
			}

			if (MotionMode < 0)
				throw new ParseException("No Motion Mode active", lineNumber);

			double UnitMultiplier = (State.Unit == ParseUnit.Metric) ? 1 : 25.4;

			Vector3 EndPos = State.Position; //ending position is made into the position of the state

            //finding end position of command
			#region FindEndPos 
			{
				int Incremental = (State.DistanceMode == ParseDistanceMode.Incremental) ? 1 : 0;

				for (int i = 0; i < Words.Count; i++)
				{
					if (Words[i].Command != 'X')
						continue;
					EndPos.X = Words[i].Parameter * UnitMultiplier + Incremental * EndPos.X;
					Words.RemoveAt(i);
					break;
				}

				for (int i = 0; i < Words.Count; i++)// <--This could be useful for y use
				{
					if (Words[i].Command != 'Y')
						continue;
					EndPos.Y = Words[i].Parameter * UnitMultiplier + Incremental * EndPos.Y;
					Words.RemoveAt(i);
					break;
				}

				for (int i = 0; i < Words.Count; i++)
				{
					if (Words[i].Command != 'Z')
						continue;
					EndPos.Z = Words[i].Parameter * UnitMultiplier + Incremental * EndPos.Z;
					Words.RemoveAt(i);
					break;
				}
			}
			#endregion

			if (MotionMode != 0 && State.Feed <= 0)
			{
				throw new ParseException("Feed Rate Undefined", lineNumber);
			}

			if (MotionMode <= 1)
			{
				if (Words.Count > 0)
					throw new ParseException("Motion Command must be last in line (unused Words in Block)", lineNumber);

				Line motion = new Line();
				motion.Start = State.Position;
				motion.End = EndPos;
				motion.Feed = State.Feed;
				motion.Rapid = MotionMode == 0;

				Commands.Add(motion);
				State.Position = EndPos;
				return;
			}

			double U, V;

			bool IJKused = false;

			switch (State.Plane)
			{
				default:
					U = State.Position.X;
					V = State.Position.Y;
					break;
				case ArcPlane.YZ:
					U = State.Position.Y;
					V = State.Position.Z;
					break;
				case ArcPlane.ZX:
					U = State.Position.Z;
					V = State.Position.X;
					break;
			}
            //IJK are the offsets on planes so below is finding them and uses them for different planes
			#region FindIJK
			{
				int ArcIncremental = (State.ArcDistanceMode == ParseDistanceMode.Incremental) ? 1 : 0;

				for (int i = 0; i < Words.Count; i++)
				{
					if (Words[i].Command != 'I')
						continue;

					switch (State.Plane)
					{
						case ArcPlane.XY:
							U = Words[i].Parameter * UnitMultiplier + ArcIncremental * State.Position.X;
							break;
						case ArcPlane.YZ:
							throw new ParseException("Current Plane is YZ, I word is invalid", lineNumber);
						case ArcPlane.ZX:
							V = Words[i].Parameter * UnitMultiplier + ArcIncremental * State.Position.X;
							break;
					}

					IJKused = true;
					Words.RemoveAt(i);
					break;
				}

				for (int i = 0; i < Words.Count; i++)
				{
					if (Words[i].Command != 'J')
						continue;

					switch (State.Plane)
					{
						case ArcPlane.XY:
							V = Words[i].Parameter * UnitMultiplier + ArcIncremental * State.Position.Y;
							break;
						case ArcPlane.YZ:
							U = Words[i].Parameter * UnitMultiplier + ArcIncremental * State.Position.Y;
							break;
						case ArcPlane.ZX:
							throw new ParseException("Current Plane is ZX, J word is invalid", lineNumber);
					}

					IJKused = true;
					Words.RemoveAt(i);
					break;
				}

				for (int i = 0; i < Words.Count; i++)
				{
					if (Words[i].Command != 'K')
						continue;

					switch (State.Plane)
					{
						case ArcPlane.XY:
							throw new ParseException("Current Plane is XY, K word is invalid", lineNumber);
						case ArcPlane.YZ:
							V = Words[i].Parameter * UnitMultiplier + ArcIncremental * State.Position.Z;
							break;
						case ArcPlane.ZX:
							U = Words[i].Parameter * UnitMultiplier + ArcIncremental * State.Position.Z;
							break;
					}

					IJKused = true;
					Words.RemoveAt(i);
					break;
				}
			}
			#endregion

			#region ResolveRadius
			for (int i = 0; i < Words.Count; i++)
			{
				if (Words[i].Command != 'R')
					continue;

				if (IJKused)
					throw new ParseException("Both IJK and R notation used", lineNumber);

				if (State.Position == EndPos)
					throw new ParseException("arcs in R-notation must have non-coincident start and end points", lineNumber);

				double Radius = Words[i].Parameter * UnitMultiplier;

				if (Radius == 0)
					throw new ParseException("Radius can't be zero", lineNumber);

				double A, B;

				switch (State.Plane)
				{
					default:
						A = EndPos.X;
						B = EndPos.Y;
						break;
					case ArcPlane.YZ:
						A = EndPos.Y;
						B = EndPos.Z;
						break;
					case ArcPlane.ZX:
						A = EndPos.Z;
						B = EndPos.X;
						break;
				}

				A -= U;     //(AB) = vector from start to end of arc along the axes of the current plane
				B -= V;

				/*
				double C = -B;  //(UV) = vector perpendicular to (AB)
				double D = A;

				{   //normalize perpendicular vector
					double perpLength = Math.Sqrt(C * C + D * D);
					C /= perpLength;
					D /= perpLength;
				}

				double PerpSquare = (Radius * Radius) - ((A * A + B * B) / 4);

				if (PerpSquare < 0)
					throw new ParseException("arc radius too small to reach both ends", lineNumber);

				double PerpLength = Math.Sqrt(PerpSquare);

				if (MotionMode == 3 ^ Radius < 0)
					PerpLength = -PerpLength;

				U += (A / 2) + C * (PerpLength);
				V += (B / 2) + (D * PerpLength);
				*/
				//see grbl/gcode.c
				double h_x2_div_d = 4.0 * (Radius * Radius) - (A * A + B * B);
				if (h_x2_div_d < 0)
				{
					throw new ParseException("arc radius too small to reach both ends", lineNumber);
				}

				h_x2_div_d = -Math.Sqrt(h_x2_div_d) / Math.Sqrt(A * A + B * B);

				if (MotionMode == 3 ^ Radius < 0)
				{
					h_x2_div_d = -h_x2_div_d;
				}

				U += 0.5 * (A - (B * h_x2_div_d));
				V += 0.5 * (B + (A * h_x2_div_d));

				Words.RemoveAt(i);
				break;
			}
			#endregion

			Arc arc = new Arc();
			arc.Start = State.Position;
			arc.End = EndPos;
			arc.Feed = State.Feed;
			arc.Direction = (MotionMode == 2) ? ArcDirection.CW : ArcDirection.CCW;
			arc.U = U;
			arc.V = V;
			arc.Plane = State.Plane;

			Commands.Add(arc);
			State.Position = EndPos;
			return;
		}
	}
}
