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
            bool notCommonBan = true;
            if (valLen >= 5)
                if (value.Substring(0, 5) == "halal")
                {
                    notCommonBan = false;
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

            int commandBanLength = 0;
            if (valLen >= 4)
            {
                if (value.Substring(0, 5) == "halal")
                {
                    commandBanLength = 5;
                    Reason = Strings.HALAL;
                    ExpiresTime = Config.DefaultHalalBanDay * 86400;
                    notCommonBan = false;
                }
                if (value.Substring(0, 4) == "spam")
                {
                    commandBanLength = 4;
                    Reason = Strings.SPAM;
                    ExpiresTime = Config.DefaultSpamBanDay * 86400;
                    notCommonBan = false;
                }
                if (value.Substring(0, 7) == "spammer")
                {
                    commandBanLength = 7;
                    Reason = Strings.SPAMMER;
                    ExpiresTime = Config.DefaultSpammerBanDay * 86400;
                    notCommonBan = false;
                }
                if (value.Substring(0, 6) == "innsfw")
                {
                    commandBanLength = 6;
                    Reason = Strings.INNSFW;
                    ExpiresTime = Config.DefaultInNsfwBanDay * 86400;
                    notCommonBan = false;
                }
                if (value.Substring(0, 7) == "outnsfw")
                {
                    commandBanLength = 7;
                    Reason = Strings.OUTNSFW;
                    ExpiresTime = Config.DefaultOutNsfwBanDay * 86400;
                    notCommonBan = false;
                }
                if (value.Substring(0, 4) == "coin")
                {
                    commandBanLength = 4;
                    Reason = Strings.COIN;
                    ExpiresTime = Config.DefaultCoinBanDay * 86400;
                    notCommonBan = false;
                }
            }

            if (!notCommonBan)
            {
                if (valLen > (commandBanLength + 1))
                {
                    if (value[commandBanLength] != ' ')
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
            
            
                

            if (notCommonBan)
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

            int AdminID = RawMessage.GetSendUser().id;
            if (RawMessage.GetMessageChatInfo().id == Config.InternGroupID)
                AdminID = 1;
                    
            bool status;            
            if (BanUserInfo == null)
                status = Config.GetDatabaseManager().BanUser(
                    AdminID,
                    BanUserId,
                    Level,
                    ExpiresTime,
                    Reason
                );
            else if (RawMessage.GetReplyMessage().new_chat_member != null)
                status = Config.GetDatabaseManager().BanUser(
                    AdminID,
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
                    AdminID,
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