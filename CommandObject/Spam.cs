using System;
using System.IO;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class Spam
    {
        internal bool offSpam(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/offspam", "").Replace(" ", "");
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /offspam UID",
                    RawMessage.message_id);
                return false;
            }

            if (UID_Value.Length == 10 && Convert.ToInt64(UID_Value) > 0) UID_Value = "-100" + UID_Value;

            if (Config.SpamBlackList.AddToList(Convert.ToInt64(UID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已經在名單內了!", RawMessage.message_id);
                return false;
            }

            return true;
        }

        internal bool onSpam(TgMessage RawMessage)
        {
            string UID_Value = RawMessage.text.Replace("/onspam", "").Replace(" ", "");
            ;
            if (UID_Value.Length < 5)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /onspam UID",
                    RawMessage.message_id);
                return false;
            }

            if (UID_Value.Length == 10 && Convert.ToInt64(UID_Value) > 0) UID_Value = "-100" + UID_Value;

            if (Config.SpamBlackList.RemoveFromList(Convert.ToInt64(UID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "刪除成功 !", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "找不到 ChatID !", RawMessage.message_id);
                return false;
            }
            
            return true;
        }

        internal bool listSpam(TgMessage RawMessage)
        {
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Config.SpamBlackList.GetListMessage(), RawMessage.message_id);
            return true;
        }
    }
}