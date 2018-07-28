using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ReimuAPI.ReimuBase;
using ReimuAPI.ReimuBase.TgData;

namespace BlackListSoamChecker.DbManager
{
    public class DatabaseManager
    {
        public void checkdb()
        {
            using (var db = new BlacklistDatabaseContext())
            {
                db.Database.EnsureCreated();
            }
        }

        public bool BanUser(
            int AdminID,
            int UserID,
            int Level,
            long Expires,
            string Reason,
            long ChatID = 0,
            int MessageID = 0,
            UserInfo userinfo = null
        )
        {
            if (RAPI.getIsInWhitelist(UserID)) return false;
            bool finalResult = true;
            string banmsg = "";
            int ReasonID = 0;
            int ChannelReasonID = 0;
            if (Config.ReasonChannelID != 0 && ChatID != 0 && MessageID != 0)
                ReasonID = TgApi.getDefaultApiConnection().forwardMessage(Config.ReasonChannelID, ChatID, MessageID)
                    .result.message_id;
            if (Config.MainChannelID != 0)
            {
                if (userinfo == null)
                {
                    UserInfoRequest userinforeq = TgApi.getDefaultApiConnection().getChat(UserID);
                    if (userinforeq.ok)
                    {
                        userinfo = userinforeq.result;
                        banmsg = userinfo.GetUserTextInfo();
                    }
                    else
                    {
                        finalResult = false;
                        banmsg = "User ID : " + UserID;
                    }
                }
                else
                {
                    banmsg = userinfo.GetUserTextInfo();
                }

                string textlevel;
                if (Level == 0)
                    textlevel = "封鎖";
                else if (Level == 1)
                    textlevel = "警告";
                else
                    textlevel = Level + " （未知）";
                banmsg += "\n處分 : " + textlevel;
                string ExpTime = GetTime.GetExpiresTime(Expires);
                if (ExpTime != "永久封鎖")
                    banmsg += "\n時效至 : " + GetTime.GetExpiresTime(Expires);
                else
                    banmsg += "\n時效 : 永久";
                banmsg += "\n原因 : " + Reason;
                if(AdminID == 0)
                    banmsg += "\nOID : Bot\n";
                else if(AdminID == 1 || ChatID == Config.InternGroupID)
                    banmsg += "\nOID : Auditors\n";
                else
                    banmsg += "\nOID : " + AdminID + "\n";
                if (Config.ReasonChannelID != 0 && ReasonID != 0 && Config.ReasonChannelName != null)
                    banmsg += "\n參考 : \nhttps://t.me/" + Config.ReasonChannelName + "/" + ReasonID;
                else if (Config.ReasonChannelID != 0 && ChatID != 0 && MessageID != 0) finalResult = false;
                banmsg += "\n";
                try
                {
                    if (ChatID != Config.InternGroupID)
                        banmsg += "\n" + TgApi.getDefaultApiConnection().getChatInfo(ChatID).result.GetChatTextInfo();
                }
                catch
                {
                }

                ChannelReasonID = TgApi.getDefaultApiConnection().sendMessage(Config.MainChannelID, banmsg).result
                    .message_id;
            }

            ChangeDbBan(AdminID, UserID, Level, Expires, Reason, ChannelReasonID, ReasonID);
            CNBlacklistApi.PostToAPI(UserID, true, Level, Expires, Reason);
            return finalResult;
        }

        public bool UnbanUser(
            int AdminID,
            int UserID,
            string Reason = null,
            UserInfo userinfo = null
        )
        {
            int ChannelReasonID = 0;
            if (Config.MainChannelID != 0)
            {
                string banmsg = "";
                if (userinfo == null)
                {
                    UserInfoRequest userinforeq = TgApi.getDefaultApiConnection().getChat(UserID);
                    if (userinforeq.ok)
                    {
                        userinfo = userinforeq.result;
                        banmsg = userinfo.GetUserTextInfo();
                    }
                    else
                    {
                        banmsg = "User ID : " + UserID;
                    }
                }
                else
                {
                    banmsg = userinfo.GetUserTextInfo();
                }

                banmsg += "\n\n已被解除封鎖";

                if (Reason != null) banmsg += "，原因 : \n" + Reason;

                banmsg += "\n原封鎖原因 : \n" + Config.GetDatabaseManager().GetUserBanStatus(UserID).Reason + "\n";

                banmsg += "\nOID : " + AdminID + "\n";

                BanUser ban = Config.GetDatabaseManager().GetUserBanStatus(UserID);
                if (ban.Ban == 1) return false;

                ChannelReasonID = TgApi.getDefaultApiConnection().sendMessage(Config.MainChannelID, banmsg).result
                    .message_id;
            }

            ChangeDbUnban(AdminID, UserID, Reason, ChannelReasonID);
            CNBlacklistApi.PostToAPI(UserID, false, 1, 0, Reason);
            return true;
        }

        private void ChangeBanTemp(
            int AdminID,
            int UserID,
            int Level,
            long Expires,
            string Reason)
        {
            BanUser baninfo = new BanUser
            {
                UserID = UserID,
                Ban = 0,
                Level = Level,
                Reason = Reason,
                HistoryID = 0,
                ChannelMessageID = 0,
                ReasonMessageID = 0,
                Expires = Expires
            };
            using (var db = new BlacklistDatabaseContext())
            {
                try
                {
                    db.BanUsers.Add(baninfo);
                    db.SaveChanges();
                }
                catch (SqliteException)
                {
                    db.BanUsers.Update(baninfo);
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    db.BanUsers.Update(baninfo);
                    db.SaveChanges();
                }
            }
        }

        public void ChangeDbBan(
            int AdminID,
            int UserID,
            int Level,
            long Expires,
            string Reason,
            int ChannelMessageID = 0,
            int ReasonMessageID = 0
        )
        {
            BanUser baninfo = new BanUser
            {
                UserID = UserID,
                Ban = 0,
                Level = Level,
                Reason = Reason,
                HistoryID = 0,
                ChannelMessageID = ChannelMessageID,
                ReasonMessageID = ReasonMessageID,
                Expires = Expires
            };
            Config.bannedUsers[UserID] = baninfo;
            BanHistory banHistory = new BanHistory
            {
                UserID = UserID,
                Ban = 0,
                Level = Level,
                ChannelMessageID = ChannelMessageID,
                ReasonMessageID = ReasonMessageID,
                AdminID = AdminID,
                BanTime = GetTime.GetUnixTime(),
                Reason = Reason,
                Expires = Expires
            };
            using (var db = new BlacklistDatabaseContext())
            {
                db.BanHistorys.Add(banHistory);
                try
                {
                    db.BanUsers.Add(baninfo);
                    db.SaveChanges();
                }
                catch (SqliteException)
                {
                    db.BanUsers.Update(baninfo);
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    db.BanUsers.Update(baninfo);
                    db.SaveChanges();
                }
            }

            new SubscribeBanListCaller().CallGroupsInThread(baninfo);
        }

        public void ChangeDbUnban(
            int AdminID,
            int UserID,
            string Reason,
            int ChannelMessageID = 0
        )
        {
            Config.bannedUsers.Remove(UserID);
            BanHistory banHistory = new BanHistory
            {
                UserID = UserID,
                Ban = 1,
                Level = 1,
                ChannelMessageID = ChannelMessageID,
                ReasonMessageID = 0,
                AdminID = AdminID,
                BanTime = GetTime.GetUnixTime(),
                Reason = Reason,
                Expires = 0
            };
            using (var db = new BlacklistDatabaseContext())
            {
                db.BanHistorys.Add(banHistory);
                var bannedUser = db.BanUsers
                    .Single(users => users.UserID == UserID);
                db.Remove(bannedUser);
                db.SaveChanges();
            }

            new UnBanCaller().UnBanCallerThread(UserID);
        }

        public BanUser GetUserBanStatus(int uid)
        {
            BanUser banuser = null;
            banuser = Config.bannedUsers.GetValueOrDefault(uid, null);
            if (banuser != null)
            {
                if (GetTime.GetIsExpired(banuser.Expires))
                {
                    banuser.Ban = 1;
                    Config.bannedUsers[uid] = banuser;
                }

                return banuser;
            }

            using (var db = new BlacklistDatabaseContext())
            {
                BanUser bannedUser;
                try
                {
                    bannedUser = db.BanUsers
                        .Single(users => users.UserID == uid);
                    Config.bannedUsers.TryAdd(uid, bannedUser);
                }
                catch (InvalidOperationException)
                {
                    bannedUser = new BanUser {Ban = 1};
                    Config.bannedUsers.TryAdd(uid, bannedUser);
                    return bannedUser;
                }

                if (GetTime.GetIsExpired(bannedUser.Expires))
                {
                    bannedUser.Ban = 1;
                    db.BanUsers.Update(bannedUser);
                }

                return bannedUser;
            }
        }

        public GroupCfg GetGroupConfig(long gid)
        {
            GroupCfg config = null;
            config = Config.groupConfig.GetValueOrDefault(gid, null);
            if (config != null) return config;
            using (var db = new BlacklistDatabaseContext())
            {
                GroupCfg groupCfg;
                try
                {
                    groupCfg = db.GroupConfig
                        .Single(groups => groups.GroupID == gid);
                    Config.groupConfig.TryAdd(gid, groupCfg);
                }
                catch (InvalidOperationException)
                {
                    groupCfg = new GroupCfg
                    {
                        GroupID = gid,
                        AdminOnly = Config.DefaultSoamAdminOnly,
                        BlackList = Config.DefaultSoamBlacklist,
                        AutoKick = Config.DefaultSoamAutoKick,
                        AntiBot = Config.DefaultSoamAntiBot,
                        AntiHalal = Config.DefaultSoamAntiHalal,
                        AutoDeleteSpamMessage = Config.DefaultSoamAutoDeleteSpamMessage,
                        AutoDeleteCommand = Config.DefaultSoamAutoDeleteCommand,
                        SubscribeBanList = Config.DefaultSoamSubscribeBanList
                    };
                    db.GroupConfig.Add(groupCfg);
                    Config.groupConfig.TryAdd(gid, groupCfg);
                    db.SaveChanges();
                    return groupCfg;
                }

                return groupCfg;
            }
        }

        public GroupCfg SetGroupConfig(
            long gid,
            int AdminOnly = 3,
            int BlackList = 3,
            int AutoKick = 3,
            int AntiBot = 3,
            int AntiHalal = 3,
            int AutoDeleteSpamMessage = 3,
            int AutoDeleteCommand = 3,
            int SubscribeBanList = 3
        )
        {
            GroupCfg groupCfg = GetGroupConfig(gid);
            if (AdminOnly != 3) groupCfg.AdminOnly = AdminOnly;
            if (BlackList != 3) groupCfg.BlackList = BlackList;
            if (AutoKick != 3) groupCfg.AutoKick = AutoKick;
            if (AntiBot != 3) groupCfg.AntiBot = AntiBot;
            if (AntiHalal != 3) groupCfg.AntiHalal = AntiHalal;
            if (AutoDeleteSpamMessage != 3) groupCfg.AutoDeleteSpamMessage = AutoDeleteSpamMessage;
            if (AutoDeleteCommand != 3) groupCfg.AutoDeleteCommand = AutoDeleteCommand;
            if (SubscribeBanList != 3) groupCfg.SubscribeBanList = SubscribeBanList;
            Config.groupConfig[gid] = groupCfg;
            using (var db = new BlacklistDatabaseContext())
            {
                try
                {
                    db.GroupConfig.Add(groupCfg);
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    db.GroupConfig.Update(groupCfg);
                    db.SaveChanges();
                }

                return groupCfg;
            }
        }

        public bool RemoveGroupCfg(long GroupID)
        {
            using (var db = new BlacklistDatabaseContext())
            {
                try
                {
                    var groupCfg = db.GroupConfig
                        .Single(groups => groups.GroupID == GroupID);
                    db.Remove(groupCfg);
                    db.SaveChanges();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<SpamMessage> GetSpamMessageList()
        {
            if (Config.spamMessageList == null)
            {
                string jsonText;
                List<SpamMessage> data;
                try
                {
                    jsonText = File.ReadAllText(ConfigManager.GetConfigPath() + "spamstrings.json");
                    data = (List<SpamMessage>) new DataContractJsonSerializer(
                        typeof(List<SpamMessage>)
                    ).ReadObject(
                        new MemoryStream(
                            Encoding.UTF8.GetBytes(jsonText)
                        )
                    );
                }
                catch
                {
                    data = new List<SpamMessage>();
                }

                Config.spamMessageList = data;
                return data;
            }

            return Config.spamMessageList;
        }

        public SpamMessage GetSpamRule(string Name)
        {
            List<SpamMessage> spamMessageList = GetSpamMessageList();
            foreach (SpamMessage smsg in spamMessageList)
                if (smsg.FriendlyName.Equals(Name))
                    return smsg;
            return null;
        }

        public void AddSpamMessage(SpamMessage msg)
        {
            List<SpamMessage> spamMessageList = GetSpamMessageList();
            spamMessageList.Add(msg);
            Config.spamMessageList = spamMessageList;
            WriteSpamMessageToDatabase(spamMessageList);
        }

        public void ChangeSpamMessage(SpamMessage msg)
        {
            List<SpamMessage> spamMessageList = GetSpamMessageList();
            int length = spamMessageList.Count;
            for (int i = 0; i < length; i++)
            {
                SpamMessage nowMsg = spamMessageList[i];
                if (nowMsg.FriendlyName == msg.FriendlyName) spamMessageList[i] = msg;
            }

            Config.spamMessageList = spamMessageList;
            WriteSpamMessageToDatabase(spamMessageList);
        }

        public int DeleteSpamMessage(string friendlyName)
        {
            List<SpamMessage> spamMessageList = GetSpamMessageList();
            List<SpamMessage> newList = new List<SpamMessage>();
            int deletedCount = 0;
            foreach (SpamMessage smsg in spamMessageList)
                if (smsg.FriendlyName != friendlyName)
                    newList.Add(smsg);
                else
                    deletedCount++;
            Config.spamMessageList = newList;
            WriteSpamMessageToDatabase(newList);
            return deletedCount;
        }

        private void WriteSpamMessageToDatabase(List<SpamMessage> msg)
        {
            string jsonDB = TgApi.getDefaultApiConnection().jsonEncode(msg);
            File.WriteAllText(ConfigManager.GetConfigPath() + "spamstrings.json", jsonDB);
        }
        
        public IDList GetIDList(string name)
        {
            IDList config = null;
            config = Config.IDList.GetValueOrDefault(name, null);
            if (config != null) return config;
            using (var db = new BlacklistDatabaseContext())
            {
                IDList idList;
                try
                {
                    idList = db.IDList
                        .Single(idLists => idLists.Name == name);
                    Config.IDList.TryAdd(name, idList);
                }
                catch (InvalidOperationException)
                {
                    idList = new IDList
                    {
                        Name = name,
                        Data = ""
                    };
                    db.IDList.Add(idList);
                    Config.IDList.TryAdd(name, idList);
                    db.SaveChanges();
                    return idList;
                }

                return idList;
            }
        }

        public void ChangeDbIDList(
            string Name,
            string Data
        )
        {
            IDList idlist = new IDList
            {
                Name = Name,
                Data = Data
            };
            using (var db = new BlacklistDatabaseContext())
            {
                db.IDList.Add(idlist);
                try
                {
                    db.IDList.Add(idlist);
                    db.SaveChanges();
                }
                catch (SqliteException)
                {
                    db.IDList.Update(idlist);
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    db.IDList.Update(idlist);
                    db.SaveChanges();
                }
            }
        }
    }

    public class SpamMessage
    {
        public bool Enabled { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public bool AutoMute { get; set; } = false;
        public bool AutoKick { get; set; } = false;
        public bool AutoBlackList { get; set; } = false;
        public int BanLevel { get; set; } = 1;
        public int BanDays { get; set; } = 0;
        public int BanHours { get; set; } = 0;
        public int BanMinutes { get; set; } = 1;
        public int Type { get; set; } = 0;
        public int MinPoints { get; set; } = 1;
        public string FriendlyName { get; set; }
        public SpamMessageObj[] Messages { get; set; }
    }

    public class SpamMessageObj
    {
        public string Message { get; set; }
        public int Point { get; set; }
    }
}