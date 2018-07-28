using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlackListSoamChecker.DbManager;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker
{
    internal class IDList
    {
        public static bool GetIsInWhiteList(long ID)
        {
            return true;
        }
    }
}