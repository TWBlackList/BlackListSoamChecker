using System.Collections.Generic;
using BlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase.Interfaces;

namespace BlackListSoamChecker
{
    internal class ItemCleaner : IClearItemsReceiver
    {
        public void ClearItems()
        {
            Config.spamMessageList = null;
            Config.groupConfig = new Dictionary<long, GroupCfg>();
            Config.bannedUsers = new Dictionary<int, BanUser>();
        }
    }
}