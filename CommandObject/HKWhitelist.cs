using System;
using System.IO;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class HKWhitelist
    {
        internal bool addHKWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/addhk", "").Replace(" ", "");
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /addhk UID",
                    RawMessage.message_id);
                return false;
            }

            if (Config.HKWhiteList.AddToList(Convert.ToInt64(UID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已經在名單內了!", RawMessage.message_id);
                return false;
            }

            return true;
        }

        internal bool deleteHKWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/delhk", "").Replace(" ", "");

            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /delhk UID",
                    RawMessage.message_id);

                return false;
            }
            
            if (Config.HKWhiteList.RemoveFromList(Convert.ToInt64(UID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "刪除成功 !", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "找不到 ChatID !", RawMessage.message_id);
                return false;
            }

            return true;
        }

        internal bool listHKWhitelist(TgMessage RawMessage)
        {
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Config.HKWhiteList.GetListMessage(), RawMessage.message_id);
            return true;
        }
    }
}