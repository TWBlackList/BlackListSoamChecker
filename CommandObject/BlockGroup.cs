using System;
using System.IO;
using Newtonsoft.Json;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.CommandObject
{
    internal class BlockGroup
    {
        internal bool addBlockGroup(TgMessage RawMessage)
        {
            string ChatID_Value = RawMessage.text.Replace("/block", "").Replace(" ", "");
            if (ChatID_Value.Length < 10)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /block ChatID",
                    RawMessage.message_id);
                return false;
            }

            if (ChatID_Value.Length == 10 && Convert.ToInt64(ChatID_Value) > 0) ChatID_Value = "-100" + ChatID_Value;

            if (Config.BlockGroups.AddToList(Convert.ToInt64(ChatID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "新增成功!", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "已經在名單內了!", RawMessage.message_id);
                return false;
            }
            
            try
            {
                TgApi.getDefaultApiConnection().sendMessage(Convert.ToInt64(ChatID_Value), "此群組禁止使用本服務。");
                TgApi.getDefaultApiConnection().leaveChat(Convert.ToInt64(ChatID_Value));
            }
            catch
            {
            }

            return true;
        }

        internal bool deleteBlockGroup(TgMessage RawMessage)
        {
            string ChatID_Value = RawMessage.text.Replace("/unblock", "").Replace(" ", "");

            if (ChatID_Value.Length < 10)
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "使用方法 : /block ChatID",
                    RawMessage.message_id);
                return false;
            }

            if (ChatID_Value.Length == 10 && Convert.ToInt64(ChatID_Value) > 0) ChatID_Value = "-100" + ChatID_Value;

            if (Config.BlockGroups.RemoveFromList(Convert.ToInt64(ChatID_Value)))
            {
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "刪除成功 !", RawMessage.message_id);
            }
            else{
                TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id, "找不到 ChatID !", RawMessage.message_id);
                return false;
            }

            return true;
        }

        internal bool listBlockGroup(TgMessage RawMessage)
        {
            TgApi.getDefaultApiConnection().sendMessage(RawMessage.chat.id,Config.BlockGroups.GetListMessage(), RawMessage.message_id);
            return true;
        }
    }
}