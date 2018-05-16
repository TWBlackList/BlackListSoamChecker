using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker
{
    public class CheckHelper
    {
        public bool CheckAdminInReportGroup(long ChatID)
        {
            if (Config.AdminContactGroupID != 0)
            {
                foreach (long i in Config.adminInReport)
                    if (i == ChatID)
                        return true;

                foreach (long i in Config.adminChecking)
                    if (i == ChatID)
                        return true;
                
                Config.adminChecking.Add(ChatID);
                
                bool status = false;
                GroupUserInfo[] admins = TgApi.getDefaultApiConnection().getChatAdministrators(ChatID,true);
                System.Console.WriteLine("[checkHelper] Getting Chat Administrator List ChatID : " + ChatID);
                foreach (var admin in admins)
                {
                    if (admin.user.id != TgApi.getDefaultApiConnection().getMe().id)
                    {
                        var result = TgApi.getDefaultApiConnection().getChatMember(Config.AdminContactGroupID , admin.user.id);
                        if (result.ok)
                            if (result.result.status != "left")
                            {
                                status = true;  
                                break;
                            }
                    }
                }

                if(!status)
                    foreach (var admin in admins)
                    {
                        if (admin.user.id != TgApi.getDefaultApiConnection().getMe().id)
                        {
                            SendMessageResult result = TgApi.getDefaultApiConnection().sendMessage(
                                Config.AdminContactGroupID,
                                "[加群測試(不用理會此訊息)](tg://user?id=" + admin.user.id.ToString() + ")",
                                ParseMode: TgApi.PARSEMODE_MARKDOWN);
                            if (result.ok)
                            {
                                TgApi.getDefaultApiConnection().deleteMessage(Config.AdminContactGroupID, result.result.message_id);
                                status = true;
                                break;
                            }
                        }
                    }
                if (status)
                {
                    System.Console.WriteLine("[checkHelper] Admin in contact group GID : " + ChatID.ToString());
                    Config.adminInReport.Add(ChatID);
                }
                else
                    System.Console.WriteLine("[checkHelper] Admin not in contact group GID : " + ChatID.ToString());

                Config.adminChecking.Remove(ChatID);
                
                return status;

            }
            else
            {
                return true;
            }
        }
    }
}