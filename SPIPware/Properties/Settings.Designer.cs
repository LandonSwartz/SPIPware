﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SPIPware.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM3")]
        public string P {
            get {
                return ((string)(this["P"]));
            }
            set {
                this["P"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("115200")]
        public int SerialPortBaud {
            get {
                return ((int)(this["SerialPortBaud"]));
            }
            set {
                this["SerialPortBaud"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Serial")]
        public global::SPIPware.Communication.ConnectionType ConnectionType {
            get {
                return ((global::SPIPware.Communication.ConnectionType)(this["ConnectionType"]));
            }
            set {
                this["ConnectionType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120")]
        public int ControllerBufferSize {
            get {
                return ((int)(this["ControllerBufferSize"]));
            }
            set {
                this["ControllerBufferSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int StatusPollInterval {
            get {
                return ((int)(this["StatusPollInterval"]));
            }
            set {
                this["StatusPollInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double ViewportArcSplit {
            get {
                return ((double)(this["ViewportArcSplit"]));
            }
            set {
                this["ViewportArcSplit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool EnableCodePreview {
            get {
                return ((bool)(this["EnableCodePreview"]));
            }
            set {
                this["EnableCodePreview"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public double ProbeSafeHeight {
            get {
                return ((double)(this["ProbeSafeHeight"]));
            }
            set {
                this["ProbeSafeHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double ProbeMinimumHeight {
            get {
                return ((double)(this["ProbeMinimumHeight"]));
            }
            set {
                this["ProbeMinimumHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public double ProbeMaxDepth {
            get {
                return ((double)(this["ProbeMaxDepth"]));
            }
            set {
                this["ProbeMaxDepth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AbortOnProbeFail {
            get {
                return ((bool)(this["AbortOnProbeFail"]));
            }
            set {
                this["AbortOnProbeFail"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public double ProbeFeed {
            get {
                return ((double)(this["ProbeFeed"]));
            }
            set {
                this["ProbeFeed"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double ArcToLineSegmentLength {
            get {
                return ((double)(this["ArcToLineSegmentLength"]));
            }
            set {
                this["ArcToLineSegmentLength"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public double SplitSegmentLength {
            get {
                return ((double)(this["SplitSegmentLength"]));
            }
            set {
                this["SplitSegmentLength"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8080")]
        public int WebServerPort {
            get {
                return ((int)(this["WebServerPort"]));
            }
            set {
                this["WebServerPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public double JogFeed {
            get {
                return ((double)(this["JogFeed"]));
            }
            set {
                this["JogFeed"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public double JogDistance {
            get {
                return ((double)(this["JogDistance"]));
            }
            set {
                this["JogDistance"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LogTraffic {
            get {
                return ((bool)(this["LogTraffic"]));
            }
            set {
                this["LogTraffic"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Probe and set Zero:G38.2Z-10F20\r\nG92Z0;")]
        public string Macros {
            get {
                return ((string)(this["Macros"]));
            }
            set {
                this["Macros"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool GCodeIncludeSpindle {
            get {
                return ((bool)(this["GCodeIncludeSpindle"]));
            }
            set {
                this["GCodeIncludeSpindle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool GCodeIncludeDwell {
            get {
                return ((bool)(this["GCodeIncludeDwell"]));
            }
            set {
                this["GCodeIncludeDwell"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool GCodeIncludeMEnd {
            get {
                return ((bool)(this["GCodeIncludeMEnd"]));
            }
            set {
                this["GCodeIncludeMEnd"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("250")]
        public double JogFeedCtrl {
            get {
                return ((double)(this["JogFeedCtrl"]));
            }
            set {
                this["JogFeedCtrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double JogDistanceCtrl {
            get {
                return ((double)(this["JogDistanceCtrl"]));
            }
            set {
                this["JogDistanceCtrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PauseFileOnHold {
            get {
                return ((bool)(this["PauseFileOnHold"]));
            }
            set {
                this["PauseFileOnHold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.95")]
        public double ProbeXAxisWeight {
            get {
                return ((double)(this["ProbeXAxisWeight"]));
            }
            set {
                this["ProbeXAxisWeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowStatusLines {
            get {
                return ((bool)(this["ShowStatusLines"]));
            }
            set {
                this["ShowStatusLines"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public double ConsoleFadeTime {
            get {
                return ((double)(this["ConsoleFadeTime"]));
            }
            set {
                this["ConsoleFadeTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double ToolLengthSetterPos {
            get {
                return ((double)(this["ToolLengthSetterPos"]));
            }
            set {
                this["ToolLengthSetterPos"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int SettingsSendDelay {
            get {
                return ((int)(this["SettingsSendDelay"]));
            }
            set {
                this["SettingsSendDelay"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool TLSUseActualPos {
            get {
                return ((bool)(this["TLSUseActualPos"]));
            }
            set {
                this["TLSUseActualPos"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ManualUseExpressions {
            get {
                return ((bool)(this["ManualUseExpressions"]));
            }
            set {
                this["ManualUseExpressions"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool FileExpanderOpen {
            get {
                return ((bool)(this["FileExpanderOpen"]));
            }
            set {
                this["FileExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string EditExpanderOpen {
            get {
                return ((string)(this["EditExpanderOpen"]));
            }
            set {
                this["EditExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OverrideExpanderOpen {
            get {
                return ((bool)(this["OverrideExpanderOpen"]));
            }
            set {
                this["OverrideExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ProbingExpanderOpen {
            get {
                return ((bool)(this["ProbingExpanderOpen"]));
            }
            set {
                this["ProbingExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ManualExpanderOpen {
            get {
                return ((bool)(this["ManualExpanderOpen"]));
            }
            set {
                this["ManualExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ManualProbingExpanderOpen {
            get {
                return ((bool)(this["ManualProbingExpanderOpen"]));
            }
            set {
                this["ManualProbingExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool MacroExpanderOpen {
            get {
                return ((bool)(this["MacroExpanderOpen"]));
            }
            set {
                this["MacroExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool MachineExpanderOpen {
            get {
                return ((bool)(this["MachineExpanderOpen"]));
            }
            set {
                this["MachineExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AboutExpanderOpen {
            get {
                return ((bool)(this["AboutExpanderOpen"]));
            }
            set {
                this["AboutExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DebugExpanderOpen {
            get {
                return ((bool)(this["DebugExpanderOpen"]));
            }
            set {
                this["DebugExpanderOpen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Einstellung {
            get {
                return ((bool)(this["Einstellung"]));
            }
            set {
                this["Einstellung"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7")]
        public int TotalPlates {
            get {
                return ((int)(this["TotalPlates"]));
            }
            set {
                this["TotalPlates"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int CurrentPlate {
            get {
                return ((int)(this["CurrentPlate"]));
            }
            set {
                this["CurrentPlate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int PlateOffset {
            get {
                return ((int)(this["PlateOffset"]));
            }
            set {
                this["PlateOffset"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("132")]
        public int BetweenDistance {
            get {
                return ((int)(this["BetweenDistance"]));
            }
            set {
                this["BetweenDistance"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\")]
        public string CameraSettingsPath {
            get {
                return ((string)(this["CameraSettingsPath"]));
            }
            set {
                this["CameraSettingsPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SPIPImages")]
        public string FileName {
            get {
                return ((string)(this["FileName"]));
            }
            set {
                this["FileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\")]
        public string SaveFolderPath {
            get {
                return ((string)(this["SaveFolderPath"]));
            }
            set {
                this["SaveFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CurrentPlateSave {
            get {
                return ((bool)(this["CurrentPlateSave"]));
            }
            set {
                this["CurrentPlateSave"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6")]
        public int NumLocationsRow {
            get {
                return ((int)(this["NumLocationsRow"]));
            }
            set {
                this["NumLocationsRow"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SelectAll {
            get {
                return ((bool)(this["SelectAll"]));
            }
            set {
                this["SelectAll"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int MinLocation {
            get {
                return ((int)(this["MinLocation"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int CurrentLocationX {
            get {
                return ((int)(this["CurrentLocationX"]));
            }
            set {
                this["CurrentLocationX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Mako")]
        public string CameraName {
            get {
                return ((string)(this["CameraName"]));
            }
            set {
                this["CameraName"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("X")]
        public string PrimaryAxis {
            get {
                return ((string)(this["PrimaryAxis"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public string Speed {
            get {
                return ((string)(this["Speed"]));
            }
            set {
                this["Speed"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Not Ready")]
        public string CameraState {
            get {
                return ((string)(this["CameraState"]));
            }
            set {
                this["CameraState"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime tlStartDate {
            get {
                return ((global::System.DateTime)(this["tlStartDate"]));
            }
            set {
                this["tlStartDate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime tlEndDate {
            get {
                return ((global::System.DateTime)(this["tlEndDate"]));
            }
            set {
                this["tlEndDate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool StartNow {
            get {
                return ((bool)(this["StartNow"]));
            }
            set {
                this["StartNow"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int tlInterval {
            get {
                return ((int)(this["tlInterval"]));
            }
            set {
                this["tlInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int tlEndInterval {
            get {
                return ((int)(this["tlEndInterval"]));
            }
            set {
                this["tlEndInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60000")]
        public long tlEndIntervalType {
            get {
                return ((long)(this["tlEndIntervalType"]));
            }
            set {
                this["tlEndIntervalType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60000")]
        public long tlIntervalType {
            get {
                return ((long)(this["tlIntervalType"]));
            }
            set {
                this["tlIntervalType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\")]
        public string ExperimentPath {
            get {
                return ((string)(this["ExperimentPath"]));
            }
            set {
                this["ExperimentPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\")]
        public string tlExperimentPath {
            get {
                return ((string)(this["tlExperimentPath"]));
            }
            set {
                this["tlExperimentPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\")]
        public string AcquisitionExperimentPath {
            get {
                return ((string)(this["AcquisitionExperimentPath"]));
            }
            set {
                this["AcquisitionExperimentPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SettingsUpdateRequired {
            get {
                return ((bool)(this["SettingsUpdateRequired"]));
            }
            set {
                this["SettingsUpdateRequired"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM5")]
        public string PeripheralSP {
            get {
                return ((string)(this["PeripheralSP"]));
            }
            set {
                this["PeripheralSP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("115200")]
        public int PeripheralBaud {
            get {
                return ((int)(this["PeripheralBaud"]));
            }
            set {
                this["PeripheralBaud"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Serial")]
        public global::SPIPware.Communication.ConnectionType PeripheralConType {
            get {
                return ((global::SPIPware.Communication.ConnectionType)(this["PeripheralConType"]));
            }
            set {
                this["PeripheralConType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFFFFFFF")]
        public global::System.Windows.Media.Color BacklightColor {
            get {
                return ((global::System.Windows.Media.Color)(this["BacklightColor"]));
            }
            set {
                this["BacklightColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool BacklightStatus {
            get {
                return ((bool)(this["BacklightStatus"]));
            }
            set {
                this["BacklightStatus"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool GrowlightStatus {
            get {
                return ((bool)(this["GrowlightStatus"]));
            }
            set {
                this["GrowlightStatus"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int CycleCount {
            get {
                return ((int)(this["CycleCount"]));
            }
            set {
                this["CycleCount"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("23:00:00")]
        public global::System.TimeSpan StartOfNight {
            get {
                return ((global::System.TimeSpan)(this["StartOfNight"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("07:00:00")]
        public global::System.TimeSpan EndOfNight {
            get {
                return ((global::System.TimeSpan)(this["EndOfNight"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Y")]
        public string SecondaryAxis {
            get {
                return ((string)(this["SecondaryAxis"]));
            }
            set {
                this["SecondaryAxis"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Z")]
        public string TertiaryAxis {
            get {
                return ((string)(this["TertiaryAxis"]));
            }
            set {
                this["TertiaryAxis"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int CurrentLocationY {
            get {
                return ((int)(this["CurrentLocationY"]));
            }
            set {
                this["CurrentLocationY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int CurrentLocationZ {
            get {
                return ((int)(this["CurrentLocationZ"]));
            }
            set {
                this["CurrentLocationZ"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6")]
        public int NumLocationsY {
            get {
                return ((int)(this["NumLocationsY"]));
            }
            set {
                this["NumLocationsY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7")]
        public int NumPlates {
            get {
                return ((int)(this["NumPlates"]));
            }
            set {
                this["NumPlates"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Y_Axis_Enable {
            get {
                return ((bool)(this["Y_Axis_Enable"]));
            }
            set {
                this["Y_Axis_Enable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Z_Axis_Enable {
            get {
                return ((bool)(this["Z_Axis_Enable"]));
            }
            set {
                this["Z_Axis_Enable"] = value;
            }
        }
    }
}
