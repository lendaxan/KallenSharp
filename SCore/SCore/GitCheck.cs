using LeagueSharp;
using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using LeagueSharp.Common;
using Version = System.Version;

namespace SCore
{
    class GitCheck
    {
        private static string _gitPath, _updateMessage,_errorMessage;
        public GitCheck(String updateMessage,string gitPath,string errorMessage)
        {
            _updateMessage = updateMessage;
            _gitPath = gitPath;
            _errorMessage = errorMessage;
            Initilize();
        }

        private static void Initilize()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            CheckVersion();
        }
        public static void CheckVersion()
        {
            try
            {
                var match =
                                   new Regex(
                                       @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                                       .Match(DownloadServerVersion());

                if (!match.Success) return;
                var gitVersion =
                    new Version(
                        string.Format(
                            "{0}.{1}.{2}.{3}",
                            match.Groups[1],
                            match.Groups[2],
                            match.Groups[3],
                            match.Groups[4]));

                if (gitVersion <= Assembly.GetExecutingAssembly().GetName().Version) return;
                Game.PrintChat(_updateMessage, gitVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Game.PrintChat(_errorMessage);
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient()) return wC.DownloadString(_gitPath);
        }
    }
}
