﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace AmadeusAI
{
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.11.0.0")]
    internal sealed partial class Telegramsettings : global::System.Configuration.ApplicationSettingsBase
    {
        private static Telegramsettings defaultInstance = ((Telegramsettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Telegramsettings())));

        public static Telegramsettings Default
        {
            get { return defaultInstance; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TelegramBotToken
        {
            get { return ((string)(this["TelegramBotToken"])); }
            set { this["TelegramBotToken"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwilioAccountSid
        {
            get { return ((string)(this["TwilioAccountSid"])); }
            set { this["TwilioAccountSid"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwilioAuthToken
        {
            get { return ((string)(this["TwilioAuthToken"])); }
            set { this["TwilioAuthToken"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwilioPhoneNumber
        {
            get { return ((string)(this["TwilioPhoneNumber"])); }
            set { this["TwilioPhoneNumber"] = value; }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DestinationPhoneNumber
        {
            get { return ((string)(this["DestinationPhoneNumber"])); }
            set { this["DestinationPhoneNumber"] = value; }
        }
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ChatId
        {
            get { return ((string)(this["ChatId"])); }
            set { this["ChatId"] = value; }
        }
    }
}
