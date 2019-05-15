using BlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class BanStatus
    {
        internal bool banstatus(TgMessage RawMessage)
        {
            int banstatSpace = RawMessage.text.IndexOf(" ");
            if (banstatSpace == -1)
            {
                string banmsg = "";
                BanUser ban;
                ban = Config.GetDatabaseManager().GetUserBanStatus(RawMessage.GetSendUser().id);
                banmsg = "發送者 : " + RawMessage.GetSendUser().GetUserTextInfoMarkdown() + "\n" + ban.GetBanMessageMarkdown();
                if (Config.GetIsInWhiteList(RawMessage.GetSendUser().id)) 
                    banmsg = banmsg + "，使用者在白名單內";
                if (ban.Ban == 0)
                    banmsg += "\n對於被封鎖的使用者，你可以通過 [點選這裡](https://t.me/" + Config.CourtGroupName + ") 以請求解除。";
                if (RawMessage.reply_to_message != null)
                {
                    ban = Config.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.GetSendUser().id);
                    banmsg += "\n\n被回覆的訊息的原發送使用者 : " +
                              RawMessage.reply_to_message.GetSendUser().GetUserTextInfoMarkdown() + "\n" +
                              ban.GetBanMessageMarkdown();
                    if (Config.GetIsInWhiteList(RawMessage.reply_to_message.GetSendUser().id))
                        banmsg = banmsg + "，使用者在白名單內";
                    if (RawMessage.reply_to_message.forward_from != null)
                    {
                        ban = Config.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.forward_from.id);
                        banmsg += "\n\n被回覆的訊息轉發自使用者 : " +
                                  RawMessage.reply_to_message.forward_from.GetUserTextInfoMarkdown() + "\n" +
                                  ban.GetBanMessageMarkdown();
                        if (Config.GetIsInWhiteList(RawMessage.reply_to_message.forward_from.id))
                            banmsg = banmsg + "，使用者在白名單內";
                    }

                    if (RawMessage.reply_to_message.forward_from_chat != null)
                    {
                        banmsg += "\n\n被回覆的訊息轉發自頻道 : \n" +
                                  RawMessage.reply_to_message.forward_from_chat.GetChatTextInfoMarkdown();
                        if (Config.GetIsInWhiteList(RawMessage.reply_to_message.forward_from_chat.id))
                            banmsg = banmsg + "\n頻道在白名單內";
                    }
                }

                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, banmsg,
                    RawMessage.message_id, TgApi.PARSEMODE_MARKDOWN);
                return true;
            }

            if (int.TryParse(RawMessage.text.Substring(banstatSpace + 1), out int userid))
            {
                BanUser ban = Config.GetDatabaseManager().GetUserBanStatus(userid);
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,
                    "這位使用者\n" + ban.GetBanMessageMarkdown(), RawMessage.message_id, TgApi.PARSEMODE_MARKDOWN);
                return true;
            }

            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id, "您的輸入有錯誤，請輸入正確的 UserID",
                RawMessage.message_id);
            return true;
        }
    }
}