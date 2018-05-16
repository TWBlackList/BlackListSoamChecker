using System;
using System.Collections.Generic;
using System.Threading;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class UnBanMultiUserCommand
    {
        internal bool UnbanMulti(TgMessage RawMessage)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.SUPERUNBAN_HELP_MESSAGE,
                    RawMessage.message_id
                );
                return true;
            }

            int[] UsersArray = { };
            bool status = false;
            int BanUserId = 0;
            string Reason;
            try
            {
                Dictionary<string, string> banValues =
                    CommandDecoder.cutKeyIsValue(RawMessage.text.Substring(banSpace + 1));

                // 获取使用者信息
                UsersArray = new GetValues().GetUserIDs(banValues, RawMessage);

                Reason = new GetValues().GetReason(banValues, RawMessage);
                if (Reason == null) return true; // 如果 Reason 是 null 則代表出现了异常
            }
            catch (DecodeException)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.SUPERUNBAN_ERROR_MESSAGE + " err10",
                    RawMessage.message_id
                );
                return true;
            }


            new Thread(delegate()
            {
                foreach (int userid in UsersArray)
                {
                    BanUserId = userid;
                    try
                    {
                        status = Config.GetDatabaseManager().UnbanUser(
                            RawMessage.GetSendUser().id,
                            BanUserId,
                            Reason
                        );
                    }
                    catch (InvalidOperationException)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            Strings.UNBAN_ERROR_USER_NOT_BANNED + " UID : " + BanUserId,
                            RawMessage.message_id
                        );
                    }

                    Thread.Sleep(3500);
                }

                //if (status)
                //{
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.EXEC_OK,
                    RawMessage.message_id
                );
                //}
                //else
                //{
                //    TgApi.getDefaultApiConnection().sendMessage(
                //        RawMessage.GetMessageChatInfo().id,
                //        "操作成功。\n\n請注意 : 轉發使用者訊息到頻道或是發送使用者訊息到頻道失敗，請您手動發送至  @" + Config.MainChannelName + " 。 err11",
                //        RawMessage.message_id
                //        );
                //    return true;
                //}
            }).Start();
            return true;
        }
    }
}