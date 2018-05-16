using System.Collections.Generic;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class BanUserCommand
    {
        internal bool Ban(TgMessage RawMessage, string JsonMessage, string Command)
        {
            int banSpace = RawMessage.text.IndexOf(" ");
            if (banSpace == -1)
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.BAN_HELP_MESSAGE,
                    RawMessage.message_id
                );
                return true;
            }

            int BanUserId = 0;
            long ExpiresTime = 0;
            int Level = 0;
            string Reason = "";
            UserInfo BanUserInfo = null;
            string value = RawMessage.text.Substring(banSpace + 1);
            int valLen = value.Length;
            bool NotHalal = true;
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
                                Strings.BAN_ERROR_MESSAGE + " err_a1",
                                RawMessage.message_id
                            );
                            return true;
                        }

                        UserInfo tmpUinfo =
                            new GetValues().GetByTgMessage(
                                new Dictionary<string, string> {{"from", value.Substring(6)}}, RawMessage);
                        if (tmpUinfo == null) return true; // 如果没拿到使用者信息则代表出现了异常

                        BanUserId = tmpUinfo.id;
                        if (tmpUinfo.language_code != null && tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__")
                            BanUserInfo = tmpUinfo;
                    }
                    else
                    {
                        UserInfo tmpUinfo =
                            new GetValues().GetByTgMessage(new Dictionary<string, string>(), RawMessage);
                        if (tmpUinfo == null) return true; // 如果没拿到使用者信息则代表出现了异常

                        BanUserId = tmpUinfo.id;
                        if (tmpUinfo.language_code != null)
                        {
                            if (tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__") BanUserInfo = tmpUinfo;
                        }
                        else
                        {
                            BanUserInfo = tmpUinfo;
                        }
                    }
                }

            if (NotHalal)
                try
                {
                    Dictionary<string, string> banValues = CommandDecoder.cutKeyIsValue(value);
                    string tmpString = "";

                    // 获取使用者信息
                    UserInfo tmpUinfo = new GetValues().GetByTgMessage(banValues, RawMessage);
                    if (tmpUinfo == null) return true; // 如果没拿到使用者信息则代表出现了异常

                    BanUserId = tmpUinfo.id;
                    if (tmpUinfo.language_code != null)
                    {
                        if (tmpUinfo.language_code != "__CAN_NOT_GET_USERINFO__") BanUserInfo = tmpUinfo;
                    }
                    else
                    {
                        BanUserInfo = tmpUinfo;
                    }

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
                            Strings.BAN_ERROR_MESSAGE + " err8",
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
                        Strings.BAN_ERROR_MESSAGE + " err10",
                        RawMessage.message_id
                    );
                    return true;
                }

            if (RAPI.getIsInWhitelist(BanUserId))
            {
                TgApi.getDefaultApiConnection().sendMessage(
                    RawMessage.GetMessageChatInfo().id,
                    Strings.EXEC_FAIL + Strings.BAN_ERROR_USER_IN_WHITELIST,
                    RawMessage.message_id
                );
                return false;
            }

            bool status;            
            if (RawMessage.GetMessageChatInfo().id == Config.InternGroupID && RawMessage.GetReplyMessage().GetSendUser().id == TgApi.getDefaultApiConnection().getMe().id)
                status = Config.GetDatabaseManager().BanUser(
                    1,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason,
                    RawMessage.GetMessageChatInfo().id,
                    RawMessage.GetReplyMessage().message_id,
                    BanUserInfo
                );
            else if (BanUserInfo == null)
                status = Config.GetDatabaseManager().BanUser(
                    RawMessage.GetSendUser().id,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason
                );
            else if (RawMessage.GetReplyMessage().new_chat_member != null)
                status = Config.GetDatabaseManager().BanUser(
                    RawMessage.GetSendUser().id,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason,
                    0,
                    0,
                    BanUserInfo
                );
            else
                status = Config.GetDatabaseManager().BanUser(
                    RawMessage.GetSendUser().id,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason,
                    RawMessage.GetMessageChatInfo().id,
                    RawMessage.GetReplyMessage().message_id,
                    BanUserInfo
                );
            //if (status)
            //{
            TgApi.getDefaultApiConnection().sendMessage(
                RawMessage.GetMessageChatInfo().id,
                Strings.EXEC_OK,
                RawMessage.message_id
            );
            return true;
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
        }
    }
}