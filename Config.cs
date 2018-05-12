using System.Collections.Generic;
using BlackListSoamChecker.DbManager;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace BlackListSoamChecker
{
    internal static class Config
    {
        static BlackListConfig getJsonConfig()
        {

            System.Console.WriteLine("Loaded Json Config");
            
            string configPath = "./config_blacklist.json";
        
            if (File.Exists(configPath))
            {
                File.Create(configPath);
                DataContractJsonSerializer configJson = new DataContractJsonSerializer(typeof(BlackListConfig));  
                MemoryStream ms = new MemoryStream();  
                configJson.WriteObject(ms, new BlackListConfig());  
                ms.Position = 0;  
                StreamReader sr = new StreamReader(ms);  
                System.IO.File.WriteAllText(configPath, sr.ReadToEnd());
            }
        
            string json = File.ReadAllText(configPath);
            BlackListConfig data = (BlackListConfig) new DataContractJsonSerializer(
                typeof(BlackListConfig)
            ).ReadObject(
                new MemoryStream(
                    Encoding.UTF8.GetBytes(json)
                )
            );
            return data;
        }

        internal static BlackListConfig json = getJsonConfig();
        
        internal static bool DisableAdminTools = false; // 管理員功能，若需要的話改成 false，否則改成 true
        internal static bool DisableBanList = false; // 封鎖清單功能，若需要的話改成 false，否則改成 true
        
        internal static bool EnableOnlyJoinGroupInviteByOPS = false; // 讓Bot只加入OP拉取的群組
        internal static bool EnableAutoKickNotBanUserinCourtGroup = false; // 讓Bot自動在申訴群組踢除非被封鎖者
        
        private static DatabaseManager databaseManager;
        internal static List<SpamMessage> spamMessageList = null;
        internal static Dictionary<long, GroupCfg> groupConfig = new Dictionary<long, GroupCfg>();
        internal static Dictionary<int, BanUser> bannedUsers = new Dictionary<int, BanUser>();
        internal static List<long> adminInReport = new List<long>();
        internal static List<long> adminChecking = new List<long>();
        
        // 0 = Enable , 1 = Disable
        internal static int DefaultSoamAdminOnly = json.DefaultSoam.AdminOnly;                 // 只允許管理員使用指令
        internal static int DefaultSoamBlacklist = json.DefaultSoam.Blacklist;                 // 黑名單功能
        internal static int DefaultSoamAutoKick = json.DefaultSoam.AutoKick;                  // 自動踢除
        internal static int DefaultSoamAntiBot = json.DefaultSoam.AntiBot;                   // 自動踢除拉機器人的人
        internal static int DefaultSoamAntiHalal = json.DefaultSoam.AntiHalal;                 // 防清真
        internal static int DefaultSoamAutoDeleteSpamMessage = json.DefaultSoam.AutoDeleteSpamMessage;     // 自動刪除 Spam 訊息
        internal static int DefaultSoamAutoDeleteCommand = json.DefaultSoam.AutoDeleteCommand;         // 自動刪除指令
        internal static int DefaultSoamSubscribeBanList = json.DefaultSoam.SubscribeBanList;          // 訂閱封鎖清單
        
        public static long AdminGroupID = json.AdminGroupID; // If haven't, change it to 0
        public static long MainChannelID = json.MainChannelID; // If haven't, change it to 0
        public static long ReasonChannelID = json.ReasonChannelID; // If haven't, change it to 0
        public static long ReportGroupID = json.ReportGroupID; // If haven't, change it to 0
        public static long CourtGroupID = json.CourtGroupID; // If haven't, change it to 0
        public static long InternGroupID = json.InternGroupID; // If haven't, change it to 0
        public static string MainChannelName = json.MainChannelName; // If haven't, change it to null
        public static string ReasonChannelName = json.ReasonChannelName; // If haven't, change it to null
        public static string ReportGroupName = json.ReportGroupName; //這ㄍ意思是 : 你他媽不能亂改群組username
        public static string CourtGroupName =json.CourtGroupName; //這ㄍ意思是 : 你他媽不能亂改群組username

        internal static DatabaseManager GetDatabaseManager()
        {
            if (databaseManager == null) databaseManager = new DatabaseManager();
            return databaseManager;
        }
    }
    
    public class BlackListConfig
    {
        public bool DisableAdminTools { get; set; } = false;
        public bool DisableBanList { get; set; } = false;
        
        public long AdminGroupID { get; set; } = 0;
        
        public long MainChannelID { get; set; } = 0;
        public string MainChannelName { get; set; } = null;
        
        public long ReasonChannelID { get; set; } = 0;
        public string ReasonChannelName { get; set; } = null;
        
        public long ReportGroupID { get; set; } = 0;
        public string ReportGroupName { get; set; } = null;
        
        public long CourtGroupID { get; set; } = 0;
        public string CourtGroupName { get; set; } = null;
        
        public long InternGroupID { get; set; } = 0;
        
        public DefaultSoam DefaultSoam { get; set; } = new DefaultSoam();
    }
    
    public class DefaultSoam
    {
        public int AdminOnly { get; set; } = 1;
        public int Blacklist { get; set; } = 0;
        public int AutoKick { get; set; } = 0;
        public int AntiBot { get; set; } = 1;
        public int AntiHalal { get; set; } = 1;
        public int AutoDeleteSpamMessage { get; set; } = 1;
        public int AutoDeleteCommand { get; set; } = 1;
        public int SubscribeBanList { get; set; } = 0;
    }
    
}