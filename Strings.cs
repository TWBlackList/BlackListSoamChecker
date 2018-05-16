using System.Collections.Generic;
using BlackListSoamChecker.DbManager;

namespace BlackListSoamChecker
{
    internal static class Strings
    {
        static LanguageData getLangData()
        {
            if (tmp == null)
            {
                if (Config.Language == "TW")
                {
                    tmp = new LanguageData();
                }
            }
            return tmp;
        }
        
        internal static LanguageData lang = getLangData();
        internal static LanguageData tmp = null;

        internal static string CAN_NOT_GET = lang.CAN_NOT_GET;
        internal static string OUTPUT_DONE = lang.OUTPUT_DONE;
        internal static string HALAL = lang.HALAL;
        internal static string EXEC_OK = lang.EXEC_OK;
        internal static string EXEC_FAIL = lang.EXEC_FAIL;
        
        internal static string SUPERBAN_HELP_MESSAGE = lang.SUPERBAN_HELP_MESSAGE;
        internal static string SUPERBAN_ERROR_MESSAGE = lang.SUPERBAN_ERROR_MESSAGE;
        internal static string SUPERUNBAN_HELP_MESSAGE = lang.SUPERUNBAN_HELP_MESSAGE;
        internal static string SUPERUNBAN_ERROR_MESSAGE = lang.SUPERUNBAN_ERROR_MESSAGE;
        internal static string BAN_HELP_MESSAGE = lang.BAN_HELP_MESSAGE;
        internal static string BAN_ERROR_MESSAGE = lang.BAN_ERROR_MESSAGE;
        internal static string BAN_ERROR_NOTFOUNDUSERID_MESSAGE = lang.BAN_ERROR_NOTFOUNDUSERID_MESSAGE;
        internal static string BAN_ERROR_NOTFOUNDMSGID_MESSAGE = lang.BAN_ERROR_NOTFOUNDMSGID_MESSAGE;
        internal static string UNBAN_HELP_MESSAGE = lang.UNBAN_HELP_MESSAGE;
        internal static string UNBAN_ERROR_MESSAGE = lang.UNBAN_ERROR_MESSAGE;
        internal static string UNBAN_ERROR_USER_NOT_BANNED = lang.UNBAN_ERROR_USER_NOT_BANNED;
        internal static string BAN_ERROR_USER_IN_WHITELIST = lang.BAN_ERROR_USER_IN_WHITELIST;
        internal static string BAN_ERROR_USER_IN_HKWHITELIST = lang.BAN_ERROR_USER_IN_HKWHITELIST;
        
        
        internal static string HELP_AD = lang.HELP_AD;
        internal static string GROUP_HELP = lang.GROUP_HELP();
        internal static string SHARED_HELP = lang.SHARED_HELP();
        internal static string PRIVATE_HELP = lang.PRIVATE_HELP();
        internal static string OPERATIOR_HELP = lang.OPERATIOR_HELP();
        internal static string ADMIN_HELP = lang.ADMIN_HELP();
        
    }

    public class LanguageData
    {
        
        public string CAN_NOT_GET { get; set; } = "無法取得";
        public string OUTPUT_DONE { get; set; } = "輸出完畢";
        public string HALAL { get; set; } = "無法理解的語言";
        public string EXEC_OK { get; set; } = "操作成功。";
        public string EXEC_FAIL { get; set; } = "操作失敗 : ";
        
        public string SUPERBAN_HELP_MESSAGE { get; set; } = "/suban [i|id=1,2,3] [l|level=0] [m|minutes=0] [h|hours=0] [d|days=15] [f|from=f|fwd|r|reply] [halal [f|fwd|r|reply]]" +
                                                            " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                                                            "m: 分鐘, h: 小時, d: 天\n" +
                                                            "from 選項僅在 id 未被定義時起作用\n" +
                                                            "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                                                            "選項優先度: 簡寫 > 全名\n" +
                                                            "halal 選項只能單獨使用，不能與其他選項共同使用，並且需要回覆一則訊息，否則將觸發異常。\n\n" +
                                                            "Example:\n" +
                                                            "/suban id=1,2,3 m=0 h=0 d=15 level=0 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                                                            "/suban halal\n" +
                                                            "/suban halal=reply";
        public string SUPERBAN_ERROR_MESSAGE { get; set; } = "您的輸入有錯誤，請檢查您的輸入，或使用 /suban 查詢幫助。";
        
        public string SUPERUNBAN_HELP_MESSAGE { get; set; } = "/suunban [i|id=1,2,3] [f|from=f|fwd|r|reply]" +
                                                              " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                                                              "from 選項僅在 id 未被定義時起作用\n" +
                                                              "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                                                              "選項優先度: 簡寫 > 全名\n" +
                                                              "Example:\n" +
                                                              "/suunban id=1 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                                                              "/suunban";
        public string SUPERUNBAN_ERROR_MESSAGE { get; set; } = "您的輸入有錯誤，請檢查您的輸入，或使用 /suunban 取得幫助。";
        public string BAN_HELP_MESSAGE { get; set; } = "/" + Config.CustomPrefix + "ban [i|id=1] [l|level=0] [m|minutes=0] [h|hours=0] [d|days=15] [f|from=f|fwd|r|reply] [halal [f|fwd|r|reply]]" +
                                                       " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                                                       "m: 分鐘, h: 小時, d: 天\n" +
                                                       "from 選項僅在 id 未被定義時起作用\n" +
                                                       "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                                                       "選項優先度: 簡寫 > 全名\n" +
                                                       "halal 選項只能單獨使用，不能與其他選項共同使用，並且需要回覆一則訊息，否則將觸發異常。\n\n" +
                                                       "Example:\n" +
                                                       "/" + Config.CustomPrefix + "ban id=1 m=0 h=0 d=15 level=0 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                                                       "/" + Config.CustomPrefix + "ban halal\n" +
                                                       "/" + Config.CustomPrefix + "ban halal=reply";
        public string BAN_ERROR_MESSAGE { get; set; } = "您的輸入有錯誤，請檢查您的輸入，或使用 /" + Config.CustomPrefix + "ban 查詢幫助。";
        public string BAN_ERROR_NOTFOUNDUSERID_MESSAGE { get; set; } = "没有找到任何使用者 ID，請檢查您的輸入，或使用 /" + Config.CustomPrefix + "ban 查詢幫助。";
        public string BAN_ERROR_NOTFOUNDMSGID_MESSAGE { get; set; } = "未檢查到您指定的回覆訊息的 ID，請檢查您的輸入，或使用 /" + Config.CustomPrefix + "ban 查詢幫助。";
        public string BAN_ERROR_USER_IN_WHITELIST { get; set; } = "使用者在白名單";
        public string BAN_ERROR_USER_IN_HKWHITELIST { get; set; } = "使用者為港人";
        public string UNBAN_HELP_MESSAGE { get; set; } = "/" + Config.CustomPrefix + "unban [i|id=1] [f|from=f|fwd|r|reply]" +
                                                         " r|reason=\"asdfsadf asdfadsf\"\n\n" +
                                                         "from 選項僅在 id 未被定義時起作用\n" +
                                                         "ID 選擇優先度: 手動輸入 ID > 回覆的被轉發訊息 > 回覆的訊息\n" +
                                                         "選項優先度: 簡寫 > 全名\n" +
                                                         "Example:\n" +
                                                         "/" + Config.CustomPrefix + "unban id=1 reason=\"aaa bbb\\n\\\"ccc\\\" ddd\"\n" +
                                                         "/" + Config.CustomPrefix + "unban";
        public string UNBAN_ERROR_MESSAGE { get; set; } = "您的輸入有錯誤，請檢查您的輸入，或使用 /ban 取得幫助。";
        public string UNBAN_ERROR_USER_NOT_BANNED = " 使用者目前可能没有被封鎖 。";
        

        public string HELP_AD { get; set; } = "\n如要贊助本項目請洽[生產公司](http://t.me/DonateDoramiBot)";

        public string GROUP_HELP()
        {
            string tmp = "";
            if (Config.EnableLeave) tmp = tmp + "/leave - 離開群組\n";
            if (Config.EnableEnableSoam) tmp = tmp + "/soamenable - 啟用功能\n";
            if (Config.EnableDisableSoam) tmp = tmp + "/soamdisable - 關閉功能\n";
            if (Config.EnableSoamStatus) tmp = tmp + "/soamstatus - 取得目前群組開啟功能";
            return tmp;
        }
        
        public string SHARED_HELP()
        {
            string tmp = "";
            if (Config.EnableBanStat) tmp = tmp + "/banstat - 查詢處分狀態\n";
            if (Config.EnableUser) tmp = tmp + "/user - 取得 User ID\n";
            if (Config.EnableListOP) tmp = tmp + "/lsop - Operator 名冊";
            return tmp;
        }
        
        public string PRIVATE_HELP()
        {
            string tmp = "";
            return tmp;
        }
        
        public string OPERATIOR_HELP()
        {
            string tmp = "\n\nOperator指令:\n";
            if(Config.EnableGetGroupAdmin) tmp = tmp + "/groupadmin - 取得群組管理員名單\n";
            if(Config.EnableCustomBan) tmp = tmp + "/" + Config.CustomPrefix + "ban - 封鎖\n";
            if(Config.EnableBan) tmp = tmp + "/ban - 封鎖\n";
            if(Config.EnableCustomUnBan) tmp = tmp + "/" + Config.CustomPrefix + "unban - 解除封鎖\n";
            if(Config.EnableUnBan) tmp = tmp + "/unban - 解除封鎖\n";
            if(Config.EnableHKWhitelistAdd) tmp = tmp + "/addhk - 新增使用者至HK白名單\n";
            if(Config.EnableHKWhitelistDelete) tmp = tmp + "/delhk - 從HK白名單中刪除使用者\n";
            if(Config.EnableHKWhitelisList) tmp = tmp + "/lshk - 取得HK白名單列表\n";
            if(Config.EnableGetAllGroup) tmp = tmp + "/groups - 取得所有群組\n";
            if(Config.EnableGetSpamStringPoints) tmp = tmp + "/getspampoints - 測試關鍵字";
            return tmp;
        }
        
        public string ADMIN_HELP()
        {
            string tmp = "\n\nAdmin指令:\n";
            if(Config.EnableSuperBan) tmp = tmp + "/suban - 批次封鎖\n";
            if(Config.EnableSuperUnBan) tmp = tmp + "/suunban - 批次解除封鎖\n";
            if(Config.EnableAddSpamString) tmp = tmp + "/addspamstr - 新增 1 個自動規則\n";
            if(Config.EnableDeleteSpamString) tmp = tmp + "/delspamstr - 刪除 1 個自動規則\n";
            if(Config.EnableGetSpamString) tmp = tmp + "/getspamstr - 查看自動規則列表\n";
            if(Config.EnableGetAllSpamStringInfo) tmp = tmp + "/getallspamstr - 查看所有自動規則列表\n";
            if(Config.EnableReloadSpamString) tmp = tmp + "/reloadspamstr - 重新載入自動規則列表\n";
            if(Config.EnableGetSpamStringKeywords) tmp = tmp + "/points - 取得匹配到的關鍵字\n";
            if(Config.EnableBroadcast) tmp = tmp + "/say - 廣播\n";
            if(Config.EnableWhitelistAdd) tmp = tmp + "/addwl - 新增使用者至白名單\n";
            if(Config.EnableWhitelistDelete) tmp = tmp + "/delwl - 從白名單中刪除使用者\n";
            if(Config.EnableWhitelisList) tmp = tmp + "/lswl - 取得白名單列表\n";
            if(Config.EnableBlockListAdd) tmp = tmp + "/block - 新增群組至禁止使用名單\n";
            if(Config.EnableBlockListDelete) tmp = tmp + "/unblock - 從禁止使用名單中刪除群組\n";
            if(Config.EnableBlockListList) tmp = tmp + "/blocks - 取得禁止使用名單\n";
            if(Config.EnableAddOP) tmp = tmp + "/addop - 新增 Operator\n";
            if(Config.EnableDeleteOP) tmp = tmp + "/delop - 解除 Operator\n";
            if(Config.EnableDisableAllGroupSoam) tmp = tmp + "/seall - 開啟所有群組功能\n";
            if(Config.EnableEnableAllGroupSoam) tmp = tmp + "/sdall - 關閉所有群組功能\n";
            if(Config.EnableCleanUp) tmp = tmp + "/cleanup - 清理機器人不在群組內的群組資料\n";
            return tmp;
        }

    }
    
    
}