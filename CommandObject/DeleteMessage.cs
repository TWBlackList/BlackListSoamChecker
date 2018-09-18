using BlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class DeleteMessage
    {
        internal bool DeleteMessageCommand(TgMessage RawMessage)
        {
            if (TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.from.id) ||
                RAPI.getIsBotOP(RawMessage.GetSendUser().id) || RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
            {
                TgApi.getDefaultApiConnection().deleteMessage(RawMessage.GetReplyMessage().chat.id, RawMessage.GetReplyMessage().message_id);
                return true;
            }
            return true;
        }
    }
}