using System;
using System.Collections.Generic;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class UnbanUserCommand
    {
        internal bool Unban(TgMessage RawMessage)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.UNBAN_HELP_MESSAGE,
                    RawMessage.message_id
                );
                return true;
            }

            int BanUserId = 0;
            string Reason;
            UserInfo BanUserInfo = null;
            try
            {
                Dictionary<string, string> banValues =
                    CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(banSpace + 1));

                // 获取使用者信息
                UserInfo tmpUinfo = new GetValues().GetByTgMessage(banValues, RawMessage);
                if (tmpUinfo == null) return true; // 如果没拿到使用者信息則代表出现了异常

                BanUserId = tmpUinfo.id;
                if (tmpUinfo.language_code != null)
                {
                    if (tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__") BanUserInfo = tmpUinfo;
                }
                else
                {
                    BanUserInfo = tmpUinfo;
                }

                // 获取 Reason
                Reason = new GetValues().GetReason(banValues, RawMessage);
                if (Reason == null) return true; // 如果 Reason 是 null 則代表出现了异常
            }
            catch (DecodeException)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.UNBAN_ERROR_MESSAGE + " err10",
                    RawMessage.message_id
                );
                return true;
            }

            new UnBanCaller().UnBanCallerThread(BanUserId);
            bool status;
            try
            {
                if (BanUserInfo == null)
                    status = Config.GetDatabaseManager().UnbanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Reason
                    );
                else
                    status = Config.GetDatabaseManager().UnbanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Reason,
                        BanUserInfo
                    );
            }
            catch (InvalidOperationException)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.EXEC_FAIL + Strings.UNBAN_ERROR_USER_NOT_BANNED,
                    RawMessage.message_id
                );
                return true;
            }

            if (status)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.EXEC_OK,
                    RawMessage.message_id
                );
                if(RawMessage.GetMessageChatInfo().id == Config.CourtGroupID)
                    TgApi.getDefaultApiConnection()
                        .kickChatMember(RawMessage.GetMessageChatInfo().id, BanUserId, GetTime.GetUnixTime() + 1);
                return true;
            }

            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                Strings.EXEC_FAIL + Strings.UNBAN_ERROR_USER_NOT_BANNED,
                RawMessage.message_id
            );
            return false;
            //    TgApi.getDefaultApiConnection().sendMessage(
            //        RawMessage.GetMessageChatInfo().id,
            //        "操作成功。\n\n請注意 : 轉發使用者訊息到頻道或是發送使用者訊息到頻道失敗，請您手動發送至  @" + Config.MainChannelName + " 。 err11",
            //        RawMessage.message_id
            //        );
            //    return true;
            //return false;
        }

        private UserInfo GetUserInfo(TgMessage RawMessage, string from)
        {
            if (RawMessage.reply_to_message == null) return null;
            if (from == "r" || from == "reply")
                return RawMessage.GetReplyMessage().GetSendUser();
            if (from == "f" || from == "fwd") return RawMessage.GetForwardedFromUser();
            return null;
        }
    }
}