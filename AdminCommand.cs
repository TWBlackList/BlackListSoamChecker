using BlackListSoamChecker.CommandObject;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker
{
    internal class AdminCommand
    {
        internal bool AdminCommands(TgMessage RawMessage, string JsonMessage, string Command)
        {
            if (RAPI.getIsBotOP(RawMessage.GetSendUser().id) || RAPI.getIsBotAdmin(RawMessage.GetSendUser().id) || RawMessage.GetMessageChatInfo().id == Config.     InternGroupID)
            {
                switch (Command)
                {
                    case "/groups":
                        if(Config.EnableGetAllGroup) new AllGroups().Groups_Status(RawMessage);
                        throw new StopProcessException();    
                }

                if (!Config.DisableBanList)
                {
                    
                    if (Command == "/" + Config.CustomPrefix + "ban")
                    {
                        if(Config.EnableCustomBan) new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                        throw new StopProcessException();
                    }
                    if (Command == "/" + Config.CustomPrefix + "unban")
                    {
                        if(Config.EnableCustomUnBan) new UnbanUserCommand().Unban(RawMessage);
                        throw new StopProcessException();
                    }
                    switch (Command)
                    {
                        case "/groupadmin":
                            if(Config.EnableGetGroupAdmin)  new GetAdmins().GetGroupAdmins(RawMessage);
                            throw new StopProcessException();
                        case "/getspampoints":
                            if(Config.EnableGetSpamStringPoints) new SpamStringManager().GetSpamPoints(RawMessage);
                            throw new StopProcessException();
                        case "/ban":
                            if(Config.EnableBan) new BanUserCommand().Ban(RawMessage, JsonMessage, Command);
                            throw new StopProcessException();
                        case "/unban":
                            if(Config.EnableUnBan) new UnbanUserCommand().Unban(RawMessage);
                            throw new StopProcessException();
                    }
                }

                if (RAPI.getIsBotAdmin(RawMessage.GetSendUser().id))
                {
                    if (!Config.DisableBanList)
                        switch (Command)
                        {
                            case "/getallspamstr":
                                if(Config.EnableGetAllSpamStringInfo) new SpamStringManager().GetAllInfo(RawMessage);
                                return true;
                            case "/addspamstr":
                                if(Config.EnableAddSpamString) new SpamStringManager().Add(RawMessage);
                                throw new StopProcessException();
                            case "/delspamstr":
                                if(Config.EnableDeleteSpamString) new SpamStringManager().Remove(RawMessage);
                                throw new StopProcessException();
                            case "/suban":
                                if(Config.EnableSuperBan) new BanMultiUserCommand().BanMulti(RawMessage, JsonMessage, Command);
                                throw new StopProcessException();
                            case "/suunban":
                                if(Config.EnableSuperUnBan) new UnBanMultiUserCommand().UnbanMulti(RawMessage);
                                throw new StopProcessException();
                            case "/getspamstr":
                                if(Config.EnableGetSpamString) new SpamStringManager().GetName(RawMessage);
                                throw new StopProcessException();
                            case "/reloadspamstr":
                                if(Config.EnableReloadSpamString) new SpamStringManager().reloadSpamList(RawMessage);
                                throw new StopProcessException();
                        }
                    switch (Command)
                    {
                        case "/points":
                            if(Config.EnableGetSpamStringKeywords) new SpamStringManager().GetSpamKeywords(RawMessage);
                            throw new StopProcessException();
                        case "/cleanup":
                            if(Config.EnableCleanUp) new CleanUP().CleanUP_Status(RawMessage);
                            throw new StopProcessException();
                        case "/say":
                            if(Config.EnableBroadcast) new BroadCast().BroadCast_Status(RawMessage);
                            throw new StopProcessException();
                        case "/sdall":
                            if(Config.EnableDisableAllGroupSoam) new OP().SDAll(RawMessage);
                            throw new StopProcessException();
                        case "/seall":
                            if(Config.EnableEnableAllGroupSoam) new OP().SEAll(RawMessage);
                            throw new StopProcessException();
                        case "/addop":
                            if(Config.EnableAddOP) new OP().addOP(RawMessage);
                            throw new StopProcessException();
                        case "/delop":
                            if(Config.EnableDeleteOP) new OP().delOP(RawMessage);
                            throw new StopProcessException();
                        case "/addwl":
                            if(Config.EnableWhitelistAdd) new Whitelist().addWhitelist(RawMessage);
                            throw new StopProcessException();
                        case "/delwl":
                            if(Config.EnableWhitelistDelete) new Whitelist().deleteWhitelist(RawMessage);
                            throw new StopProcessException();
                        case "/lswl":
                            if(Config.EnableWhitelisList) new Whitelist().listWhitelist(RawMessage);
                            throw new StopProcessException();
                        case "/block":
                            if(Config.EnableBlockListAdd) new BlockGroup().addBlockGroup(RawMessage);
                            throw new StopProcessException();
                        case "/unblock":
                            if(Config.EnableBlockListDelete) new BlockGroup().deleteBlockGroup(RawMessage);
                            throw new StopProcessException();
                        case "/blocks":
                            if(Config.EnableBlockListList) new BlockGroup().listBlockGroup(RawMessage);
                            throw new StopProcessException();
                    }
                }
            }

            return false;
        }
    }
}