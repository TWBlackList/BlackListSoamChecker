using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReimuAPI.ReimuBase;

namespace BlackListSoamChecker.DbManager
{
    internal class BlacklistDatabaseContext : DbContext
    {
        public DbSet<BanUser> BanUsers { get; set; }
        public DbSet<BanHistory> BanHistorys { get; set; }
        public DbSet<GroupCfg> GroupConfig { get; set; }
        public DbSet<IDList> IDList { get; set; }
        public DbSet<UnbanRequest> UnbanRequests { get; set; }
        public DbSet<UnbanRequestCount> UnbanRequestCount { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + ConfigManager.GetConfigPath() + "soamchecker.db");
        }
    }

    public class BanUser
    {
        [Key] public int UserID { get; set; }

        public int Ban { get; set; } = 1;
        public int Level { get; set; } = 0;
        public int ChannelMessageID { get; set; } = 0;
        public int ReasonMessageID { get; set; } = 0;
        public int HistoryID { get; set; } = 0;
        public long Expires { get; set; } = 0;
        public string Reason { get; set; }

        public string GetBanMessage()
        {
            string msg = "未封鎖";
            if (Ban == 0)
            {
                string ExpTime = GetTime.GetExpiresTime(Expires);
                msg = "處分 : ";
                if (Level == 0)
                    msg += "封鎖";
                else if (Level == 1)
                    msg += "警告";
                else
                    msg += " : " + Level + " (未知)";

                if (ExpTime != "永久封鎖")
                    msg += "\n時效至 : " + GetTime.GetExpiresTime(Expires);
                else
                    msg += "\n時效 : 永久";

                msg += "\n原因 : " + Reason;

                if (ChannelMessageID != 0 && Config.MainChannelName != null)
                    msg += "\n\n參考 : https://t.me/" + Config.MainChannelName + "/" + ChannelMessageID;
            }

            return msg;
        }
        
        public string GetBanMessageMarkdown()
        {
            string msg = "未封鎖";
            if (Ban == 0)
            {
                string ExpTime = GetTime.GetExpiresTime(Expires);
                msg = "處分 : `";
                if (Level == 0)
                    msg += "封鎖";
                else if (Level == 1)
                    msg += "警告";
                else
                    msg += " : " + Level + " (未知)";
                
                msg += "`";

                if (ExpTime != "永久封鎖")
                    msg += "\n時效至 : `" + GetTime.GetExpiresTime(Expires)+"`";
                else
                    msg += "\n時效 : `永久`";

                if (Reason.Contains("`"))
                    msg += "\n原因 : " + Reason;
                else
                    msg += "\n原因 : " + RAPI.escapeMarkdown(Reason);

                if (ChannelMessageID != 0 && Config.MainChannelName != null)
                    msg += "\n\n參考 : https://t.me/" + RAPI.escapeMarkdown(Config.MainChannelName) + "/" + ChannelMessageID;

            }

            return msg;
        }
    }

    public class BanHistory
    {
        [Key] public int ID { get; set; }

        public int UserID { get; set; }
        public int Ban { get; set; } = 0;
        public int Level { get; set; } = 0;
        public int ChannelMessageID { get; set; } = 0;
        public int ReasonMessageID { get; set; } = 0;
        public int AdminID { get; set; } = 0;
        public long Expires { get; set; } = 0;
        public long BanTime { get; set; } = 0;
        public string Reason { get; set; }
    }

    public class UnbanRequest
    {
        [Key] public int ID { get; set; }

        public int UserID { get; set; }
        public int Pass { get; set; }
        public long UserReplyTime { get; set; }
        public long AdminReplyTime { get; set; }
        public string UserReplyText { get; set; }
        public string AdminReplyText { get; set; }
    }

    public class GroupCfg
    {
        [Key] public long GroupID { get; set; }

        public int AdminOnly { get; set; }
        public int BlackList { get; set; }
        public int AutoKick { get; set; }
        public int AntiBot { get; set; }
        public int AntiHalal { get; set; }
        public int AutoDeleteCommand { get; set; }
        public int AutoDeleteSpamMessage { get; set; }
        public int SubscribeBanList { get; set; }
    }

    public class UnbanRequestCount
    {
        [Key] public int UserID { get; set; }

        public int RequestCount { get; set; }
        public int RequestLock { get; set; }
    }
    
    public class IDList
    {
        [Key] public string Name { get; set; }

        public string Data { get; set; }

        public string GetListMessage()
        {
            return Name + " : \n\n" + string.Join("\n",Data.Split(","));
        }        
	
		public string GetListMessage_MD()
        {
            return Name + " : \n\n`" + string.Join("`\n`",Data.Split(","))+"`";
        }
        
        public List<long> GetList()
        {
            List<long> tmpList = new List<long>();
            if(Data == "")
                return tmpList;
            foreach (var i in Data.Split(","))
            {
                tmpList.Add(Convert.ToInt64(i));
            }
            return tmpList;
        }
        
        public long[] GetArray()
        {
            return GetList().ToArray();
        }
        
        public void Save(long[] list)
        {
            long[] tmpList = list;
            Config.GetDatabaseManager().ChangeDbIDList(Name, string.Join(",", tmpList));
            Data = string.Join(",", tmpList);
        }
        
        public void Save(List<long> list)
        {
            long[] tmpList = list.ToArray();
            Config.GetDatabaseManager().ChangeDbIDList(Name, string.Join(",", tmpList));
            Data = string.Join(",", tmpList);
        }
        
        public bool AddToList(int id)
        {            
            long tempID = Convert.ToInt64(id);

            if (CheckInList(id))
            {
                return false;
            }
            else
            {
                List<long> tmpArray = GetList();
                tmpArray.Add(id);
                Save(tmpArray);
                return true;
            }
        }
        
        public bool AddToList(long id)
        {            
            long tempID = id;

            if (CheckInList(id))
            {
                return false;
            }
            else
            {
                List<long> tmpArray = GetList();
                tmpArray.Add(id);
                Save(tmpArray);
                return true;
            }
        }
        
        public bool RemoveFromList(int id)
        {
            long tempID = Convert.ToInt64(id);

            if (!CheckInList(id))
            {
                return false;
            }
            else
            {
                List<long> tmpArray = GetList();
                tmpArray.Remove(id);
                Save(tmpArray);
                return true;
            }
        }
        
        public bool RemoveFromList(long id)
        {
            long tempID = id;

            if (!CheckInList(id))
            {
                return false;
            }
            else
            {
                List<long> tmpArray = GetList();
                tmpArray.Remove(id);
                Save(tmpArray);
                return true;
            }
        }
        
        public bool CheckInList(int id)
        {
            long tempID = Convert.ToInt64(id);
            
            foreach (long i in GetList())
                if (i == tempID)
                    return true;
            return false;
        }        
        
        public bool CheckInList(long id)
        {
            long tempID = Convert.ToInt64(id);
            
            foreach (long i in GetList())
                if (i == tempID)
                    return true;
            return false;
        }
        
    }
    
}