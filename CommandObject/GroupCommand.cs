using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class GroupCommand
    {
        internal bool GroupID(TgMessage RawMessage)
        {
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.GetMessageChatInfo().id,
                RawMessage.GetMessageChatInfo().id.ToString(), RawMessage.message_id);
            return true;
        }
    }
}