using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker
{
    internal class SubscribeBanListCaller
    {
        internal void CallGroupsInThread(BanUser user)
        {
            new Thread(delegate() { CallGroups(user); }).Start();
        }

        internal void CallGroups(BanUser user)
        {
            if (Config.DisableAdminTools) return;
            if (user.Level == 1)
                return;
            using (var db = new BlacklistDatabaseContext())
            {
                List<GroupCfg> groupCfg = null;
                try
                {
                    groupCfg = db.GroupConfig
                        .Where(cfg => cfg.SubscribeBanList == 0)
                        .ToList();
                }
                catch (InvalidOperationException)
                {
                    return;
                }

                if (groupCfg == null) return;
                
                var random = new Random();
                groupCfg = groupCfg.OrderBy (x => random.Next()).ToList();
                
                foreach (GroupCfg cfg in groupCfg)
                {
                    var userInChatInfo = TgApi.getDefaultApiConnection().getChatMember(cfg.GroupID, user.UserID);
                    if (userInChatInfo.ok)
                        if (userInChatInfo.result.status == "member")
                            try
                            {
                                new Thread(delegate()
                                {
                                    Console.WriteLine("[SubscribeBanList] Ban " + user.UserID +
                                                      " in " + cfg.GroupID);
                                    //TgApi.getDefaultApiConnection().restrictChatMember(
                                    //    cfg.GroupID,
                                    //    user.UserID,
                                    //    GetTime.GetUnixTime() + 10,
                                    //    false);
                                    SendMessageResult result = TgApi.getDefaultApiConnection().sendMessage(
                                        cfg.GroupID,
                                        "使用者 : `" + user.UserID + "`\n" + user.GetBanMessage_MD() +
                                        "\n\n由於開啟了 SubscribeBanList ，已嘗試自動移除。",
                                        ParseMode: TgApi.PARSEMODE_MARKDOWN);
                                    Thread.Sleep(5000);
                                    TgApi.getDefaultApiConnection()
                                        .kickChatMember(cfg.GroupID, user.UserID, GetTime.GetUnixTime() + 1800);
                                }).Start();
                            }
                            catch
                            {
                            }
                    Thread.Sleep(500);
                }
            }
        }
    }
}