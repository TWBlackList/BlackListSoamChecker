using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using BlackListSoamChecker.DbManager;
using BlackListSoamChecker.CommandObject;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.Interfaces;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker
{
    internal class MemberJoinReceiver : IMemberJoinLeftListener
    {
        public CallbackMessage OnGroupMemberJoinReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            return OnSupergroupMemberJoinReceive(RawMessage, JsonMessage, JoinedUser);
        }

        public CallbackMessage OnSupergroupMemberJoinReceive(TgMessage RawMessage, string JsonMessage,
            UserInfo JoinedUser)
        {
            DatabaseManager dbmgr = Config.GetDatabaseManager();
            GroupCfg groupCfg = dbmgr.GetGroupConfig(RawMessage.GetMessageChatInfo().id);

            if (groupCfg.AntiBot == 0 && JoinedUser.is_bot && !TgApi.getDefaultApiConnection()
                    .checkIsAdmin(RawMessage.GetMessageChatInfo().id, RawMessage.from.id))
            {
                SetActionResult result = TgApi.getDefaultApiConnection()
                    .kickChatMember(RawMessage.GetMessageChatInfo().id, JoinedUser.id, GetTime.GetUnixTime() + 300);
                if (result.ok)
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "機器人 : " + JoinedUser.GetUserTextInfoMarkdown() + "\n由於開啟了 AntiBot ，已自動移除機器人。",
                        ParseMode: TgApi.PARSEMODE_MARKDOWN
                    );
                else
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "機器人 : " + JoinedUser.GetUserTextInfoMarkdown() + "\n由於開啟了 AntiBot ，但沒有 (Ban User) 權限，請設定正確的權限。",
                        ParseMode: TgApi.PARSEMODE_MARKDOWN
                    );

                new Task(() =>
                {
                    long banUtilTime = GetTime.GetUnixTime() + 86400;
                    Config.GetDatabaseManager().BanUser(
                        0,
                        RawMessage.GetSendUser().id,
                        0,
                        banUtilTime,
                        RAPI.escapeMarkdown("自動封鎖 - 拉入機器人 ") + JoinedUser.GetUserTextInfoMarkdown(),
                        RawMessage.GetMessageChatInfo().id,
                        0,
                        RawMessage.GetSendUser()
                    );
                }).Start();
            }

            if (JoinedUser.id == TgApi.getDefaultApiConnection().getMe().id)
            {
                if (Config.EnableOnlyJoinGroupInviteByAdmin)
                {
                    if (!RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                    {
                        Thread.Sleep(500);
                        TgApi.getDefaultApiConnection().leaveChat(RawMessage.GetMessageChatInfo().id);
                        return new CallbackMessage();
                    }
                }
                if (Config.BlockGroups.CheckInList(RawMessage.GetMessageChatInfo().id))
                {
                    new Thread(delegate()
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "此群組禁止使用本服務。");
                        Thread.Sleep(1000);
                        TgApi.getDefaultApiConnection().leaveChat(RawMessage.GetMessageChatInfo().id);
                    }).Start();
                    return new CallbackMessage();
                }

                if (Config.EnableAutoLeaveNormalGroup)
                {
                    if (RawMessage.GetMessageChatInfo().type == "group")
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,
                            "一般群組無法使用本服務，如有疑問請至 @" + Config.CourtGroupName + "詢問");
                        Thread.Sleep(2000);
                        TgApi.getDefaultApiConnection().leaveChat(RawMessage.GetMessageChatInfo().id);
                        return new CallbackMessage();
                    }

                }

                if (!new CheckHelper().CheckAdminInReportGroup(RawMessage.GetMessageChatInfo().id))
                {
                    new Thread(delegate()
                    {
                        TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, 
                            "群管理必須加入[項目群組](https://t.me/" + Config.AdminContactGroupName + ")才可使用本服務。",ParseMode: TgApi.PARSEMODE_MARKDOWN);
                        Thread.Sleep(2000);
                        TgApi.getDefaultApiConnection().leaveChat(RawMessage.GetMessageChatInfo().id);
                    }).Start();
                    return new CallbackMessage();
                
                }

                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    "歡迎使用 @" + TgApi.getDefaultApiConnection().getMe().username + "\n" +
                    "1.請在群組中给予 @" + TgApi.getDefaultApiConnection().getMe().username + " 管理員權限\n" +
                    "2.使用 /help 可查閱使用說明\n" +
                    "預設開啟的功能有 BlackList AutoKick AntiHalal SubscribeBanList，可以根據需要來調整。\n\n" +
                    "注意 : 加入機器人即同意讓渡部分 Ban Users 權限予本項目組，並授權本組依據 @" + Config.ReportGroupName + " 置頂規定，代表群管理對群組內成員逕行封鎖\n" +
                    "如不同意請立即移除此機器人，且禁止違背群主意願私自添加",
                    RawMessage.message_id
                );
                return new CallbackMessage();
            }

            if (Config.DisableBanList) return new CallbackMessage();

            if (Config.CourtGroupName != null && RawMessage.GetMessageChatInfo().username == Config.CourtGroupName)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                if (banUser.Ban == 0)
                {
                    string resultmsg = "這位使用者被封鎖了\n請先閱讀置頂及公告區\n未依規定發表的任何訊息皆不回應\n\n" + banUser.GetBanMessageMarkdown() ;
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        resultmsg,
                        RawMessage.message_id,
                        TgApi.PARSEMODE_MARKDOWN
                    );
                }
                else
                {
                    if (!Config.EnableAutoKickNotBanUserinCourtGroup) return new CallbackMessage();
                    if (Config.GetIsInWhiteList(JoinedUser.id)) return new CallbackMessage();
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        "您未被封鎖，請離開，本群僅提供被 CNBL 封鎖者申訴",
                        RawMessage.message_id,
                        TgApi.PARSEMODE_MARKDOWN
                    );

                    TgApi.getDefaultApiConnection().restrictChatMember(
                        RawMessage.GetMessageChatInfo().id,
                        JoinedUser.id,
                        0, false, false, false, false);
                    new Thread(delegate()
                    {
                        Thread.Sleep(30000);
                        try
                        {
                            TgApi.getDefaultApiConnection().kickChatMember(
                                RawMessage.GetMessageChatInfo().id,
                                JoinedUser.id,
                                GetTime.GetUnixTime() + 300
                            );
                            TgApi.getDefaultApiConnection().restrictChatMember(
                                RawMessage.GetMessageChatInfo().id,
                                JoinedUser.id,
                                0, true, false, false, false);
                        }
                        catch
                        {
                        }
                    }).Start();
                }

                return new CallbackMessage();
            }

            if (groupCfg.BlackList == 0)
            {
                BanUser banUser = dbmgr.GetUserBanStatus(JoinedUser.id);
                string resultmsg = "";
                if (banUser.Ban == 0)
                {
                    string banReason;
                    if (banUser.ChannelMessageID != 0 && Config.MainChannelName != null)
                        banReason = "[原因請點選這裡查看](https://t.me/" + Config.MainChannelName + "/" +
                                    banUser.ChannelMessageID + ")\n";
                    else
                        banReason = "\n原因 : " + RAPI.escapeMarkdown(banUser.Reason) + "\n";
                    if (banUser.Level == 0)
                    {
                        resultmsg += "警告 : 這個使用者「將會」對群組造成負面影響\n" +
                                     banReason +
                                     "\n若有誤判，可以到 [這個群組](https://t.me/" + Config.CourtGroupName + ") 尋求申訴";
                        if (groupCfg.AutoKick == 0)
                            try
                            {
                                SetActionResult result = TgApi.getDefaultApiConnection().kickChatMember(
                                    RawMessage.GetMessageChatInfo().id,
                                    JoinedUser.id,
                                    GetTime.GetUnixTime() + 300
                                );
                                if (!result.ok)
                                    resultmsg += "\n注意 : 由於開啟了 AutoKick 但沒有 Ban Users 權限" +
                                                 "，請關閉此功能或給予權限（Ban users）。";
                            }
                            catch
                            {
                            }
                    }
                    else if (banUser.Level == 1)
                    {
                        resultmsg += "警告 : 這個使用者「可能」對群組造成負面影響" + banReason + "\n" +
                                     "請群組管理員多加留意\n" +
                                     "對於被警告的使用者，你可以通過 [這個群組](https://t.me/" + Config.CourtGroupName + ") 以請求解除。";
                    }
                }
                else
                {
                    if (Config.DisableBanList == false && groupCfg.AutoDeleteSpamMessage == 0)
                    {
                        int max_point = 0;
                        SpamMessage max_point_spam = new SpamMessage();
                        List<SpamMessage> spamMsgList = dbmgr.GetSpamMessageList();
                        foreach (SpamMessage smsg in spamMsgList)
                        {
                            int points = 0;
                            switch (smsg.Type)
                            {
                                case 8:
                                    points = new SpamMessageChecker().GetNamePoints(smsg.Messages,
                                        RawMessage.from.full_name());
                                    break;
                            }
    
                            if (points >= smsg.MinPoints)
                                if (points > max_point)
                                {
                                    max_point = points;
                                    max_point_spam = smsg;
                                }
                        }
        
                        if (max_point > 0)
                            {
                            //Send alert and delete alert after 60 second
                                new Thread(delegate()
                                {
                                    string msg = "";
                                    if (Config.ReportGroupName == Config.CourtGroupName)
                                        msg = "偵測到 " + max_point_spam.FriendlyName +
                                          " ，已自動回報，如有誤封請聯繫 @" + Config.ReportGroupName + " 提出申訴。";
                                    else
                                        msg = "偵測到 " + max_point_spam.FriendlyName +
                                          " ，已自動回報，如有誤報請加入 @" + Config.ReportGroupName + " 以報告誤報" +
                                          " ，如有疑慮請加入 @" + Config.CourtGroupName + " 提出申訴。";
                                    SendMessageResult autodeletespammessagesendresult = TgApi.getDefaultApiConnection()
                                        .sendMessage(
                                            RawMessage.GetMessageChatInfo().id,
                                            msg
                                        );

                                    ProcessMessage(max_point_spam, RawMessage.message_id, RawMessage.GetMessageChatInfo().id,
                                            RawMessage.GetSendUser(), max_point);
                                    Thread.Sleep(30000);
                                    TgApi.getDefaultApiConnection().deleteMessage(
                                        autodeletespammessagesendresult.result.chat.id,
                                        autodeletespammessagesendresult.result.message_id
                                    );
                                }).Start();
                            return new CallbackMessage {StopProcess = true};
                        }
                        else
                        {
                            return new CallbackMessage();
                        }
                    }
                }

                new Thread(delegate()
                {
                    SendMessageResult autodeletespammessagesendresult = TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        resultmsg,
                        RawMessage.message_id,
                        TgApi.PARSEMODE_MARKDOWN
                    );
                    Thread.Sleep(60000);
                    TgApi.getDefaultApiConnection().deleteMessage(
                        autodeletespammessagesendresult.result.chat.id,
                        autodeletespammessagesendresult.result.message_id
                    );
                    TgApi.getDefaultApiConnection().deleteMessage(
                        RawMessage.GetMessageChatInfo().id,
                        RawMessage.message_id
                    );
                }).Start();

                return new CallbackMessage {StopProcess = true};
            }
            
            return new CallbackMessage();
        }
        
        private void ProcessMessage(SpamMessage smsg, int MsgID, long ChatID, UserInfo SendUserInfo, int point)
        {
            long banUtilTime;
            if (smsg.BanDays == 0 && smsg.BanHours == 0 && smsg.BanMinutes == 0)
                banUtilTime = 0;
            else
                banUtilTime = GetTime.GetUnixTime() + smsg.BanDays * 86400 + smsg.BanHours * 3600 +
                              smsg.BanMinutes * 60;

            if (smsg.AutoKick)
                new Thread(delegate()
                {
                    //TgApi.getDefaultApiConnection().restrictChatMember(
                    //    ChatID,
                    //    SendUserInfo.id,
                    //    GetTime.GetUnixTime() + 60,
                    //    false);
                    Thread.Sleep(5500);
                    TgApi.getDefaultApiConnection()
                        .kickChatMember(ChatID, SendUserInfo.id, GetTime.GetUnixTime() + 1800);
                }).Start();
            if (smsg.AutoBlackList)
            {
                new Thread(delegate()
                {
                    if (Config.GetDatabaseManager().GetUserBanStatus(SendUserInfo.id).Ban == 0) return;
                    new Task(() =>
                    {
                        Config.GetDatabaseManager().BanUser(
                            0,
                            SendUserInfo.id,
                            smsg.BanLevel,
                            banUtilTime,
                            "`" + RAPI.escapeMarkdown(smsg.FriendlyName) + "`\n分數 : `" + point + "`",
                            ChatID,
                            MsgID,
                            SendUserInfo
                        );
                    }).Start();
                }).Start();
            }
            else
            {
                if (smsg.AutoMute)
                    TgApi.getDefaultApiConnection().restrictChatMember(
                        ChatID,
                        SendUserInfo.id,
                        banUtilTime,
                        true,
                        false
                    );
            }

            if (smsg.AutoDelete)
                new Thread(delegate()
                {
                    Thread.Sleep(10000);
                    TgApi.getDefaultApiConnection().deleteMessage(ChatID, MsgID);
                }).Start();
        }

        public CallbackMessage OnGroupMemberLeftReceive(TgMessage RawMessage, string JsonMessage, UserInfo JoinedUser)
        {
            return new CallbackMessage();
        }

        public CallbackMessage OnSupergroupMemberLeftReceive(TgMessage RawMessage, string JsonMessage,
            UserInfo JoinedUser)
        {
            return new CallbackMessage();
        }
    }
}