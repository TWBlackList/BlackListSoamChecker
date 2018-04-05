using System.Collections.Generic;
using CNBlackListSoamChecker.DbManager;

namespace CNBlackListSoamChecker
{
    internal static class Temp
    {
        internal static bool DisableAdminTools = false; // If you need use /ban , please change it to false.
        internal static bool DisableBanList = false; // If you need ban user, plese change it to true.
        private static DatabaseManager databaseManager;
        internal static List<SpamMessage> spamMessageList = null;
        internal static Dictionary<long, GroupCfg> groupConfig = new Dictionary<long, GroupCfg>();
        internal static Dictionary<int, BanUser> bannedUsers = new Dictionary<int, BanUser>();
        internal static List<long> adminInReport = new List<long>();
        public static long AdminGroupID = -1001157582347; // If haven't, change it to 0
        public static long MainChannelID = -1001263651463; // If haven't, change it to 0
        public static long ReasonChannelID = -1001222529529; // If haven't, change it to 0
        public static long ReportGroupID = -1001157582347; // If haven't, change it to 0
        public static long CourtGroupID = -1001197757308; // If haven't, change it to 0
        public static long InternGroupID = -1001378349819; // If haven't, change it to 0
        public static string MainChannelName = "CNBL_main"; // If haven't, change it to null
        public static string ReasonChannelName = "CNBL_Reason"; // If haven't, change it to null
        public static string ReportGroupName = "ChineseBlackList"; //這ㄍ意思是 : 你他媽不能亂改群組username
        public static string CourtGroupName = "CNBL_Court"; //這ㄍ意思是 : 你他媽不能亂改群組username

        internal static DatabaseManager GetDatabaseManager()
        {
            if (databaseManager == null) databaseManager = new DatabaseManager();
            return databaseManager;
        }
    }
}