using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class Help
    {
        internal bool HelpStatus(TgMessage RawMessage)
        {
            string finalHelpMsg;
            string groupHelp = Strings.GROUP_HELP;
            string privateHelp = Strings.PRIVATE_HELP;
            string sharedHelp = Strings.SHARED_HELP;
            switch (RawMessage.chat.type)
            {
                case "group":
                case "supergroup":
                    finalHelpMsg = groupHelp + "\n" + sharedHelp;
                    break;
                case "private":
                    finalHelpMsg = privateHelp + "\n" + sharedHelp;
                    break;
                default:
                    finalHelpMsg = sharedHelp;
                    break;
            }

            if (RAPI.getIsBotOP(RawMessage.from.id) || RAPI.getIsBotAdmin(RawMessage.from.id))
                finalHelpMsg = finalHelpMsg + Strings.OPERATIOR_HELP;
            if (RAPI.getIsBotAdmin(RawMessage.from.id))
                finalHelpMsg = finalHelpMsg + Strings.ADMIN_HELP;
            finalHelpMsg = finalHelpMsg + Strings.HELP_AD;
            TgApi.getDefaultApiConnection()
                .sendMessage(RawMessage.GetMessageChatInfo().id, finalHelpMsg, RawMessage.message_id,ParseMode: TgApi.PARSEMODE_MARKDOWN);
            return true;
        }
    }
}