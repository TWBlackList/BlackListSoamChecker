using System;
using System.IO;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class Whitelist
    {
        internal bool addWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/addwl", "").Replace(" ", "");
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /addwl UID",
                    RawMessage.message_id);
                return false;
            }

            if (UID_Value.Length == 10 && Convert.ToInt64(UID_Value) > 0) UID_Value = "-100" + UID_Value;

            if (Config.WhiteList.AddToList(Convert.ToInt64(UID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已經在名單內了!", RawMessage.message_id);
                return false;
            }

            return true;
        }

        internal bool deleteWhitelist(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/delwl", "").Replace(" ", "");
            ;
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /delwl UID",
                    RawMessage.message_id);

                return false;
            }

            if (UID_Value.Length == 10 && Convert.ToInt64(UID_Value) > 0) UID_Value = "-100" + UID_Value;

            if (Config.WhiteList.RemoveFromList(Convert.ToInt64(UID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "刪除成功 !", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "找不到 ChatID !", RawMessage.message_id);
                return false;
            }
            
            return true;
        }

        internal bool listWhitelist(TgMessage RawMessage)
        {
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Config.WhiteList.GetListMessage(), RawMessage.message_id);
            return true;
        }
    }
}