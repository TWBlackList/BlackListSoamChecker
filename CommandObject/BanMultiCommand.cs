using System.Collections.Generic;
using System.Threading;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class BanMultiUserCommand
    {
        internal bool BanMulti(TgMessage RawMessage, string JsonMessage, string Command)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.SUPERBAN_HELP_MESSAGE,
                    RawMessage.message_id
                );
                return true;
            }

            int BanUserId = 0;
            int[] UsersArray = { };
            long ExpiresTime = 0;
            int Level = 0;
            string Reason = "";
            string value = RawMessage.text.Substring(banSpace + 1);
            int valLen = value.Length;
            bool NotHalal = true;
            bool status = false;

            if (valLen >= 5)
                if (value.Substring(0, 5) == "halal")
                {
                    NotHalal = false;
                    Reason = Strings.HALAL;
                    if (valLen > 6)
                    {
                        if (value[5] != ' ')
                        {
                            TgApi.getDefaultApiConnection().sendMessage(
                                RawMessage.GetMessageChatInfo().id,
                                Strings.SUPERBAN_ERROR_MESSAGE + " err_a1",
                                RawMessage.message_id
                            );
                            return true;
                        }

                        UsersArray =
                            new GetValues().GetUserIDs(new Dictionary<string, string> {{"from", value.Substring(6)}},
                                RawMessage);
                    }
                    else
                    {
                        UsersArray = new GetValues().GetUserIDs(new Dictionary<string, string>(), RawMessage);
                    }
                }

            if (NotHalal)
                try
                {
                    Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(value);
                    string tmpString = "";

                    // 获取使用者
                    UsersArray = new GetValues().GetUserIDs(banValues, RawMessage);

                    // 获取 ExpiresTime
                    long tmpExpiresTime = new GetValues().GetBanUnixTime(banValues, RawMessage);
                    if (tmpExpiresTime == -1) return true; // 如果过期时间是 -1 则代表出现了异常
                    ExpiresTime = tmpExpiresTime;

                    // 获取 Level
                    tmpString = banValues.GetValueOrDefault("l", "__invalid__");
                    if (tmpString == "__invalid__") tmpString = banValues.GetValueOrDefault("level", "0");
                    if (!int.TryParse(tmpString, out Level))
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetMessageChatInfo().id,
                            Strings.SUPERBAN_ERROR_MESSAGE + " err8",
                            RawMessage.message_id
                        );
                        return true;
                    }

                    // 获取 Reason
                    Reason = new GetValues().GetReason(banValues, RawMessage);
                    if (Reason == null) return true; // 如果 Reason 是 null 则代表出现了异常
                    if (Reason.ToLower() == "halal") Reason = Strings.HALAL;
                }
                catch (DecodeException)
                {
                    TgApi.getDefaultApiConnection().sendMessage(
                        RawMessage.GetMessageChatInfo().id,
                        Strings.SUPERBAN_ERROR_MESSAGE + " err10",
                        RawMessage.message_id
                    );
                    return true;
                }

            new Thread(delegate()
            {
                foreach (int userid in UsersArray)
                {
                    BanUserId = userid;
                    status = Config.GetDatabaseManager().BanUser(
                        RawMessage.GetSendUser().id,
                        BanUserId,
                        Level,
                        ExpiresTime,
                        Reason
                    );
                    if (Config.GetIsInWhiteList(BanUserId))
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.GetSendUser().id,
                            Strings.EXEC_FAIL + Strings.BAN_ERROR_USER_IN_WHITELIST + " UID " + BanUserId,
                            RawMessage.message_id
                        );
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
                //        "操作成功。\n\n請注意 : 轉發使用者訊息到頻道或是發送使用者訊息到頻道失敗，請您手動發送至 @" + Config.MainChannelName + " 。 err11",
                //        RawMessage.message_id
                //        );
                //    return true;
                //}
                //return false;
            }).Start();
            return true;
        }
    }
}