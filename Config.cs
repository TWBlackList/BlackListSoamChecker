using System.Collections.Generic;
using BlackListSoamChecker.DbManager;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Linq;

namespace BlackListSoamChecker
{
    internal static class Config
    {
        static BlackListConfig getJsonConfig()
        {
            if (tmp == null)
            {
                System.Console.WriteLine("Loaded Json Config");
            
                string configPath = "./config_blacklist.json";
        
                if (!File.Exists(configPath))
                {
                    DataContractJsonSerializer configJson = new DataContractJsonSerializer(typeof(BlackListConfig));  
                    MemoryStream ms = new MemoryStream();  
                    configJson.WriteObject(ms, new BlackListConfig());  
                    ms.Position = 0;  
                    StreamReader sr = new StreamReader(ms);  
                    System.IO.File.WriteAllText(configPath, JsonHelper.FormatJson(sr.ReadToEnd()));
                }
        
                string json = File.ReadAllText(configPath);
                BlackListConfig data = (BlackListConfig) new DataContractJsonSerializer(
                    typeof(BlackListConfig)
                ).ReadObject(
                    new MemoryStream(
                        Encoding.UTF8.GetBytes(json)
                    )
                );
                tmp = data;
                return data;
            }

            return null;
        }

        private static DatabaseManager databaseManager;
        internal static List<SpamMessage> spamMessageList = null;
        internal static Dictionary<long, GroupCfg> groupConfig = new Dictionary<long, GroupCfg>();
        internal static Dictionary<int, BanUser> bannedUsers = new Dictionary<int, BanUser>();
        internal static List<long> adminInReport = new List<long>();
        internal static List<long> adminChecking = new List<long>();
        
        internal static BlackListConfig json = getJsonConfig();
        internal static BlackListConfig tmp = null;
        
        internal static bool DisableAdminTools = json.Functions.DisableAdminTools;                                      // 管理員功能，若需要的話改成 false，否則改成 true
        internal static bool DisableBanList = json.Functions.DisableBanList;                                            // 封鎖清單功能，若需要的話改成 false，否則改成 true
        
        internal static bool EnableOnlyJoinGroupInviteByAdmin = json.Setting.EnableOnlyJoinGroupInviteByAdmin;          // 讓Bot只加入OP拉取的群組
        internal static bool EnableAutoKickNotBanUserinCourtGroup = json.Setting.EnableAutoKickNotBanUserinCourtGroup;  // 讓Bot自動在申訴群組踢除非被封鎖者
        
        // 0 = Disable , 1 = Enable
        internal static int DefaultSoamAdminOnly = json.DefaultSoam.AdminOnly ? 0 : 1;                             // 只允許管理員使用指令
        internal static int DefaultSoamBlacklist = json.DefaultSoam.Blacklist ? 0 : 1;                             // 黑名單功能
        internal static int DefaultSoamAutoKick = json.DefaultSoam.AutoKick ? 0 : 1;                               // 自動踢除
        internal static int DefaultSoamAntiBot = json.DefaultSoam.AntiBot ? 0 : 1;                                 // 自動踢除拉機器人的人
        internal static int DefaultSoamAntiHalal = json.DefaultSoam.AntiHalal ? 0 : 1;                             // 防清真
        internal static int DefaultSoamAutoDeleteSpamMessage = json.DefaultSoam.AutoDeleteSpamMessage ? 0 : 1;     // 自動刪除 Spam 訊息
        internal static int DefaultSoamAutoDeleteCommand = json.DefaultSoam.AutoDeleteCommand ? 0 : 1;             // 自動刪除指令
        internal static int DefaultSoamSubscribeBanList = json.DefaultSoam.SubscribeBanList ? 0 : 1;               // 訂閱封鎖清單
        
        public static long AdminGroupID = json.Chats.AdminGroupID;                                                      // 管理用群組
        
        public static long MainChannelID = json.Chats.MainChannelID;                                                    // 主要頻道 ChatID ( 封鎖通知 )
        public static string MainChannelName = json.Chats.MainChannelName;                                              // 主要頻道 Username
        
        public static long ReasonChannelID = json.Chats.ReasonChannelID;                                                // 原因頻道 ChatID ( 封鎖原因 )
        public static string ReasonChannelName = json.Chats.ReasonChannelName;                                          // 原因頻道 Username
        
        public static long ReportGroupID = json.Chats.ReportGroupID;                                                    // 回報群組 ChatID ( 回報SPAM )
        public static string ReportGroupName = json.Chats.ReportGroupName;                                              // 回報群組 Username

        public static long CourtGroupID = json.Chats.CourtGroupID;                                                      // 申訴群組 ChatID ( 申訴 )
        public static string CourtGroupName =json.Chats.CourtGroupName;                                                 // 申訴群組 UserName
        
        public static long InternGroupID = json.Chats.InternGroupID;                                                    // 內部群組 ChatID ( 此群組內的所有人都可對Bot轉發的訊息執行封鎖 )

        internal static DatabaseManager GetDatabaseManager()
        {
            if (databaseManager == null) databaseManager = new DatabaseManager();
            return databaseManager;
        }
    }
    
    public class BlackListConfig
    {
        public string Language { get; set; } = "zh_TW";
        public Chats Chats { get; set; } = new Chats();
        public Setting Setting { get; set; } = new Setting();
        public DefaultSoam DefaultSoam { get; set; } = new DefaultSoam();
        public Functions Functions { get; set; } = new Functions();
    }

    public class Chats
    {
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
    }


    public class Setting
    {
        internal static bool EnableOnlyJoinGroupInviteByAdmin = false; // 讓Bot只加入Admin拉取的群組
        internal static bool EnableAutoKickNotBanUserinCourtGroup = false; // 讓Bot自動在申訴群組踢除非被封鎖者
    }

    public class Functions
    {
        public bool DisableAdminTools { get; set; } = false;
        public bool DisableBanList { get; set; } = false;
        public BanFunctions BanFunctions { get; set; } = new BanFunctions();
        public SpamStringFunctions SpamStringFunctions { get; set; } = new SpamStringFunctions();
        public SoamFunctions SoamFunctions { get; set; } = new SoamFunctions();
        public AdminFunctions AdminFunctions { get; set; } = new AdminFunctions();
        public WhiteListFunctions WhiteListFunctions { get; set; } = new WhiteListFunctions();
        public BlockListFunctions BlockListFunctions { get; set; } = new BlockListFunctions();
    }

    public class BanFunctions 
    {
        public bool EnableCustomBan { get; set; } = true;
        public bool EnableCustomUnBan { get; set; } = true;
        
        public bool EnableBan { get; set; } = true;
        public bool EnableUnBan { get; set; } = true;
        
        public bool EnableSuperBan { get; set; } = true;
        public bool EnableSuperUnBan { get; set; } = true;
        
        public bool EnableBanStat { get; set; } = true;
    }
    
    public class SpamStringFunctions
    {
        public bool EnableGetAllSpamStringInfo { get; set; } = true;
        
        public bool EnableAddSpamString { get; set; } = true;
        public bool EnableDeleteSpamString { get; set; } = true;
        public bool EnableGetSpamString { get; set; } = true;
        
        public bool EnableReloadSpamString { get; set; } = true;
        public bool EnableGetSpamStringPoints { get; set; } = true;
    }
    
    public class SoamFunctions
    {
        public bool EnableEnableAll { get; set; } = true;
        public bool EnableDisableAll { get; set; } = true;
        
        public bool EnableEnableSoam { get; set; } = true;
        public bool EnableDisableSoam { get; set; } = true;
        public bool EnableSoamStatus { get; set; } = true;
    }
    
    public class AdminFunctions
    {
        public bool EnableGetAllGroup { get; set; } = true;
        public bool EnableGetGroupAdmin { get; set; } = true;
        
        public bool EnableCleanUp { get; set; } = true;
        
        public bool EnableBroadcast { get; set; } = true;
        
        public bool EnableAddOP { get; set; } = true;
        public bool EnableDeleteOP { get; set; } = true;
        
        public bool EnableLeave { get; set; } = true;
    }
    
    public class WhiteListFunctions
    {
        public bool EnableWhitelistAdd { get; set; } = true;
        public bool EnableWhitelistDelete { get; set; } = true;
        public bool EnableWhitelisList { get; set; } = true;
        
        public bool EnableHKWhitelistAdd { get; set; } = true;
        public bool EnableHKWhitelistDelete { get; set; } = true;
        public bool EnableHKWhitelisList { get; set; } = true;
    }
    
    public class BlockListFunctions
    {
        public bool EnableBlockListAdd { get; set; } = true;
        public bool EnableBlockListDelete { get; set; } = true;
        public bool EnableBlockListList { get; set; } = true;
    }

    public class DefaultSoam
    {
        public bool AdminOnly { get; set; } = false;
        public bool Blacklist { get; set; } = true;
        public bool AutoKick { get; set; } = true;
        public bool AntiBot { get; set; } = false;
        public bool AntiHalal { get; set; } = false;
        public bool AutoDeleteSpamMessage { get; set; } = false;
        public bool AutoDeleteCommand { get; set; } = false;
        public bool SubscribeBanList { get; set; } = true;
    }
    
    // code from : https://gist.github.com/wcoder/c24050c166b139739301
    class JsonHelper
    {
        public static string FormatJson(string str, string indentString = "    ")
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, ++indent))
                                sb.Append(indentString);
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, --indent))
                                sb.Append(indentString);
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, indent))
                                sb.Append(indentString);
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
    
}