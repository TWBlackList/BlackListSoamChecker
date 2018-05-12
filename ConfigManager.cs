using System;

namespace BlackListSoamChecker
{
    internal class ConfigManager
    {
        private static string ConfigPath;

        internal static string GetConfigPath()
        {
            if (ConfigPath == null)
            {
                string configPath = Environment.GetEnvironmentVariable("BOT_CONFIGPATH");
                if (configPath == "" || configPath == null)
                {
                    ConfigPath = @"plugincfg/soamchecker/";
                    System.IO.Directory.CreateDirectory(@"plugincfg/");
                    System.IO.Directory.CreateDirectory(@"plugincfg/soamchecker/");
                }
                else
                    ConfigPath = configPath + "/";
            }

            return ConfigPath;
        }
    }
}