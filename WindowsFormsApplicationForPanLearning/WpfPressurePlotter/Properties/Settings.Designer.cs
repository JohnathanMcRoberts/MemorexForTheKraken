﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfPressurePlotter.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\E142890\\Documents\\LAS Files\\REP[1].las")]
        public string LasFile {
            get {
                return ((string)(this["LasFile"]));
            }
            set {
                this["LasFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\E142890\\Documents\\Iceland.PNG")]
        public string LastPngFile {
            get {
                return ((string)(this["LastPngFile"]));
            }
            set {
                this["LastPngFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\E142890\\MemorexForTheKraken\\Projects\\WindowsFormsApplicationForPanLearni" +
            "ng\\WpfPressurePlotter\\GeoData\\ConstituencyAndNeighbourDistances.xml")]
        public string NeighbourDistancesFile {
            get {
                return ((string)(this["NeighbourDistancesFile"]));
            }
            set {
                this["NeighbourDistancesFile"] = value;
            }
        }
    }
}
