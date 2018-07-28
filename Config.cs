using System.Collections.Generic;
using BlackListSoamChecker.DbManager;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Linq;
using ReimuAPI.ReimuBase;

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

            return tmp;
        }

        
        private static DatabaseManager databaseManager;
        internal static List<SpamMessage> spamMessageList = null;
        internal static Dictionary<long, GroupCfg> groupConfig = new Dictionary<long, GroupCfg>();
        internal static Dictionary<int, BanUser> bannedUsers = new Dictionary<int, BanUser>();
        internal static Dictionary<string, IDList> IDList = new Dictionary<string, IDList>();
        internal static List<long> adminInReport = new List<long>();
        internal static List<long> adminChecking = new List<long>();
        
        
        internal static BlackListConfig json = getJsonConfig();
        internal static BlackListConfig tmp = null;
        
        
        internal static string Language = json.Setting.Language;                                                        // 語言
        internal static string CustomPrefix = json.Setting.CustomPrefix;                                                // 自訂前綴
        
        internal static int DefaultBanDay = json.Setting.DefaultBanDay;                                                    // 預設封鎖天數
        internal static int DefaultHalalBanDay = json.Setting.DefaultHalalBanDay;                                    // 常用封鎖: 無法理解的語言 天數
        internal static int DefaultSpamBanDay = json.Setting.DefaultSpamBanDay;                                      // 常用封鎖: 垃圾/無意義/濫發訊 天數
        internal static int DefaultSpammerBanDay = json.Setting.DefaultSpammerBanDay;                                // 常用封鎖: 拉人訊息or用戶名/廣告用戶名 天數
        internal static int DefaultInNsfwBanDay = json.Setting.DefaultInNsfwBanDay;                                  // 常用封鎖: 裸露/暴力/色情/非公眾適宜訊息 天數
        internal static int DefaultOutNsfwBanDay = json.Setting.DefaultOutNsfwBanDay;                                // 常用封鎖: 連外裸露/暴力/色情/非公眾適宜訊息 天數
        internal static int DefaultCoinBanDay = json.Setting.DefaultCoinBanDay;                                      // 常用封鎖: 虛擬貨幣廣告 天數

        internal static bool DisableAdminTools = json.Setting.DisableAdminTools;                                        // 管理員功能，若需要的話改成 false，否則改成 true
        internal static bool DisableBanList = json.Setting.DisableBanList;                                              // 封鎖清單功能，若需要的話改成 false，否則改成 true

        internal static bool EnableAutoLeaveNormalGroup = json.Setting.EnableAutoLeaveNormalGroup;                      // 自動離開普通群組
        internal static bool EnableOnlyJoinGroupInviteByAdmin = json.Setting.EnableOnlyJoinGroupInviteByAdmin;          // 讓Bot只加入Admin拉取的群組
        internal static bool EnableAutoKickNotBanUserinCourtGroup = json.Setting.EnableAutoKickNotBanUserinCourtGroup;  // 讓Bot自動在申訴群組踢除非被封鎖者
        
        
        // 0 = Disable , 1 = Enable
        internal static int DefaultSoamAdminOnly = json.DefaultSoam.AdminOnly ? 0 : 1;                                  // 只允許管理員使用指令
        internal static int DefaultSoamBlacklist = json.DefaultSoam.Blacklist ? 0 : 1;                                  // 黑名單功能
        internal static int DefaultSoamAutoKick = json.DefaultSoam.AutoKick ? 0 : 1;                                    // 自動踢除
        internal static int DefaultSoamAntiBot = json.DefaultSoam.AntiBot ? 0 : 1;                                      // 自動踢除拉機器人的人
        internal static int DefaultSoamAntiHalal = json.DefaultSoam.AntiHalal ? 0 : 1;                                  // 防清真
        internal static int DefaultSoamAutoDeleteSpamMessage = json.DefaultSoam.AutoDeleteSpamMessage ? 0 : 1;          // 自動刪除 Spam 訊息
        internal static int DefaultSoamAutoDeleteCommand = json.DefaultSoam.AutoDeleteCommand ? 0 : 1;                  // 自動刪除指令
        internal static int DefaultSoamSubscribeBanList = json.DefaultSoam.SubscribeBanList ? 0 : 1;                    // 訂閱封鎖清單
        
        
        internal static long AdminGroupID = json.Chats.AdminGroupID;                                                    // 管理用群組
        
        internal static long MainChannelID = json.Chats.MainChannelID;                                                  // 主要頻道 ChatID ( 封鎖通知 )
        internal static string MainChannelName = json.Chats.MainChannelName;                                            // 主要頻道 Username
        
        internal static long ReasonChannelID = json.Chats.ReasonChannelID;                                              // 原因頻道 ChatID ( 封鎖原因 )
        internal static string ReasonChannelName = json.Chats.ReasonChannelName;                                        // 原因頻道 Username
        
        internal static long ReportGroupID = json.Chats.ReportGroupID;                                                  // 回報群組 ChatID ( 回報SPAM )
        internal static string ReportGroupName = json.Chats.ReportGroupName;                                            // 回報群組 Username

        internal static long CourtGroupID = json.Chats.CourtGroupID;                                                    // 申訴群組 ChatID ( 申訴 )
        internal static string CourtGroupName =json.Chats.CourtGroupName;                                               // 申訴群組 UserName
        
        internal static long AdminContactGroupID = json.Chats.AdminContactGroupID;                                      // 溝通群組 ChatID ( 申訴 )
        internal static string AdminContactGroupName =json.Chats.AdminContactGroupName;                                 // 溝通群組 UserName
        
        internal static long InternGroupID = json.Chats.InternGroupID;                                                  // 內部群組 ChatID ( 此群組內的所有人都可對Bot轉發的訊息執行封鎖 )
        
        
        internal static bool EnableUser = json.BasicFunctions.EnableUser;                                               // 開啟取得用戶ID
        internal static bool EnableGroupID = json.BasicFunctions.EnableGroupID;                                         // 開啟取得群組ID
        internal static bool EnableHelp = json.BasicFunctions.EnableHelp;                                               // 開啟取得幫助
        
        
        internal static bool EnableCustomBan = json.BanFunctions.EnableCustomBan;                                       // 開啟擁有前綴的 ban 指令
        internal static bool EnableCustomUnBan = json.BanFunctions.EnableCustomUnBan;                                   // 開啟擁有前綴的 unban 指令
        internal static bool EnableCustomBanStat = json.BanFunctions.EnableCustomBanStat;                               // 開啟擁有前綴的 unban 指令
        internal static bool EnableBan = json.BanFunctions.EnableBan;                                                   // 開啟封鎖指令
        internal static bool EnableUnBan = json.BanFunctions.EnableUnBan;                                               // 開啟解除封鎖指令
        internal static bool EnableSuperBan = json.BanFunctions.EnableSuperBan;                                         // 開啟多重封鎖指令
        internal static bool EnableSuperUnBan = json.BanFunctions.EnableSuperUnBan;                                     // 開啟多重解除封鎖指令
        internal static bool EnableBanStat = json.BanFunctions.EnableBanStat;                                           // 開啟封鎖狀態查詢
        
       
        internal static bool EnableGetAllSpamStringInfo = json.SpamStringFunctions.EnableGetAllSpamStringInfo;          // 開啟取得所有 SPAM 規則
        internal static bool EnableAddSpamString = json.SpamStringFunctions.EnableAddSpamString;                        // 開啟新增 SPAM 規則
        internal static bool EnableDeleteSpamString = json.SpamStringFunctions.EnableDeleteSpamString;                  // 開啟刪除 SPAM 規則
        internal static bool EnableGetSpamString = json.SpamStringFunctions.EnableGetSpamString;                        // 開啟取得 SPAM 規則
        internal static bool EnableReloadSpamString = json.SpamStringFunctions.EnableReloadSpamString;                  // 開啟重新讀取 SPAM 規則
        internal static bool EnableGetSpamStringPoints = json.SpamStringFunctions.EnableGetSpamStringPoints;            // 開啟測試 SPAM 點數
        internal static bool EnableGetSpamStringKeywords = json.SpamStringFunctions.EnableGetSpamStringKeywords;        // 開啟測試 SPAM 關鍵字
        
        
        internal static bool EnableEnableAllGroupSoam = json.SoamFunctions.EnableEnableAllGroupSoam;                    // 開啟啟用所有群組功能
        internal static bool EnableDisableAllGroupSoam = json.SoamFunctions.EnableDisableAllGroupSoam;                  // 開啟關閉所有群組功能
        internal static bool EnableEnableSoam = json.SoamFunctions.EnableEnableSoam;                                    // 開啟啟用群組功能
        internal static bool EnableDisableSoam = json.SoamFunctions.EnableDisableSoam;                                  // 開啟關閉群組功能
        internal static bool EnableSoamStatus = json.SoamFunctions.EnableSoamStatus;                                    // 開啟取得群組功能狀態
        
        
        internal static bool EnableGetAllGroup = json.AdminFunctions.EnableGetAllGroup;                                 // 開啟取得所有群組資訊
        internal static bool EnableGetGroupAdmin = json.AdminFunctions.EnableGetGroupAdmin;                             // 開啟取得群組管理員名單
        internal static bool EnableCleanUp = json.AdminFunctions.EnableCleanUp;                                         // 開啟清除未使用的群組
        internal static bool EnableBroadcast = json.AdminFunctions.EnableBroadcast;                                     // 開啟廣播
        internal static bool EnableAddOP = json.AdminFunctions.EnableAddOP;                                             // 開啟新增 OP
        internal static bool EnableDeleteOP = json.AdminFunctions.EnableDeleteOP;                                       // 開啟刪除 OP
        internal static bool EnableListOP = json.AdminFunctions.EnableListOP;                                           // 開啟取得 OP 清單
        internal static bool EnableLeave = json.AdminFunctions.EnableLeave;                                             // 開啟取得 OP 清單
        
        
        internal static bool EnableWhitelistAdd = json.WhiteListFunctions.EnableWhitelistAdd;                           // 開啟新增白名單
        internal static bool EnableWhitelistDelete = json.WhiteListFunctions.EnableWhitelistDelete;                     // 開啟刪除白名單
        internal static bool EnableWhitelisList = json.WhiteListFunctions.EnableWhitelisList;                           // 開啟白名單清單
        internal static bool EnableHKWhitelistAdd = json.WhiteListFunctions.EnableHKWhitelistAdd;                       // 開啟新增HK白名單
        internal static bool EnableHKWhitelistDelete = json.WhiteListFunctions.EnableHKWhitelistDelete;                 // 開啟刪除HK白名單
        internal static bool EnableHKWhitelisList = json.WhiteListFunctions.EnableHKWhitelisList;                       // 開啟HK白名單清單
        
        
        internal static bool EnableBlockListAdd = json.BlockListFunctions.EnableBlockListAdd;                           // 開啟新增拒絕服務名單
        internal static bool EnableBlockListDelete = json.BlockListFunctions.EnableBlockListDelete;                     // 開啟刪除拒絕服務名單
        internal static bool EnableBlockListList = json.BlockListFunctions.EnableBlockListList;                         // 開啟拒絕服務名單清單
        

        internal static DatabaseManager GetDatabaseManager()
        {
            if (databaseManager == null) databaseManager = new DatabaseManager();
            return databaseManager;
        }

        internal static IDList WhiteList = GetDatabaseManager().GetIDList("WhiteList");
        internal static IDList HKWhiteList = GetDatabaseManager().GetIDList("HKWhiteList");
        internal static IDList BlockGroups = GetDatabaseManager().GetIDList("BlockGroups");
        internal static IDList SpamBlackList = GetDatabaseManager().GetIDList("SpamBlackList");
        
        internal static bool GetIsInWhiteList(int id)
        {
            if (RAPI.getIsBotOP(id) || RAPI.getIsBotAdmin(id))
                return true;
            if (WhiteList.CheckInList(id) || HKWhiteList.CheckInList(id))
                return true;
            return false;
        }
        internal static bool GetIsInWhiteList(long id)
        {
            if (WhiteList.CheckInList(id))
                return true;
            return false;
        }
    }
    
    public class BlackListConfig
    {
        public Chats Chats { get; set; } = new Chats();
        public Setting Setting { get; set; } = new Setting();
        public DefaultSoam DefaultSoam { get; set; } = new DefaultSoam();
        public BasicFunctions BasicFunctions { get; set; } = new BasicFunctions();
        public BanFunctions BanFunctions { get; set; } = new BanFunctions();
        public SpamStringFunctions SpamStringFunctions { get; set; } = new SpamStringFunctions();
        public SoamFunctions SoamFunctions { get; set; } = new SoamFunctions();
        public AdminFunctions AdminFunctions { get; set; } = new AdminFunctions();
        public WhiteListFunctions WhiteListFunctions { get; set; } = new WhiteListFunctions();
        public BlockListFunctions BlockListFunctions { get; set; } = new BlockListFunctions();
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
        
        public long AdminContactGroupID { get; set; } = 0;
        public string AdminContactGroupName { get; set; } = null;
        
        public long InternGroupID { get; set; } = 0;
    }


    public class Setting
    {
        public string Language { get; set; } = "zh_TW";
        public string CustomPrefix { get; set; } = "tw";
        public bool EnableOnlyJoinGroupInviteByAdmin { get; set; } = false; // 讓Bot只加入Admin拉取的群組
        public bool EnableAutoKickNotBanUserinCourtGroup { get; set; } = false; // 讓Bot自動在申訴群組踢除非被封鎖者
        public bool EnableAutoLeaveNormalGroup { get; set; } = true; // 自動離開普通群組
        public bool DisableAdminTools { get; set; } = false;
        public bool DisableBanList { get; set; } = false;
        public int DefaultBanDay { get; set; } = 90;
        public int DefaultHalalBanDay { get; set; } = 0;
        public int DefaultSpamBanDay { get; set; } = 7;
        public int DefaultSpammerBanDay { get; set; } = 0;
        public int DefaultInNsfwBanDay { get; set; } = 90;
        public int DefaultOutNsfwBanDay { get; set; } = 0;
        public int DefaultCoinBanDay { get; set; } = 90;

    }



    public class BasicFunctions
    {
        public bool EnableUser { get; set; } = true;
        public bool EnableHelp { get; set; } = true;
        public bool EnableGroupID { get; set; } = true;
    }

    public class BanFunctions 
    {
        public bool EnableCustomBan { get; set; } = true;
        public bool EnableCustomUnBan { get; set; } = true;
        public bool EnableCustomBanStat { get; set; } = true;
        
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
        public bool EnableGetSpamStringKeywords { get; set; } = true;
        public bool EnableGetSpamStringPoints { get; set; } = true;
    }
    
    public class SoamFunctions
    {
        public bool EnableEnableAllGroupSoam { get; set; } = true;
        public bool EnableDisableAllGroupSoam { get; set; } = true;
        
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
        public bool EnableListOP { get; set; } = true;
        
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