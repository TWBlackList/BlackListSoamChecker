using BlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class GetHistory
    {
        internal bool getHistoryStatus(TgMessage RawMessage)
        {
            if (Config.ReasonChannelID == 0)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "沒有證據頻道。",
                    RawMessage.message_id, TgApi.PARSEMODE_MARKDOWN);
                return true;
            }

            int banstatSpace = RawMessage.text.IndexOf(" ");
            if (banstatSpace == -1)
            {
                if (RawMessage.reply_to_message.forward_from != null)
                {
                    BanUser ban = Config.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.forward_from.id);
                    if (ban.ReasonMessageID != 0)
                        TgApi.getDefaultApiConnection().forwardMessage(RawMessage.GetMessageChatInfo().id, Config.ReasonChannelID, ban.ReasonMessageID);
                    return true;
                }

                if (RawMessage.reply_to_message != null)
                {
                    BanUser ban = Config.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.GetSendUser().id);
                    if (ban.ReasonMessageID != 0)
                        TgApi.getDefaultApiConnection().forwardMessage(RawMessage.GetMessageChatInfo().id, Config.ReasonChannelID, ban.ReasonMessageID);
                    return true;
                }
            }

            if (int.TryParse(RawMessage.text.Substring(banstatSpace + 1), out int userid))
            {
                BanUser ban = Config.GetDatabaseManager().GetUserBanStatus(userid);
                if (ban.ReasonMessageID != 0)
                    TgApi.getDefaultApiConnection().forwardMessage(RawMessage.GetMessageChatInfo().id, Config.ReasonChannelID, ban.ReasonMessageID);
                return true;
            }

            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "您的輸入有錯誤，請回覆或是輸入 UserID",
                RawMessage.message_id);
            return true;
        }
    }
}