using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Gamejam.Utils;
using UnityEngine;

namespace OneP.GMConsole
{
    public partial class GMCommand
    {
        // Main logger
        protected static readonly OneP.GMConsole.GMLogger log = GMLogger.GetLogger(typeof(GMCommand));

        public GMCommand()
        {

        }


        [Description("Display all available command")]
        public static string Help()
        {
            string str = ConsoleCommands.GetAllCommandHelps();
            //UnityEngine.Debug.LogError(str);
            return str;
        }

        [Description("Display a specific command")]
        public static string Help(string command)
        {
            string str = ConsoleCommands.GetCommandHelp(command); ;
            //UnityEngine.Debug.LogError(str);
            return str;
        }

        [Description("Change current quality settings.")]
        [CmdDetail("Params> 0: Fastest, " +
            "1: Fast, " +
            "2: Simple, " +
            "3: Good, " +
            "4: Beautiful, " +
            "5: Fantastic.")]
        public static string Quality(int value)
        {
            QualitySettings.SetQualityLevel(value);
            return "Quality now set to: " + CheckQuality();
        }

        [Description("Get current quality settings.")]
        public static string CheckQuality()
        {
            return QualitySettings.names[QualitySettings.GetQualityLevel()] + " (" + QualitySettings.GetQualityLevel() + ")";
        }
    }
}


