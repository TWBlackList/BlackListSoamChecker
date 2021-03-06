﻿using System;
using System.Threading;
using BlackListSoamChecker.CommandObject;
using BlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.Interfaces;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker
{
    internal class CommandListener : ICommandReceiver
    {
        public CommandListener()
        {
            new DatabaseManager().checkdb();
        }

        public CallbackMessage OnGroupCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            return OnSupergroupCommandReceive(RawMessage, JsonMessage, Command);
        }

        public CallbackMessage OnPrivateCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            try
            {
                if (SharedCommand(RawMessage, JsonMessage, Command)) return new CallbackMessage();
                return new CallbackMessage();
            }
            catch (StopProcessException)
            {
                return new CallbackMessage {StopProcess = true};
            }
            catch (Exception e)
            {
                RAPI.GetExceptionListener().OnException(e, JsonMessage);
                throw e;
            }
        }

        public CallbackMessage OnSupergroupCommandReceive(TgMessage RawMessage, string JsonMessage, string Command)
        {
            try
            {
                GroupCfg cfg = Config.GetDatabaseManager().GetGroupConfig(RawMessage.chat.id);
                if (cfg.AdminOnly == 0)
                    if (TgApi.getDefaultApiConnection().checkIsAdmin(RawMessage.chat.id, RawMessage.from.id) ||
                        RAPI.getIsBotAdmin(RawMessage.from.id) || RAPI.getIsBotOP(RawMessage.from.id))
                    {
                    }
                    else
                    {
                        if (cfg.AutoDeleteCommand == 0)
                        {
                            new Thread(delegate()
                            {
                                SendMessageResult autodeletecommandsendresult = TgApi.getDefaultApiConnection()
                                    .sendMessage(
                                        RawMessage.GetMessageChatInfo().id,
                                        "請您不要亂玩機器人的指令，有問題請聯絡群組管理員。"
                                    );
                                Thread.Sleep(60000);
                                TgApi.getDefaultApiConnection().deleteMessage(
                                    autodeletecommandsendresult.result.chat.id,
                                    autodeletecommandsendresult.result.message_id
                                );
                            }).Start();
                            TgApi.getDefaultApiConnection().deleteMessage(RawMessage.chat.id, RawMessage.message_id);
                        }

                        return new CallbackMessage {StopProcess = true};
                    }

                if (SharedCommand(RawMessage, JsonMessage, Command)) return new CallbackMessage();
                switch (Command)
                {
                    case "/delmsg":
                        if(Config.EnableDeleteMessage) new DeleteMessage().DeleteMessageCommand(RawMessage);
                        break;
                    case "/gid":
                        if(Config.EnableGroupID) new GroupCommand().GroupID(RawMessage);
                        break;
                    case "/leave":
                        if(Config.EnableLeave) new LeaveCommand().Leave(RawMessage);
                        break;
                    case "/soamenable":
                        if(Config.EnableEnableSoam) new SoamManager().SoamEnable(RawMessage);
                        break;
                    case "/soamdisable":
                        if(Config.EnableDisableSoam) new SoamManager().SoamDisable(RawMessage);
                        break;
                    case "/__get_exception":
                        throw new Exception();
                    case "/soamstat":
                    case "/soamstatus":
                        if(Config.EnableSoamStatus) new SoamManager().SoamStatus(RawMessage);
                        break;
                    //case "/cnkick":
                    //    if (Config.DisableBanList)
                    //    {
                    //        TgApi.getDefaultApiConnection().sendMessage(
                    //            RawMessage.chat.id,
                    //            "非常抱歉，目前版本已禁用封鎖用戶的功能，請聯絡管理員開啟此功能。",
                    //            RawMessage.message_id
                    //            );
                    //        break;
                    //    }
                    //    if (RawMessage.reply_to_message == null)
                    //    {
                    //        TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "請回覆一則訊息", RawMessage.message_id);
                    //        return new CallbackMessage();
                    //    }
                    //    BanUser ban = Config.GetDatabaseManager().GetUserBanStatus(RawMessage.reply_to_message.from.id);
                    //    if (ban.Ban == 0)
                    //    {
                    //        if (ban.Level == 0)
                    //        {
                    //            SetActionResult bkick_result = TgApi.getDefaultApiConnection().kickChatMember(
                    //                RawMessage.chat.id,
                    //                RawMessage.reply_to_message.from.id,
                    //                GetTime.GetUnixTime() + 86400
                    //                );
                    //            if (bkick_result.ok)
                    //            {
                    //                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已移除", RawMessage.message_id);
                    //                return new CallbackMessage();
                    //            }
                    //            else
                    //            {
                    //                TgApi.getDefaultApiConnection().sendMessage(
                    //                    RawMessage.chat.id,
                    //                    "無法移除，可能是機器人沒有適當的管理員權限。",
                    //                    RawMessage.message_id
                    //                    );
                    //                return new CallbackMessage();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            TgApi.getDefaultApiConnection().sendMessage(
                    //                RawMessage.chat.id,
                    //                "無法移除，因為此使用者不在黑名單，請您聯絡群組的管理員處理。" +
                    //                "如果你認為這位使用者將會影響大量群組，您可連絡 @" + Config.MainChannelName + " 提供的群組。",
                    //                RawMessage.message_id
                    //                );
                    //            return new CallbackMessage();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        TgApi.getDefaultApiConnection().sendMessage(
                    //            RawMessage.chat.id,
                    //            "無法移除，因為此使用者不在黑名單，請您聯絡群組的管理員處理。" +
                    //            "如果你認為這位使用者將會影響大量群組，您可連絡 @" + Config.MainChannelName + " 提供的群組。",
                    //            RawMessage.message_id
                    //            );
                    //        return new CallbackMessage();
                    //    }
                }

                return new CallbackMessage();
            }
            catch (StopProcessException)
            {
                return new CallbackMessage {StopProcess = true};
            }
            catch (Exception e)
            {
                RAPI.GetExceptionListener().OnException(e, JsonMessage);
                throw e;
            }
        }

        private bool SharedCommand(TgMessage RawMessage, string JsonMessage, string Command)
        {
            if (Command == "/" + Config.CustomPrefix + "banstat")
            {
                if(Config.EnableCustomBanStat)  
                    return new BanStatus().banstatus(RawMessage);
            }
            switch (Command)
            {
                case "/user":
                    if(Config.EnableUser) {new UserCommand().User(RawMessage);}
                    return true;
                case "/lsop":
                    if (Config.EnableListOP) {new OP().lsOP(RawMessage);}
                    return true;
                case "/help":
                    if (Config.EnableHelp){new Help().HelpStatus(RawMessage);}
                    return true;
                case "/banstat":
                case "/banstatus":
                    if (Config.DisableBanList)
                    {
                        TgApi.getDefaultApiConnection().sendMessage(
                            RawMessage.chat.id,
                            "非常抱歉，目前版本已禁用封鎖用戶的功能，請聯絡管理員開啟此功能。",
                            RawMessage.message_id
                        );
                        break;
                    }

                    if (Config.EnableBanStat) {new BanStatus().banstatus(RawMessage);}
                    return true;
                //case "/clickmetobesb"://垃圾功能，之後拔掉，希望不要爆炸！
                //    TgApi.getDefaultApiConnection().sendMessage(
                //        RawMessage.chat.id,
                //        "Success, now you are SB.",
                //        RawMessage.message_id
                //        );
                //    break;
            }

            return new AdminCommand().AdminCommands(RawMessage, JsonMessage, Command);
        }
    }
}