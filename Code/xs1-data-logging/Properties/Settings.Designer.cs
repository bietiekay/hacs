﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace xs1_data_logging.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.178.36")]
        public string XS1 {
            get {
                return ((string)(this["XS1"]));
            }
            set {
                this["XS1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Admin")]
        public string Username {
            get {
                return ((string)(this["Username"]));
            }
            set {
                this["Username"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("92")]
        public int HTTPPort {
            get {
                return ((int)(this["HTTPPort"]));
            }
            set {
                this["HTTPPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.0.0.0")]
        public string HTTPIP {
            get {
                return ((string)(this["HTTPIP"]));
            }
            set {
                this["HTTPIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http")]
        public string HTTPDocumentRoot {
            get {
                return ((string)(this["HTTPDocumentRoot"]));
            }
            set {
                this["HTTPDocumentRoot"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int ConfigurationCacheMinutes {
            get {
                return ((int)(this["ConfigurationCacheMinutes"]));
            }
            set {
                this["ConfigurationCacheMinutes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ScriptingActorConfiguration.config")]
        public string ScriptingActorConfigurationFilename {
            get {
                return ((string)(this["ScriptingActorConfigurationFilename"]));
            }
            set {
                this["ScriptingActorConfigurationFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("password")]
        public string Password {
            get {
                return ((string)(this["Password"]));
            }
            set {
                this["Password"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PowerSensorConfiguration.config")]
        public string PowerSensorConfigurationFilename {
            get {
                return ((string)(this["PowerSensorConfigurationFilename"]));
            }
            set {
                this["PowerSensorConfigurationFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ScriptingTimerConfiguration.config")]
        public string ScriptingTimerConfigurationFilename {
            get {
                return ((string)(this["ScriptingTimerConfigurationFilename"]));
            }
            set {
                this["ScriptingTimerConfigurationFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("14")]
        public int DefaultSensorOutputPeriod {
            get {
                return ((int)(this["DefaultSensorOutputPeriod"]));
            }
            set {
                this["DefaultSensorOutputPeriod"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int SwitchAgainCheckpointMinutes {
            get {
                return ((int)(this["SwitchAgainCheckpointMinutes"]));
            }
            set {
                this["SwitchAgainCheckpointMinutes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public int SwitchAgainTimeWindowMinutes {
            get {
                return ((int)(this["SwitchAgainTimeWindowMinutes"]));
            }
            set {
                this["SwitchAgainTimeWindowMinutes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int AutomatedSensorCheck_ResponseTimeWindow {
            get {
                return ((int)(this["AutomatedSensorCheck_ResponseTimeWindow"]));
            }
            set {
                this["AutomatedSensorCheck_ResponseTimeWindow"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SensorCheckIgnoreList.config")]
        public string SensorCheckIgnoreFile {
            get {
                return ((string)(this["SensorCheckIgnoreFile"]));
            }
            set {
                this["SensorCheckIgnoreFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("log")]
        public string LogfileDirectory {
            get {
                return ((string)(this["LogfileDirectory"]));
            }
            set {
                this["LogfileDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000000")]
        public long DataObjectCacheSize {
            get {
                return ((long)(this["DataObjectCacheSize"]));
            }
            set {
                this["DataObjectCacheSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HTTPProxy.config")]
        public string HTTPProxyConfigurationFilename {
            get {
                return ((string)(this["HTTPProxyConfigurationFilename"]));
            }
            set {
                this["HTTPProxyConfigurationFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ELVMAXEnabled {
            get {
                return ((bool)(this["ELVMAXEnabled"]));
            }
            set {
                this["ELVMAXEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.178.82")]
        public string ELVMAXIP {
            get {
                return ((string)(this["ELVMAXIP"]));
            }
            set {
                this["ELVMAXIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("62910")]
        public int ELVMAXPort {
            get {
                return ((int)(this["ELVMAXPort"]));
            }
            set {
                this["ELVMAXPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int ELVMAXUpdateIntervalMsec {
            get {
                return ((int)(this["ELVMAXUpdateIntervalMsec"]));
            }
            set {
                this["ELVMAXUpdateIntervalMsec"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60000")]
        public int ELVMAXReconnectTimeMsec {
            get {
                return ((int)(this["ELVMAXReconnectTimeMsec"]));
            }
            set {
                this["ELVMAXReconnectTimeMsec"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int SensorCheckIntervalSec {
            get {
                return ((int)(this["SensorCheckIntervalSec"]));
            }
            set {
                this["SensorCheckIntervalSec"] = value;
            }
        }
    }
}
