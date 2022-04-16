using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using SystemModule;

namespace DBSvr
{
    public class MySqlPlayDataService : IPlayDataService
    {
        private readonly Dictionary<string, int> MirQuickList = null;
        private readonly TQuickIDList MirQuickIDList = null;
        private readonly Dictionary<int, int> QuickIndexIdList = null;
        private int _recordCount = 0;

        public MySqlPlayDataService()
        {
            MirQuickList = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            MirQuickIDList = new TQuickIDList();
            _recordCount = -1;
            QuickIndexIdList = new Dictionary<int, int>();
        }

        public void LoadQuickList()
        {
            const string sSQLString = "SELECT * FROM TBL_CHARACTER WHERE FLD_Deleted=0";
            MirQuickList.Clear();
            MirQuickIDList.Clear();
            _recordCount = -1;
            IList<TQuickID> AccountList = new List<TQuickID>();
            IList<string> ChrNameList = new List<string>();
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            try
            {
                var command = new MySqlCommand();
                command.CommandText = sSQLString;
                command.Connection = dbConnection;
                using var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    var nIndex = dr.GetInt32("Id");
                    var boDeleted = dr.GetBoolean("FLD_DELETED");
                    var sAccount = dr.GetString("FLD_LOGINID");
                    var sChrName = dr.GetString("FLD_CHARNAME");
                    if (!boDeleted && (!string.IsNullOrEmpty(sChrName)))
                    {
                        MirQuickList.Add(sChrName, nIndex);
                        AccountList.Add(new TQuickID()
                        {
                            sAccount = sAccount,
                            nSelectID = 0
                        });
                        ChrNameList.Add(sChrName);
                        QuickIndexIdList.Add(nIndex, nIndex);
                    }
                }
            }
            finally
            {
                Close(dbConnection);
            }
            for (var nIndex = 0; nIndex < AccountList.Count; nIndex++)
            {
                MirQuickIDList.AddRecord(AccountList[nIndex].sAccount, ChrNameList[nIndex], 0, AccountList[nIndex].nSelectID);
            }
            AccountList = null;
            ChrNameList = null;
        }

        private MySqlConnection Open(ref bool success)
        {
            var dbConnection = new MySqlConnection(DBShare.DBConnection);
            try
            {
                dbConnection.Open();
                success = true;
            }
            catch (Exception e)
            {
                DBShare.MainOutMessage("打开数据库[MySql]失败.");
                DBShare.MainOutMessage(e.StackTrace);
                success = false;
            }
            return dbConnection;
        }

        private void Close(MySqlConnection dbConnection)
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public int Index(string sName)
        {
            if (MirQuickList.ContainsKey(sName))
            {
                return MirQuickList[sName];
            }
            return -1;
        }

        public int Get(int nIndex, ref THumDataInfo HumanRCD)
        {
            var result = -1;
            if (nIndex < 0)
            {
                return result;
            }
            if (MirQuickList.Count < nIndex)
            {
                return result;
            }
            if (GetRecord(nIndex, ref HumanRCD))
            {
                result = nIndex;
            }
            return result;
        }

        public bool Update(int nIndex, ref THumDataInfo HumanRCD)
        {
            var result = false;
            if ((nIndex >= 0) && (MirQuickList.Count >= nIndex))
            {
                if (UpdateRecord(nIndex, ref HumanRCD))
                {
                    result = true;
                }
            }
            return result;
        }

        public bool UpdateQryChar(int nIndex, TQueryChr QueryChrRcd)
        {
            var result = false;
            if ((nIndex >= 0) && (MirQuickList.Count > nIndex))
            {
                if (UpdateChrRecord(nIndex, QueryChrRcd))
                {
                    result = true;
                }
            }
            return result;
        }

        private bool UpdateChrRecord(int playerId, TQueryChr QueryChrRcd)
        {
            var result = false;
            var dbConnection = Open(ref result);
            try
            {
                if (!result)
                {
                    return false;
                }
                const string sStrString = "UPDATE TBL_CHARACTER SET FLD_SEX=@FLD_SEX, FLD_JOB=@FLD_JOB WHERE ID=@ID";
                var command = new MySqlCommand();
                command.CommandText = sStrString;
                command.Parameters.AddWithValue("@FLD_SEX", QueryChrRcd.btSex);
                command.Parameters.AddWithValue("@FLD_JOB", QueryChrRcd.btJob);
                command.Parameters.AddWithValue("@ID", playerId);
                command.Connection = dbConnection;
                try
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
                catch
                {
                    DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateChrRecord");
                    result = false;
                }
            }
            finally
            {
                Close(dbConnection);
            }
            return result;
        }

        public bool Add(ref THumDataInfo HumanRCD)
        {
            var result = false;
            int nIndex;
            var sChrName = HumanRCD.Header.sName;
            if (MirQuickList.TryGetValue(sChrName, out nIndex))
            {
                if (nIndex >= 0)
                {
                    return false;
                }
            }
            else
            {
                nIndex = _recordCount;
                _recordCount++;
                if (AddRecord(ref nIndex, ref HumanRCD))
                {
                    MirQuickList.Add(sChrName, nIndex);
                    QuickIndexIdList.Add(nIndex, nIndex);
                    result = true;
                }
            }
            return result;
        }

        private bool GetRecord(int nIndex, ref THumDataInfo HumanRCD)
        {
            var playerId = 0;
            if (HumanRCD == null)
            {
                playerId = QuickIndexIdList[nIndex];
            }
            if (playerId == 0)
            {
                return false;
            }
            GetChrRecord(playerId, ref HumanRCD);
            GetAbilGetRecord(playerId, ref HumanRCD);
            GetBonusAbilRecord(playerId, ref HumanRCD);
            GetMagicRecord(playerId, ref HumanRCD);
            GetItemRecord(playerId, ref HumanRCD);
            GetStorageRecord(playerId, ref HumanRCD);
            GetPlayerStatus(playerId, ref HumanRCD);
            return true;
        }

        private void GetChrRecord(int playerId, ref THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            const string sSQLString = "SELECT * FROM TBL_CHARACTER WHERE ID={0}";
            var command = new MySqlCommand();
            try
            {
                command.CommandText = string.Format(sSQLString, playerId);
                command.Connection = dbConnection;
                using var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    HumanRCD = new THumDataInfo();
                    HumanRCD.Data.Initialization();
                    HumanRCD.Data.sAccount = dr.GetString("FLD_LOGINID");
                    HumanRCD.Header.sName = dr.GetString("FLD_CHARNAME");
                    HumanRCD.Header.boDeleted = dr.GetBoolean("FLD_DELETED");
                    HumanRCD.Header.dCreateDate = HUtil32.DateTimeToDouble(dr.GetDateTime("FLD_CREATEDATE"));
                    HumanRCD.Data.sCharName = dr.GetString("FLD_CHARNAME");
                    if (!dr.IsDBNull(dr.GetOrdinal("FLD_MAPNAME")))
                    {
                        HumanRCD.Data.sCurMap = dr.GetString("FLD_MAPNAME");
                    }
                    HumanRCD.Data.wCurX = dr.GetInt16("FLD_CX");
                    HumanRCD.Data.wCurY = dr.GetInt16("FLD_CY");
                    HumanRCD.Data.btDir = dr.GetByte("FLD_DIR");
                    HumanRCD.Data.btHair = dr.GetByte("FLD_HAIR");
                    HumanRCD.Data.btSex = dr.GetByte("FLD_SEX");
                    HumanRCD.Data.btJob = dr.GetByte("FLD_JOB");
                    HumanRCD.Data.nGold = dr.GetInt32("FLD_GOLD");
                    if (!dr.IsDBNull(dr.GetOrdinal("FLD_HOMEMAP")))
                    {
                        HumanRCD.Data.sHomeMap = dr.GetString("FLD_HOMEMAP");
                    }
                    HumanRCD.Data.wHomeX = dr.GetInt16("FLD_HOMEX");
                    HumanRCD.Data.wHomeY = dr.GetInt16("FLD_HOMEY");
                    if (!dr.IsDBNull(dr.GetOrdinal("FLD_DearName")))
                    {
                        HumanRCD.Data.sDearName = dr.GetString("FLD_DearName");
                    }
                    if (!dr.IsDBNull(dr.GetOrdinal("FLD_MasterName")))
                    {
                        HumanRCD.Data.sMasterName = dr.GetString("FLD_MasterName");
                    }
                    HumanRCD.Data.boMaster = dr.GetBoolean("FLD_IsMaster");
                    HumanRCD.Data.btCreditPoint = (byte)dr.GetInt32("FLD_CREDITPOINT");
                    if (!dr.IsDBNull(dr.GetOrdinal("FLD_StoragePwd")))
                    {
                        HumanRCD.Data.sStoragePwd = dr.GetString("FLD_StoragePwd");
                    }
                    HumanRCD.Data.btReLevel = dr.GetByte("FLD_ReLevel");
                    HumanRCD.Data.boLockLogon = dr.GetBoolean("FLD_LOCKLOGON");
                    HumanRCD.Data.nBonusPoint = dr.GetInt32("FLD_BONUSPOINT");
                    HumanRCD.Data.nGameGold = dr.GetInt32("FLD_Gold");
                    HumanRCD.Data.nGamePoint = dr.GetInt32("FLD_GamePoint");
                    HumanRCD.Data.nPayMentPoint = dr.GetInt32("FLD_PayMentPoint");
                    HumanRCD.Data.nHungerStatus = dr.GetInt32("FLD_HungerStatus");
                    HumanRCD.Data.btAllowGroup = (byte)dr.GetInt32("FLD_AllowGroup");
                    HumanRCD.Data.btAttatckMode = dr.GetByte("FLD_AttatckMode");
                    HumanRCD.Data.btIncHealth = dr.GetByte("FLD_IncHealth");
                    HumanRCD.Data.btIncSpell = dr.GetByte("FLD_IncSpell");
                    HumanRCD.Data.btIncHealing = dr.GetByte("FLD_IncHealing");
                    HumanRCD.Data.btFightZoneDieCount = dr.GetByte("FLD_FightZoneDieCount");
                    HumanRCD.Data.boAllowGuildReCall = dr.GetBoolean("FLD_AllowGuildReCall");
                    HumanRCD.Data.boAllowGroupReCall = dr.GetBoolean("FLD_AllowGroupReCall");
                    HumanRCD.Data.wGroupRcallTime = dr.GetInt16("FLD_GroupRcallTime");
                    HumanRCD.Data.dBodyLuck = dr.GetDouble("FLD_BodyLuck");
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.GetChrRecord");
                DBShare.MainOutMessage(ex.StackTrace);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void GetAbilGetRecord(int playerId, ref THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            int dw;
            var command = new MySqlCommand();
            try
            {
                command.CommandText = $"select * from TBL_CHARACTER_ABLITY where FLD_PLAYERID={playerId}";
                command.Connection = dbConnection;
                using var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    HumanRCD.Data.Abil.Level = dr.GetUInt16("FLD_LEVEL");
                    dw = dr.GetInt32("FLD_HP");
                    HumanRCD.Data.Abil.HP = HUtil32.LoWord(dw);
                    HumanRCD.Data.Abil.AC = HUtil32.HiWord(dw);
                    dw = dr.GetInt32("FLD_MP");
                    HumanRCD.Data.Abil.MP = HUtil32.LoWord(dw);
                    HumanRCD.Data.Abil.MAC = HUtil32.HiWord(dw);
                    HumanRCD.Data.Abil.DC = dr.GetInt32("FLD_DC");
                    HumanRCD.Data.Abil.MC = dr.GetInt32("FLD_MC");
                    HumanRCD.Data.Abil.SC = dr.GetInt32("FLD_SC");
                    HumanRCD.Data.Abil.Exp = dr.GetInt32("FLD_EXP");
                    HumanRCD.Data.Abil.MaxExp = dr.GetInt32("FLD_MaxExp");
                    HumanRCD.Data.Abil.Weight = dr.GetUInt16("FLD_Weight");
                    HumanRCD.Data.Abil.MaxWeight = dr.GetUInt16("FLD_MaxWeight");
                    HumanRCD.Data.Abil.WearWeight = dr.GetUInt16("FLD_WearWeight");
                    HumanRCD.Data.Abil.MaxWearWeight = dr.GetUInt16("FLD_MaxWearWeight");
                    HumanRCD.Data.Abil.HandWeight = dr.GetUInt16("FLD_HandWeight");
                    HumanRCD.Data.Abil.MaxHandWeight = dr.GetUInt16("FLD_MaxHandWeight");
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.GetAbilGetRecord");
                DBShare.MainOutMessage(ex.StackTrace);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void GetBonusAbilRecord(int playerId, ref THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            const string sSQLString = "SELECT * FROM TBL_BONUSABILITY WHERE FLD_PLAYERID={0}";
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            try
            {
                command.CommandText = string.Format(sSQLString, playerId);
                using var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    if (HumanRCD.Data.BonusAbil == null)
                    {
                        HumanRCD.Data.BonusAbil = new TNakedAbility();
                    }
                    HumanRCD.Data.BonusAbil.AC = dr.GetUInt16("FLD_AC");
                    HumanRCD.Data.BonusAbil.MAC = dr.GetUInt16("FLD_MAC");
                    HumanRCD.Data.BonusAbil.DC = dr.GetUInt16("FLD_DC");
                    HumanRCD.Data.BonusAbil.MC = dr.GetUInt16("FLD_MC");
                    HumanRCD.Data.BonusAbil.SC = dr.GetUInt16("FLD_SC");
                    HumanRCD.Data.BonusAbil.HP = dr.GetUInt16("FLD_HP");
                    HumanRCD.Data.BonusAbil.MP = dr.GetUInt16("FLD_MP");
                    HumanRCD.Data.BonusAbil.Hit = dr.GetByte("FLD_HIT");
                    HumanRCD.Data.BonusAbil.Speed = dr.GetInt32("FLD_SPEED");
                    HumanRCD.Data.BonusAbil.X2 = dr.GetByte("FLD_RESERVED");
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.GetBonusAbilRecord");
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void GetMagicRecord(int playerId, ref THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            const string sSQLString = "SELECT * FROM TBL_CHARACTER_MAGIC WHERE FLD_PLAYERID={0}";
            var command = new MySqlCommand();
            try
            {
                for (var i = 0; i < HumanRCD.Data.Magic.Length; i++)
                {
                    HumanRCD.Data.Magic[i] = new TMagicRcd();
                }
                command.Connection = dbConnection;
                command.CommandText = string.Format(sSQLString, playerId);
                using var dr = command.ExecuteReader();
                var position = 0;
                while (dr.Read())
                {
                    HumanRCD.Data.Magic[position].wMagIdx = dr.GetUInt16("FLD_MAGICID");
                    HumanRCD.Data.Magic[position].btKey = (byte)dr.GetInt32("FLD_USEKEY");
                    HumanRCD.Data.Magic[position].btLevel = (byte)dr.GetInt32("FLD_LEVEL");
                    HumanRCD.Data.Magic[position].nTranPoint = dr.GetInt32("FLD_CURRTRAIN");
                    position++;
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.GetMagicRecord");
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void GetItemRecord(int playerId, ref THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            const string sSQLString = "SELECT * FROM TBL_ITEM WHERE FLD_PLAYERID={0}";
            var command = new MySqlCommand();
            try
            {
                command.Connection = dbConnection;
                command.CommandText = string.Format(sSQLString, playerId);
                using var dr = command.ExecuteReader();
                var i = 0;
                for (var j = 0; j < HumanRCD.Data.HumItems.Length; j++)
                {
                    HumanRCD.Data.HumItems[j] = new TUserItem();
                }
                for (var j = 0; j < HumanRCD.Data.BagItems.Length; j++)
                {
                    HumanRCD.Data.BagItems[j] = new TUserItem();
                }
                while (dr.Read())
                {
                    var nPosition = dr.GetInt32("FLD_POSITION");
                    if ((nPosition >= 0) && (nPosition <= 9))
                    {
                        HumanRCD.Data.HumItems[nPosition].MakeIndex = dr.GetInt32("FLD_MAKEINDEX");
                        HumanRCD.Data.HumItems[nPosition].wIndex = dr.GetUInt16("FLD_STDINDEX");
                        HumanRCD.Data.HumItems[nPosition].Dura = dr.GetUInt16("FLD_DURA");
                        HumanRCD.Data.HumItems[nPosition].DuraMax = dr.GetUInt16("FLD_DURAMAX");
                        for (var ii = 0; ii < 14; ii++)
                        {
                            HumanRCD.Data.HumItems[nPosition].btValue[ii] = (byte)dr.GetInt32($"FLD_VALUE{ii}");
                        }
                    }
                    else
                    {
                        if (i > HumanRCD.Data.BagItems.Length - 1)
                        {
                            break;
                        }
                        HumanRCD.Data.BagItems[i].MakeIndex = dr.GetInt32("FLD_MAKEINDEX");
                        HumanRCD.Data.BagItems[i].wIndex = dr.GetUInt16("FLD_STDINDEX");
                        HumanRCD.Data.BagItems[i].Dura = dr.GetUInt16("FLD_DURA");
                        HumanRCD.Data.BagItems[i].DuraMax = dr.GetUInt16("FLD_DURAMAX");
                        for (var ii = 0; ii < 14; ii++)
                        {
                            HumanRCD.Data.BagItems[i].btValue[ii] = (byte)dr.GetInt32($"FLD_VALUE{ii}");
                        }
                        i++;
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.GetItemRecord:" + ex.StackTrace);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void GetStorageRecord(int playerId, ref THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            for (var i = 0; i < HumanRCD.Data.StorageItems.Length; i++)
            {
                HumanRCD.Data.StorageItems[i] = new TUserItem();
            }
            const string sSQLString = "SELECT * FROM TBL_Storages WHERE FLD_PLAYERID={0}";
            var command = new MySqlCommand();
            try
            {
                command.CommandText = string.Format(sSQLString, playerId);
                command.Connection = dbConnection;
                var dr = command.ExecuteReader();
                var i = 0;
                while (dr.Read())
                {
                    HumanRCD.Data.StorageItems[i].MakeIndex = dr.GetInt32("FLD_MAKEINDEX");
                    HumanRCD.Data.StorageItems[i].wIndex = dr.GetUInt16("FLD_STDINDEX");
                    HumanRCD.Data.StorageItems[i].Dura = dr.GetUInt16("FLD_DURA");
                    HumanRCD.Data.StorageItems[i].DuraMax = dr.GetUInt16("FLD_DURAMAX");
                    for (var ii = 0; ii < 14; ii++)
                    {
                        HumanRCD.Data.StorageItems[i].btValue[ii] = dr.GetByte(string.Format("FLD_VALUE{0}", ii));
                    }
                    i++;
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.GetStorageRecord");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void GetPlayerStatus(int playerId, ref THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            const string sSQLString = "SELECT * FROM TBL_CHARACTER_STATUS WHERE FLD_PLAYERID={0}";
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            try
            {
                command.CommandText = string.Format(sSQLString, playerId);
                using var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    var sTmp = dr.GetString("FLD_STATUS");
                    var i = 0;
                    var str = string.Empty;
                    while (!string.IsNullOrEmpty(sTmp))
                    {
                        sTmp = HUtil32.GetValidStr3(sTmp, ref str, new[] { "/" });
                        HumanRCD.Data.wStatusTimeArr[i] = Convert.ToUInt16(str);
                        i++;
                        if (i > HumanRCD.Data.wStatusTimeArr.GetUpperBound(0))
                        {
                            break;
                        }
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            catch (Exception)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.GetPlayerStatus");
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private bool AddRecord(ref int nIndex, ref THumDataInfo HumanRCD)
        {
            return InsertRecord(HumanRCD.Data, ref nIndex);
        }

        private bool InsertRecord(THumInfoData hd, ref int nIndex)
        {
            var strSql = new StringBuilder();
            strSql.AppendLine("INSERT INTO TBL_CHARACTER (FLD_ServerNum, FLD_LoginID, FLD_CharName, FLD_MapName, FLD_CX, FLD_CY, FLD_Level, FLD_Dir, FLD_Hair, FLD_Sex, FLD_Job, FLD_Gold, FLD_GamePoint, FLD_HomeMap,");
            strSql.AppendLine("FLD_HomeX, FLD_HomeY, FLD_PkPoint, FLD_ReLevel, FLD_AttatckMode, FLD_FightZoneDieCount, FLD_BodyLuck, FLD_IncHealth,FLD_IncSpell, FLD_IncHealing, FLD_CreditPoint, FLD_BonusPoint,");
            strSql.AppendLine("FLD_HungerStatus, FLD_PayMentPoint, FLD_LockLogon, FLD_MarryCount, FLD_AllowGroupReCall, FLD_GroupRcallTime, FLD_AllowGuildReCall, FLD_IsMaster, FLD_MasterName, FLD_DearName");
            strSql.AppendLine(",FLD_StoragePwd, FLD_Deleted, FLD_CREATEDATE, FLD_LASTUPDATE) VALUES ");
            strSql.AppendLine("(@FLD_ServerNum, @FLD_LoginID, @FLD_CharName, @FLD_MapName, @FLD_CX, @FLD_CY, @FLD_Level, @FLD_Dir, @FLD_Hair, @FLD_Sex, @FLD_Job, @FLD_Gold, @FLD_GamePoint, @FLD_HomeMap,");
            strSql.AppendLine("@FLD_HomeX, @FLD_HomeY, @FLD_PkPoint, @FLD_ReLevel, @FLD_AttatckMode, @FLD_FightZoneDieCount, @FLD_BodyLuck, @FLD_IncHealth,@FLD_IncSpell, @FLD_IncHealing, @FLD_CreditPoint, @FLD_BonusPoint,");
            strSql.AppendLine("@FLD_HungerStatus, @FLD_PayMentPoint, @FLD_LockLogon, @FLD_MarryCount, @FLD_AllowGroupReCall, @FLD_GroupRcallTime, @FLD_AllowGuildReCall, @FLD_IsMaster, @FLD_MasterName, @FLD_DearName");
            strSql.AppendLine(",@FLD_StoragePwd, @FLD_Deleted, now(), now()) ");
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return false;
            }
            var command = new MySqlCommand();
            command.Parameters.AddWithValue("@FLD_ServerNum", 1);
            command.Parameters.AddWithValue("@FLD_LoginID", hd.sAccount);
            command.Parameters.AddWithValue("@FLD_CharName", hd.sCharName);
            command.Parameters.AddWithValue("@FLD_MapName", hd.sCurMap);
            command.Parameters.AddWithValue("@FLD_CX", hd.wCurX);
            command.Parameters.AddWithValue("@FLD_CY", hd.wCurY);
            command.Parameters.AddWithValue("@FLD_Level", hd.Abil.Level);
            command.Parameters.AddWithValue("@FLD_Dir", hd.btDir);
            command.Parameters.AddWithValue("@FLD_Hair", hd.btHair);
            command.Parameters.AddWithValue("@FLD_Sex", hd.btSex);
            command.Parameters.AddWithValue("@FLD_Job", hd.btJob);
            command.Parameters.AddWithValue("@FLD_Gold", hd.nGold);
            command.Parameters.AddWithValue("@FLD_GamePoint", hd.nGamePoint);
            command.Parameters.AddWithValue("@FLD_HomeMap", hd.sHomeMap);
            command.Parameters.AddWithValue("@FLD_HomeX", hd.wHomeX);
            command.Parameters.AddWithValue("@FLD_HomeY", hd.wHomeY);
            command.Parameters.AddWithValue("@FLD_PkPoint", hd.nPKPoint);
            command.Parameters.AddWithValue("@FLD_ReLevel", hd.btReLevel);
            command.Parameters.AddWithValue("@FLD_AttatckMode", hd.btAttatckMode);
            command.Parameters.AddWithValue("@FLD_FightZoneDieCount", hd.btFightZoneDieCount);
            command.Parameters.AddWithValue("@FLD_BodyLuck", hd.dBodyLuck);
            command.Parameters.AddWithValue("@FLD_IncHealth", hd.btIncHealth);
            command.Parameters.AddWithValue("@FLD_IncSpell", hd.btIncSpell);
            command.Parameters.AddWithValue("@FLD_IncHealing", hd.btIncHealing);
            command.Parameters.AddWithValue("@FLD_CreditPoint", hd.btCreditPoint);
            command.Parameters.AddWithValue("@FLD_BonusPoint", hd.nBonusPoint);
            command.Parameters.AddWithValue("@FLD_HungerStatus", hd.nHungerStatus);
            command.Parameters.AddWithValue("@FLD_PayMentPoint", hd.nPayMentPoint);
            command.Parameters.AddWithValue("@FLD_LockLogon", hd.boLockLogon);
            command.Parameters.AddWithValue("@FLD_MarryCount", hd.MarryCount);
            command.Parameters.AddWithValue("@FLD_AllowGroupReCall", hd.btAllowGroup);
            command.Parameters.AddWithValue("@FLD_GroupRcallTime", hd.wGroupRcallTime);
            command.Parameters.AddWithValue("@FLD_AllowGuildReCall", hd.boAllowGuildReCall);
            command.Parameters.AddWithValue("@FLD_IsMaster", hd.boMaster);
            command.Parameters.AddWithValue("@FLD_MasterName", hd.sMasterName);
            command.Parameters.AddWithValue("@FLD_DearName", hd.sDearName);
            command.Parameters.AddWithValue("@FLD_StoragePwd", hd.sStoragePwd);
            command.Parameters.AddWithValue("@FLD_Deleted", 0);
            command.CommandText = string.Format(strSql.ToString());
            command.Connection = dbConnection;
            try
            {
                command.ExecuteNonQuery();
                nIndex = (int)command.LastInsertedId;

                strSql.Clear();
                strSql.AppendLine("INSERT INTO TBL_CHARACTER_ABLITY (FLD_PlayerId, FLD_Level, FLD_Ac, FLD_Mac, FLD_Dc, FLD_Mc, FLD_Sc, FLD_Hp, FLD_Mp, FLD_MaxHP, FLD_MAxMP, FLD_Exp, FLD_MaxExp,");
                strSql.AppendLine(" FLD_Weight, FLD_MaxWeight, FLD_WearWeight,FLD_MaxWearWeight, FLD_HandWeight, FLD_MaxHandWeight) VALUES ");
                strSql.AppendLine(" (@FLD_PlayerId, @FLD_Level, @FLD_Ac, @FLD_Mac, @FLD_Dc, @FLD_Mc, @FLD_Sc, @FLD_Hp, @FLD_Mp, @FLD_MaxHP, @FLD_MAxMP, @FLD_Exp, @FLD_MaxExp, @FLD_Weight, @FLD_MaxWeight, @FLD_WearWeight, @FLD_MaxWearWeight, @FLD_HandWeight, @FLD_MaxHandWeight) ");

                command.CommandText = strSql.ToString();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FLD_PlayerId", nIndex);
                command.Parameters.AddWithValue("@FLD_Level", hd.Abil.Level);
                command.Parameters.AddWithValue("@FLD_Ac", hd.Abil.Level);
                command.Parameters.AddWithValue("@FLD_Mac", hd.Abil.MAC);
                command.Parameters.AddWithValue("@FLD_Dc", hd.Abil.DC);
                command.Parameters.AddWithValue("@FLD_Mc", hd.Abil.MC);
                command.Parameters.AddWithValue("@FLD_Sc", hd.Abil.SC);
                command.Parameters.AddWithValue("@FLD_Hp", hd.Abil.HP);
                command.Parameters.AddWithValue("@FLD_Mp", hd.Abil.MP);
                command.Parameters.AddWithValue("@FLD_MaxHP", hd.Abil.MaxHP);
                command.Parameters.AddWithValue("@FLD_MAxMP", hd.Abil.MaxMP);
                command.Parameters.AddWithValue("@FLD_Exp", hd.Abil.Exp);
                command.Parameters.AddWithValue("@FLD_MaxExp", hd.Abil.MaxExp);
                command.Parameters.AddWithValue("@FLD_Weight", hd.Abil.Weight);
                command.Parameters.AddWithValue("@FLD_MaxWeight", hd.Abil.MaxWeight);
                command.Parameters.AddWithValue("@FLD_WearWeight", hd.Abil.WearWeight);
                command.Parameters.AddWithValue("@FLD_MaxWearWeight", hd.Abil.MaxWearWeight);
                command.Parameters.AddWithValue("@FLD_HandWeight", hd.Abil.HandWeight);
                command.Parameters.AddWithValue("@FLD_MaxHandWeight", hd.Abil.MaxHandWeight);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.InsertRecord");
                DBShare.MainOutMessage(ex.StackTrace);
                return false;
            }
            try
            {
                strSql.Clear();
                strSql.AppendLine("INSERT INTO TBL_ITEM(FLD_PLAYERID,FLD_CHARNAME, FLD_POSITION, FLD_MAKEINDEX, FLD_STDINDEX, FLD_DURA, FLD_DURAMAX,");
                strSql.AppendLine("FLD_VALUE0, FLD_VALUE1, FLD_VALUE2, FLD_VALUE3, FLD_VALUE4, FLD_VALUE5, FLD_VALUE6, FLD_VALUE7, FLD_VALUE8, FLD_VALUE9, FLD_VALUE10, FLD_VALUE11, FLD_VALUE12, FLD_VALUE13) ");
                strSql.AppendLine(" VALUES ");
                strSql.AppendLine("(@FLD_PLAYERID,@FLD_CHARNAME, @FLD_POSITION, @FLD_MAKEINDEX, @FLD_STDINDEX, @FLD_DURA, @FLD_DURAMAX,@FLD_VALUE0, @FLD_VALUE1, @FLD_VALUE2, @FLD_VALUE3, @FLD_VALUE4, @FLD_VALUE5,");
                strSql.AppendLine("@FLD_VALUE6, @FLD_VALUE7, @FLD_VALUE8, @FLD_VALUE9, @FLD_VALUE10, @FLD_VALUE11, @FLD_VALUE12, @FLD_VALUE13)");
                for (int i = 0; i < hd.HumItems.Length + hd.BagItems.Length; i++)
                {
                    command.CommandText = strSql.ToString();
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@FLD_PLAYERID", nIndex);
                    command.Parameters.AddWithValue("@FLD_CHARNAME", hd.sCharName);
                    command.Parameters.AddWithValue("@FLD_POSITION", -1);
                    command.Parameters.AddWithValue("@FLD_MAKEINDEX", 0);
                    command.Parameters.AddWithValue("@FLD_STDINDEX", 0);
                    command.Parameters.AddWithValue("@FLD_DURA", 0);
                    command.Parameters.AddWithValue("@FLD_DURAMAX", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE0", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE1", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE2", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE3", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE4", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE5", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE6", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE7", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE8", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE9", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE10", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE11", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE12", 0);
                    command.Parameters.AddWithValue("@FLD_VALUE13", 0);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        DBShare.MainOutMessage("[Exception] MySqlHumDB.Insert TBL_ITEM");
                        DBShare.MainOutMessage(ex.StackTrace);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally{
                Close(dbConnection);
            }
            return true;
        }

        private bool UpdateRecord(int nIndex, ref THumDataInfo HumanRCD)
        {
            var result = true;
            try
            {
                UpdateRecord(nIndex, HumanRCD);
                UpdateAblity(nIndex, HumanRCD);
                UpdateItem(nIndex, HumanRCD);
                SavePlayerMagic(nIndex, HumanRCD);
                UpdateStatus(nIndex, HumanRCD);
                SaveItemStorge(nIndex, HumanRCD);
                UpdateBonusability(nIndex, HumanRCD);
            }
            catch (Exception ex)
            {
                result = false;
                DBShare.MainOutMessage($"保存玩家[{HumanRCD.Header.sName}]数据失败. " + ex.Message);
            }
            return result;
        }

        private void UpdateRecord(int Id, THumDataInfo HumanRCD)
        {
            var hd = HumanRCD.Data;
            var dwHP = HUtil32.MakeLong(hd.Abil.HP, hd.Abil.AC);
            var dwMP = HUtil32.MakeLong(hd.Abil.MP, hd.Abil.MAC);
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var strSql = new StringBuilder();
            strSql.AppendLine("UPDATE TBL_CHARACTER SET FLD_ServerNum = @FLD_ServerNum, FLD_LoginID = @FLD_LoginID,FLD_MapName = @FLD_MapName, FLD_CX = @FLD_CX, FLD_CY = @FLD_CY, FLD_Level = @FLD_Level, FLD_Dir = @FLD_Dir, FLD_Hair = @FLD_Hair, FLD_Sex = @FLD_Sex, FLD_Job = FLD_Job, FLD_Gold = @FLD_Gold, ");
            strSql.AppendLine("FLD_GamePoint = @FLD_GamePoint, FLD_HomeMap = @FLD_HomeMap, FLD_HomeX = @FLD_HomeX, FLD_HomeY = @FLD_HomeY, FLD_PkPoint = @FLD_PkPoint, FLD_ReLevel = @FLD_ReLevel, FLD_AttatckMode = @FLD_AttatckMode, FLD_FightZoneDieCount = @FLD_FightZoneDieCount, FLD_BodyLuck = @FLD_BodyLuck, FLD_IncHealth = @FLD_IncHealth, FLD_IncSpell = @FLD_IncSpell,");
            strSql.AppendLine("FLD_IncHealing = @FLD_IncHealing, FLD_CreditPoint = @FLD_CreditPoint, FLD_BonusPoint =@FLD_BonusPoint, FLD_HungerStatus =@FLD_HungerStatus, FLD_PayMentPoint = @FLD_PayMentPoint, FLD_LockLogon = @FLD_LockLogon, FLD_MarryCount = @FLD_MarryCount, FLD_AllowGroupReCall = @FLD_AllowGroupReCall, ");
            strSql.AppendLine("FLD_GroupRcallTime = @FLD_GroupRcallTime, FLD_AllowGuildReCall = @FLD_AllowGuildReCall, FLD_IsMaster = @FLD_IsMaster, FLD_MasterName = @FLD_MasterName, FLD_DearName = @FLD_DearName, FLD_StoragePwd = @FLD_StoragePwd, FLD_Deleted = @FLD_Deleted,FLD_LASTUPDATE = now() WHERE ID = @ID;");
            var command = new MySqlCommand();
            command.CommandText = strSql.ToString();
            command.Connection = dbConnection;
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@FLD_ServerNum", 1);
            command.Parameters.AddWithValue("@FLD_LoginID", 1);
            command.Parameters.AddWithValue("@FLD_MapName", hd.sCurMap);
            command.Parameters.AddWithValue("@FLD_CX", hd.wCurX);
            command.Parameters.AddWithValue("@FLD_CY", hd.wCurY);
            command.Parameters.AddWithValue("@FLD_Level", hd.Abil.Level);
            command.Parameters.AddWithValue("@FLD_Dir", hd.btDir);
            command.Parameters.AddWithValue("@FLD_Hair", hd.btHair);
            command.Parameters.AddWithValue("@FLD_Sex", hd.btSex);
            command.Parameters.AddWithValue("@FLD_Job", hd.btJob);
            command.Parameters.AddWithValue("@FLD_Gold", hd.nGold);
            command.Parameters.AddWithValue("@FLD_GamePoint", hd.nGamePoint);
            command.Parameters.AddWithValue("@FLD_HomeMap", hd.sHomeMap);
            command.Parameters.AddWithValue("@FLD_HomeX", hd.wHomeX);
            command.Parameters.AddWithValue("@FLD_HomeY", hd.wHomeY);
            command.Parameters.AddWithValue("@FLD_PkPoint", hd.nPKPoint);
            command.Parameters.AddWithValue("@FLD_ReLevel", hd.btReLevel);
            command.Parameters.AddWithValue("@FLD_AttatckMode", hd.btAttatckMode);
            command.Parameters.AddWithValue("@FLD_FightZoneDieCount", hd.btFightZoneDieCount);
            command.Parameters.AddWithValue("@FLD_BodyLuck", hd.dBodyLuck);
            command.Parameters.AddWithValue("@FLD_IncHealth", hd.btIncHealth);
            command.Parameters.AddWithValue("@FLD_IncSpell", hd.btIncSpell);
            command.Parameters.AddWithValue("@FLD_IncHealing", hd.btIncHealing);
            command.Parameters.AddWithValue("@FLD_CreditPoint", hd.btCreditPoint);
            command.Parameters.AddWithValue("@FLD_BonusPoint", hd.nBonusPoint);
            command.Parameters.AddWithValue("@FLD_HungerStatus", hd.nHungerStatus);
            command.Parameters.AddWithValue("@FLD_PayMentPoint", hd.nPayMentPoint);
            command.Parameters.AddWithValue("@FLD_LockLogon", hd.boLockLogon);
            command.Parameters.AddWithValue("@FLD_MarryCount", hd.MarryCount);
            command.Parameters.AddWithValue("@FLD_AllowGroupReCall", hd.btAllowGroup);
            command.Parameters.AddWithValue("@FLD_GroupRcallTime", hd.wGroupRcallTime);
            command.Parameters.AddWithValue("@FLD_AllowGuildReCall", hd.boAllowGuildReCall);
            command.Parameters.AddWithValue("@FLD_IsMaster", hd.boMaster);
            command.Parameters.AddWithValue("@FLD_MasterName", hd.sMasterName);
            command.Parameters.AddWithValue("@FLD_DearName", hd.sDearName);
            command.Parameters.AddWithValue("@FLD_StoragePwd", hd.sStoragePwd);
            command.Parameters.AddWithValue("@FLD_Deleted", 0);
            command.Parameters.AddWithValue("@Id", Id);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateRecord:" + ex.Message);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void UpdateAblity(int playerId, THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var hd = HumanRCD.Data;
            var strSql = new StringBuilder();
            strSql.AppendLine(" UPDATE TBL_CHARACTER_ABLITY SET FLD_Level = @FLD_Level,");
            strSql.AppendLine("FLD_Ac = @FLD_Ac, FLD_Mac = @FLD_Mac, FLD_Dc = @FLD_Dc, FLD_Mc = @FLD_Mc, FLD_Sc = @FLD_Sc, FLD_Hp = @FLD_Hp, FLD_Mp = @FLD_Mp, FLD_MaxHP = @FLD_MaxHP,");
            strSql.AppendLine("FLD_MAxMP = @FLD_MAxMP, FLD_Exp = @FLD_Exp, FLD_MaxExp = @FLD_MaxExp, FLD_Weight = @FLD_Weight, FLD_MaxWeight = @FLD_MaxWeight, FLD_WearWeight = @FLD_WearWeight,");
            strSql.AppendLine("FLD_MaxWearWeight = @FLD_MaxWearWeight, FLD_HandWeight = @FLD_HandWeight, FLD_MaxHandWeight = @FLD_MaxHandWeight,FLD_ModifyTime=now() WHERE FLD_PLAYERID = @FLD_PLAYERID;");
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            command.CommandText = strSql.ToString();
            command.Parameters.AddWithValue("@FLD_PLAYERID", playerId);
            command.Parameters.AddWithValue("@FLD_Level", hd.Abil.Level);
            command.Parameters.AddWithValue("@FLD_Ac", hd.Abil.Level);
            command.Parameters.AddWithValue("@FLD_Mac", hd.Abil.MAC);
            command.Parameters.AddWithValue("@FLD_Dc", hd.Abil.DC);
            command.Parameters.AddWithValue("@FLD_Mc", hd.Abil.MC);
            command.Parameters.AddWithValue("@FLD_Sc", hd.Abil.SC);
            command.Parameters.AddWithValue("@FLD_Hp", hd.Abil.HP);
            command.Parameters.AddWithValue("@FLD_Mp", hd.Abil.MP);
            command.Parameters.AddWithValue("@FLD_MaxHP", hd.Abil.MaxHP);
            command.Parameters.AddWithValue("@FLD_MAxMP", hd.Abil.MaxMP);
            command.Parameters.AddWithValue("@FLD_Exp", hd.Abil.Exp);
            command.Parameters.AddWithValue("@FLD_MaxExp", hd.Abil.MaxExp);
            command.Parameters.AddWithValue("@FLD_Weight", hd.Abil.Weight);
            command.Parameters.AddWithValue("@FLD_MaxWeight", hd.Abil.MaxWeight);
            command.Parameters.AddWithValue("@FLD_WearWeight", hd.Abil.WearWeight);
            command.Parameters.AddWithValue("@FLD_MaxWearWeight", hd.Abil.MaxWearWeight);
            command.Parameters.AddWithValue("@FLD_HandWeight", hd.Abil.HandWeight);
            command.Parameters.AddWithValue("@FLD_MaxHandWeight", hd.Abil.MaxHandWeight);
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateRecord (DELETE TBL_ITEM)");
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void UpdateItem(int playerId, THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            command.CommandText = @$"UPDATE tbl_item SET FLD_POSITION = 0, FLD_MAKEINDEX = 0, FLD_STDINDEX = 0, FLD_DURA = 0, FLD_DURAMAX = 0, `FLD_VALUE0` = 0, FLD_VALUE1 = 0, 
            FLD_VALUE2 = 0, FLD_VALUE3 = 0, FLD_VALUE4 = 0, FLD_VALUE5 = 0, FLD_VALUE6 = 0, FLD_VALUE7 = 0, FLD_VALUE8 = 0, FLD_VALUE9 = 0, 
            FLD_VALUE10 = 0, FLD_VALUE11 = 0, FLD_VALUE12 = 0, FLD_VALUE13 = 0 WHERE FLD_PLAYERID={playerId};";
            
            try
            {
                var hd = HumanRCD.Data;
                var strSql = new StringBuilder();
                strSql.AppendLine("UPDATE tbl_item SET FLD_CHARNAME = @FLD_CHARNAME, FLD_POSITION = @FLD_POSITION, FLD_MAKEINDEX = @FLD_MAKEINDEX, FLD_STDINDEX = @FLD_STDINDEX, FLD_DURA = @FLD_DURA, FLD_DURAMAX = @FLD_DURAMAX, ");
                strSql.AppendLine("FLD_VALUE0 = @FLD_VALUE0, FLD_VALUE1 = @FLD_VALUE1, FLD_VALUE2 = @FLD_VALUE2, FLD_VALUE3 = @FLD_VALUE3, FLD_VALUE4 = @FLD_VALUE4, FLD_VALUE5 = @FLD_VALUE5, FLD_VALUE6 = @FLD_VALUE6, FLD_VALUE7 = @FLD_VALUE7, ");
                strSql.AppendLine("FLD_VALUE8 = @FLD_VALUE8, FLD_VALUE9 = @FLD_VALUE9, FLD_VALUE10 = @FLD_VALUE10, FLD_VALUE11 = @FLD_VALUE11, FLD_VALUE12 = @FLD_VALUE12, FLD_VALUE13 = @FLD_VALUE13 WHERE FLD_PLAYERID = @FLD_PLAYERID");
                for (var i = 0; i <= hd.BagItems.GetUpperBound(0); i++)
                {
                    if ((hd.BagItems[i].wIndex > 0) && (hd.BagItems[i].MakeIndex > 0))
                    {
                        command.CommandText = strSql.ToString();
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@FLD_PLAYERID", playerId);
                        command.Parameters.AddWithValue("@FLD_CHARNAME", hd.sCharName);
                        command.Parameters.AddWithValue("@FLD_POSITION", -1);
                        command.Parameters.AddWithValue("@FLD_MAKEINDEX", hd.BagItems[i].MakeIndex);
                        command.Parameters.AddWithValue("@FLD_STDINDEX", hd.BagItems[i].wIndex);
                        command.Parameters.AddWithValue("@FLD_DURA", hd.BagItems[i].Dura);
                        command.Parameters.AddWithValue("@FLD_DURAMAX", hd.BagItems[i].DuraMax);
                        command.Parameters.AddWithValue("@FLD_VALUE0", hd.BagItems[i].btValue[0]);
                        command.Parameters.AddWithValue("@FLD_VALUE1", hd.BagItems[i].btValue[1]);
                        command.Parameters.AddWithValue("@FLD_VALUE2", hd.BagItems[i].btValue[2]);
                        command.Parameters.AddWithValue("@FLD_VALUE3", hd.BagItems[i].btValue[3]);
                        command.Parameters.AddWithValue("@FLD_VALUE4", hd.BagItems[i].btValue[4]);
                        command.Parameters.AddWithValue("@FLD_VALUE5", hd.BagItems[i].btValue[5]);
                        command.Parameters.AddWithValue("@FLD_VALUE6", hd.BagItems[i].btValue[6]);
                        command.Parameters.AddWithValue("@FLD_VALUE7", hd.BagItems[i].btValue[7]);
                        command.Parameters.AddWithValue("@FLD_VALUE8", hd.BagItems[i].btValue[8]);
                        command.Parameters.AddWithValue("@FLD_VALUE9", hd.BagItems[i].btValue[9]);
                        command.Parameters.AddWithValue("@FLD_VALUE10", hd.BagItems[i].btValue[10]);
                        command.Parameters.AddWithValue("@FLD_VALUE11", hd.BagItems[i].btValue[11]);
                        command.Parameters.AddWithValue("@FLD_VALUE12", hd.BagItems[i].btValue[12]);
                        command.Parameters.AddWithValue("@FLD_VALUE13", hd.BagItems[i].btValue[13]);
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateRecord (INSERT TBL_ITEM)");
                            DBShare.MainOutMessage(ex.StackTrace);
                        }
                    }
                }

                for (var i = 0; i <= hd.HumItems.GetUpperBound(0); i++)
                {
                    if ((hd.HumItems[i].wIndex > 0) && (hd.HumItems[i].MakeIndex > 0))
                    {
                        command.CommandText = strSql.ToString();
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@FLD_PLAYERID", playerId);
                        command.Parameters.AddWithValue("@FLD_CHARNAME", hd.sCharName);
                        command.Parameters.AddWithValue("@FLD_POSITION", i);
                        command.Parameters.AddWithValue("@FLD_MAKEINDEX", hd.HumItems[i].MakeIndex);
                        command.Parameters.AddWithValue("@FLD_STDINDEX", hd.HumItems[i].wIndex);
                        command.Parameters.AddWithValue("@FLD_DURA", hd.HumItems[i].Dura);
                        command.Parameters.AddWithValue("@FLD_DURAMAX", hd.HumItems[i].DuraMax);
                        command.Parameters.AddWithValue("@FLD_VALUE0", hd.HumItems[i].btValue[0]);
                        command.Parameters.AddWithValue("@FLD_VALUE1", hd.HumItems[i].btValue[1]);
                        command.Parameters.AddWithValue("@FLD_VALUE2", hd.HumItems[i].btValue[2]);
                        command.Parameters.AddWithValue("@FLD_VALUE3", hd.HumItems[i].btValue[3]);
                        command.Parameters.AddWithValue("@FLD_VALUE4", hd.HumItems[i].btValue[4]);
                        command.Parameters.AddWithValue("@FLD_VALUE5", hd.HumItems[i].btValue[5]);
                        command.Parameters.AddWithValue("@FLD_VALUE6", hd.HumItems[i].btValue[6]);
                        command.Parameters.AddWithValue("@FLD_VALUE7", hd.HumItems[i].btValue[7]);
                        command.Parameters.AddWithValue("@FLD_VALUE8", hd.HumItems[i].btValue[8]);
                        command.Parameters.AddWithValue("@FLD_VALUE9", hd.HumItems[i].btValue[9]);
                        command.Parameters.AddWithValue("@FLD_VALUE10", hd.HumItems[i].btValue[10]);
                        command.Parameters.AddWithValue("@FLD_VALUE11", hd.HumItems[i].btValue[11]);
                        command.Parameters.AddWithValue("@FLD_VALUE12", hd.HumItems[i].btValue[12]);
                        command.Parameters.AddWithValue("@FLD_VALUE13", hd.HumItems[i].btValue[13]);
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateRecord (13)");
                        }
                    }
                }
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateRecord (DELETE TBL_ITEM)");
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void SaveItemStorge(int playerId, THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            command.CommandText = $"DELETE FROM TBL_Storages WHERE FLD_PLAYERID={playerId}";
            try
            {
                command.ExecuteNonQuery();
                var hd = HumanRCD.Data;

                var strSql = new StringBuilder();
                strSql.AppendLine("INSERT INTO TBL_Storages(FLD_PLAYERID, FLD_POSITION, FLD_MAKEINDEX, FLD_STDINDEX, FLD_DURA, FLD_DURAMAX,");
                strSql.AppendLine("FLD_VALUE0, FLD_VALUE1, FLD_VALUE2, FLD_VALUE3, FLD_VALUE4, FLD_VALUE5, FLD_VALUE6, FLD_VALUE7, FLD_VALUE8, FLD_VALUE9, FLD_VALUE10, FLD_VALUE11, FLD_VALUE12, FLD_VALUE13) ");
                strSql.AppendLine(" VALUES ");
                strSql.AppendLine("(@FLD_PLAYERID, @FLD_POSITION, @FLD_MAKEINDEX, @FLD_STDINDEX, @FLD_DURA, @FLD_DURAMAX,@FLD_VALUE0, @FLD_VALUE1, @FLD_VALUE2, @FLD_VALUE3, @FLD_VALUE4, @FLD_VALUE5,");
                strSql.AppendLine("@FLD_VALUE6, @FLD_VALUE7, @FLD_VALUE8, @FLD_VALUE9, @FLD_VALUE10, @FLD_VALUE11, @FLD_VALUE12, @FLD_VALUE13)");

                for (var i = 0; i <= hd.StorageItems.GetUpperBound(0); i++)
                {
                    if (hd.StorageItems[i] == null)
                    {
                        continue;
                    }
                    if ((hd.StorageItems[i].wIndex > 0) && (hd.StorageItems[i].MakeIndex > 0))
                    {
                        command.CommandText = strSql.ToString();
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@FLD_PLAYERID", playerId);
                        command.Parameters.AddWithValue("@FLD_POSITION", -1);
                        command.Parameters.AddWithValue("@FLD_MAKEINDEX", hd.BagItems[i].MakeIndex);
                        command.Parameters.AddWithValue("@FLD_STDINDEX", hd.BagItems[i].wIndex);
                        command.Parameters.AddWithValue("@FLD_DURA", hd.BagItems[i].Dura);
                        command.Parameters.AddWithValue("@FLD_DURAMAX", hd.BagItems[i].DuraMax);
                        command.Parameters.AddWithValue("@FLD_VALUE0", hd.BagItems[i].btValue[0]);
                        command.Parameters.AddWithValue("@FLD_VALUE1", hd.BagItems[i].btValue[1]);
                        command.Parameters.AddWithValue("@FLD_VALUE2", hd.BagItems[i].btValue[2]);
                        command.Parameters.AddWithValue("@FLD_VALUE3", hd.BagItems[i].btValue[3]);
                        command.Parameters.AddWithValue("@FLD_VALUE4", hd.BagItems[i].btValue[4]);
                        command.Parameters.AddWithValue("@FLD_VALUE5", hd.BagItems[i].btValue[5]);
                        command.Parameters.AddWithValue("@FLD_VALUE6", hd.BagItems[i].btValue[6]);
                        command.Parameters.AddWithValue("@FLD_VALUE7", hd.BagItems[i].btValue[7]);
                        command.Parameters.AddWithValue("@FLD_VALUE8", hd.BagItems[i].btValue[8]);
                        command.Parameters.AddWithValue("@FLD_VALUE9", hd.BagItems[i].btValue[9]);
                        command.Parameters.AddWithValue("@FLD_VALUE10", hd.BagItems[i].btValue[10]);
                        command.Parameters.AddWithValue("@FLD_VALUE11", hd.BagItems[i].btValue[11]);
                        command.Parameters.AddWithValue("@FLD_VALUE12", hd.BagItems[i].btValue[12]);
                        command.Parameters.AddWithValue("@FLD_VALUE13", hd.BagItems[i].btValue[13]);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateRecord");
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void SavePlayerMagic(int playerId, THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            command.CommandText = $"DELETE FROM TBL_CHARACTER_MAGIC WHERE FLD_PLAYERID={playerId}";
            try
            {
                command.ExecuteNonQuery();
                var hd = HumanRCD.Data;
                const string sStrSql = "INSERT INTO TBL_CHARACTER_MAGIC(FLD_PLAYERID, FLD_MAGICID, FLD_LEVEL, FLD_USEKEY, FLD_CURRTRAIN) VALUES (@FLD_PLAYERID, @FLD_MAGICID, @FLD_LEVEL, @FLD_USEKEY, @FLD_CURRTRAIN)";
                for (var i = 0; i < HumanRCD.Data.Magic.GetUpperBound(0); i++)
                {
                    if (HumanRCD.Data.Magic[i].wMagIdx > 0)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@FLD_PLAYERID", playerId);
                        command.Parameters.AddWithValue("@FLD_MAGICID", hd.Magic[i].wMagIdx);
                        command.Parameters.AddWithValue("@FLD_LEVEL", hd.Magic[i].btLevel);
                        command.Parameters.AddWithValue("@FLD_USEKEY", hd.Magic[i].btKey);
                        command.Parameters.AddWithValue("@FLD_CURRTRAIN", hd.Magic[i].nTranPoint);
                        command.CommandText = sStrSql;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.SavePlayerMagic");
                DBShare.MainOutMessage(ex.StackTrace);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void UpdateBonusability(int playerId, THumDataInfo HumanRCD)
        {
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var bonusAbil = HumanRCD.Data.BonusAbil;
            const string sSqlStr = "UPDATE TBL_BONUSABILITY SET FLD_AC=@FLD_AC, FLD_MAC=@FLD_MAC, FLD_DC=@FLD_DC, FLD_MC=@FLD_MC, FLD_SC=@FLD_SC, FLD_HP=@FLD_HP, FLD_MP=@FLD_MP, FLD_HIT=@FLD_HIT, FLD_SPEED=@FLD_SPEED, FLD_RESERVED=@FLD_RESERVED WHERE FLD_PLAYERID=@FLD_PLAYERID";
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            command.Parameters.AddWithValue("@FLD_PLAYERID", playerId);
            command.Parameters.AddWithValue("@FLD_AC", bonusAbil.AC);
            command.Parameters.AddWithValue("@FLD_MAC", bonusAbil.MAC);
            command.Parameters.AddWithValue("@FLD_DC", bonusAbil.DC);
            command.Parameters.AddWithValue("@FLD_MC", bonusAbil.MC);
            command.Parameters.AddWithValue("@FLD_SC", bonusAbil.SC);
            command.Parameters.AddWithValue("@FLD_HP", bonusAbil.HP);
            command.Parameters.AddWithValue("@FLD_MP", bonusAbil.MP);
            command.Parameters.AddWithValue("@FLD_HIT", bonusAbil.Hit);
            command.Parameters.AddWithValue("@FLD_SPEED", bonusAbil.Speed);
            command.Parameters.AddWithValue("@FLD_RESERVED", bonusAbil.X2);
            command.CommandText = sSqlStr;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateBonusability");
                DBShare.MainOutMessage(ex.StackTrace);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void UpdateQuest(int Id, THumDataInfo HumanRCD)
        {
            const string sSqlStr4 = "DELETE FROM TBL_QUEST WHERE FLD_PLAYERID='{0}'";
            const string sSqlStr5 = "INSERT INTO TBL_QUEST (FLD_PLAYERID, FLD_QUESTOPENINDEX, FLD_QUESTFININDEX, FLD_QUEST) VALUES(@FLD_PLAYERID, @FLD_QUESTOPENINDEX, @FLD_QUESTFININDEX, @FLD_QUEST)";
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            command.CommandText = string.Format(sSqlStr4, HumanRCD.Header.sName);
            try
            {
                command.ExecuteNonQuery();
                //command.CommandText = sSqlStr5;
                //Units.HumDB_SQL.dbQry.ParamByName("FLD_CHARNAME").Value = HumanRCD.Header.sName;
                //EDcode.Encode6BitBuf(HumanRCD.Data.QuestUnitOpen, TempBuf, sizeof(HumanRCD.Data.QuestUnitOpen), sizeof(TempBuf));
                //Units.HumDB_SQL.dbQry.ParamByName("FLD_QUESTOPENINDEX").Value = TempBuf;
                //EDcode.Encode6BitBuf(HumanRCD.Data.QuestUnit, TempBuf, sizeof(HumanRCD.Data.QuestUnit), sizeof(TempBuf));
                //Units.HumDB_SQL.dbQry.ParamByName("FLD_QUESTFININDEX").Value = TempBuf;
                //EDcode.Encode6BitBuf(HumanRCD.Data.QuestFlag, TempBuf, sizeof(HumanRCD.Data.QuestFlag), sizeof(TempBuf));
                //Units.HumDB_SQL.dbQry.ParamByName("FLD_QUEST").Value = TempBuf;
                //Units.HumDB_SQL.dbQry.Execute;
            }
            catch
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateQuest");
            }
            finally
            {
                Close(dbConnection);
            }
        }

        private void UpdateStatus(int playerId, THumDataInfo HumanRCD)
        {
            const string sSqlStr4 = "DELETE FROM TBL_CHARACTER_STATUS WHERE FLD_PlayerId={0}";
            const string sSqlStr5 = "INSERT INTO TBL_CHARACTER_STATUS (FLD_PlayerId, FLD_CharName, FLD_Status) VALUES(@FLD_PlayerId, @FLD_CharName, @FLD_Status)";
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return;
            }
            var command = new MySqlCommand();
            command.Connection = dbConnection;
            command.CommandText = string.Format(sSqlStr4, playerId);
            try
            {
                command.ExecuteNonQuery();
                command.CommandText = sSqlStr5;
                command.Parameters.AddWithValue("@FLD_PLAYERID", playerId);
                command.Parameters.AddWithValue("@FLD_CharName", HumanRCD.Data.sCharName);
                command.Parameters.AddWithValue("@FLD_Status", string.Join("/", HumanRCD.Data.wStatusTimeArr));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DBShare.MainOutMessage("[Exception] MySqlHumDB.UpdateStatus (INSERT TBL_CHARACTER_STATUS)");
                DBShare.MainOutMessage(ex.StackTrace);
            }
            finally
            {
                Close(dbConnection);
            }
        }

        public int Find(string sChrName, StringDictionary List)
        {
            int result;
            for (var i = 0; i < MirQuickList.Count; i++)
            {
                //if (HUtil32.CompareLStr(m_MirQuickList[i], sChrName, sChrName.Length))
                //{
                //    List.Add(m_MirQuickList[i], m_MirQuickList.Values[i]);
                //}
            }
            result = List.Count;
            return result;
        }

        public bool Delete(int nIndex)
        {
            for (var i = 0; i < MirQuickList.Count; i++)
            {
                //if (((int)m_MirQuickList.Values[i]) == nIndex)
                //{
                //    s14 = m_MirQuickList[i];
                //    if (DeleteRecord(nIndex))
                //    {
                //        m_MirQuickList.Remove(i);
                //        result = true;
                //        break;
                //    }
                //}
            }
            return false;
        }

        private bool DeleteRecord(int nIndex)
        {
            var result = true;
            var PlayerId = QuickIndexIdList[nIndex];
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return false;
            }
            var command = new MySqlCommand();
            command.CommandText = $"UPDATE TBL_CHARACTER SET FLD_DELETED=1, FLD_CREATEDATE=now() WHERE ID={PlayerId}";
            command.Connection = dbConnection;
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                result = false;
                DBShare.MainOutMessage("[Exception] MySqlHumDB.DeleteRecord");
            }
            finally
            {
                Close(dbConnection);
            }
            return result;
        }

        public int Count()
        {
            return MirQuickList.Count;
        }

        public bool Delete(string sChrName)
        {
            //int nIndex = m_MirQuickList.GetIndex(sChrName);
            //if (nIndex < 0)
            //{
            //    return result;
            //}
            //if (DeleteRecord(nIndex))
            //{
            //    m_MirQuickList.Remove(nIndex);
            //    result = true;
            //}
            return false;
        }

        public int GetQryChar(int nIndex, ref TQueryChr QueryChrRcd)
        {
            var result = -1;
            const string sSQL = "SELECT * FROM TBL_CHARACTER WHERE ID={0}";
            if (nIndex < 0)
            {
                return result;
            }
            if (QuickIndexIdList.Count <= nIndex)
            {
                return result;
            }
            var PlayerId = QuickIndexIdList[nIndex];
            var success = false;
            var dbConnection = Open(ref success);
            if (!success)
            {
                return -1;
            }
            try
            {
                var command = new MySqlCommand();
                try
                {
                    command.CommandText = string.Format(sSQL, PlayerId);
                    command.Connection = dbConnection;
                    using var dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        QueryChrRcd.sName = dr.GetString("FLD_CHARNAME");
                        QueryChrRcd.btJob = dr.GetByte("FLD_JOB");
                        QueryChrRcd.btHair = dr.GetByte("FLD_HAIR");
                        QueryChrRcd.btSex = dr.GetByte("FLD_SEX");
                        QueryChrRcd.wLevel = dr.GetUInt16("FLD_LEVEL");
                    }
                }
                catch (Exception)
                {
                    DBShare.MainOutMessage("[Exception] MySqlHumDB.GetQryChar (1)");
                    return result;
                }
            }
            finally
            {
                Close(dbConnection);
            }
            result = nIndex;
            return result;
        }
    }
}