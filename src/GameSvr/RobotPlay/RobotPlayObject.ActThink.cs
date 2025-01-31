using System.Collections;
using SystemModule;

namespace GameSvr
{
    public partial class RobotPlayObject
    {
        public TMapWalkXY ActThink_FindGoodPathA(TMapWalkXY[] WalkStep, int nRange, int nType)
        {
            TMapWalkXY result = null;
            int n10 = int.MaxValue;
            int nMastrRange;
            int nMonCount;
            TMapWalkXY MapWalkXY = null;
            TMapWalkXY MapWalkXYA = null;
            //FillChar(result, sizeof(TMapWalkXY), 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (WalkStep[i].nWalkStep > 0 && Math.Abs(WalkStep[i].nX - m_TargetCret.m_nCurrX) >= nRange && Math.Abs(WalkStep[i].nY - m_TargetCret.m_nCurrY) >= nRange)
                {
                    if (WalkStep[i].nMonCount < n10)
                    {
                        n10 = WalkStep[i].nMonCount;
                        MapWalkXY = WalkStep[i];
                    }
                }
            }
            if (MapWalkXY != null && m_Master != null)
            {
                nMonCount = MapWalkXY.nMonCount;
                nMastrRange = MapWalkXY.nMastrRange;
                n10 = int.MaxValue;
                MapWalkXYA = MapWalkXY;
                MapWalkXY = null;
                for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
                {
                    if (WalkStep[i].nWalkStep > 0 && WalkStep[i].nMonCount <= nMonCount && Math.Abs(WalkStep[i].nX - m_TargetCret.m_nCurrX) >= nRange && Math.Abs(WalkStep[i].nY - m_TargetCret.m_nCurrY) >= nRange)
                    {
                        if (WalkStep[i].nMastrRange < n10 && WalkStep[i].nMastrRange < nMastrRange)
                        {
                            n10 = WalkStep[i].nMastrRange;
                            MapWalkXY = WalkStep[i];
                        }
                    }
                }
                if (MapWalkXY == null)
                {
                    MapWalkXY = MapWalkXYA;
                }
            }
            if (MapWalkXY != null)
            {
                result = MapWalkXY;
            }
            return result;
        }

        public TMapWalkXY ActThink_FindGoodPathB(TMapWalkXY[] WalkStep, int nType)
        {
            TMapWalkXY result = null;
            int nMastrRange;
            int nMonCount;
            TMapWalkXY MapWalkXY = null;
            int n10 = int.MaxValue;
            //FillChar(result, sizeof(TMapWalkXY), 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (WalkStep[i].nWalkStep > 0)
                {
                    if (WalkStep[i].nMonCount < n10)
                    {
                        n10 = WalkStep[i].nMonCount;
                        MapWalkXY = WalkStep[i];
                    }
                }
            }
            if (MapWalkXY != null && m_Master != null)
            {
                nMonCount = MapWalkXY.nMonCount;
                nMastrRange = MapWalkXY.nMastrRange;
                n10 = int.MaxValue;
                TMapWalkXY MapWalkXYA = MapWalkXY;
                MapWalkXY = null;
                for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
                {
                    if (WalkStep[i].nWalkStep > 0 && WalkStep[i].nMonCount <= nMonCount)
                    {
                        if (WalkStep[i].nMastrRange < n10 && WalkStep[i].nMastrRange < nMastrRange)
                        {
                            n10 = WalkStep[i].nMastrRange;
                            MapWalkXY = WalkStep[i];
                        }
                    }
                }
                if (MapWalkXY == null)
                {
                    MapWalkXY = MapWalkXYA;
                }
            }
            if (MapWalkXY != null)
            {
                result = MapWalkXY;
            }
            return result;
        }

        public TMapWalkXY ActThink_FindMinRange(TMapWalkXY[] WalkStep)
        {
            TMapWalkXY result = null;
            int n10 = int.MaxValue;
            int n1C;
            int nMonCount;
            TMapWalkXY MapWalkXY = null;
            TMapWalkXY MapWalkXYA = null;
            //FillChar(result, sizeof(TMapWalkXY), 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (WalkStep[i].nWalkStep > 0)
                {
                    n1C = Math.Abs(WalkStep[i].nX - m_TargetCret.m_nCurrX) + Math.Abs(WalkStep[i].nY - m_TargetCret.m_nCurrY);
                    if (n1C < n10)
                    {
                        n10 = n1C;
                        MapWalkXY = WalkStep[i];
                    }
                }
            }
            if (MapWalkXY != null)
            {
                nMonCount = MapWalkXY.nMonCount;
                MapWalkXYA = MapWalkXY;
                MapWalkXY = null;
                for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
                {
                    if (WalkStep[i].nWalkStep > 0 && WalkStep[i].nMonCount <= nMonCount)
                    {
                        n1C = Math.Abs(WalkStep[i].nX - m_TargetCret.m_nCurrX) + Math.Abs(WalkStep[i].nY - m_TargetCret.m_nCurrY);
                        if (n1C <= n10)
                        {
                            n10 = n1C;
                            MapWalkXY = WalkStep[i];
                        }
                    }
                }
                if (MapWalkXY == null)
                {
                    MapWalkXY = MapWalkXYA;
                }
            }
            if (MapWalkXY != null)
            {
                result = MapWalkXY;
            }
            return result;
        }

        public bool ActThink_CanWalkNextPosition(short nX, short nY, int nRange, byte btDir, bool boFlag)
        {
            // 检测下一步在不在攻击位
            short nCurrX = 0;
            short nCurrY = 0;
            bool result = false;
            if (m_PEnvir.GetNextPosition(nX, nY, btDir, 1, ref nCurrX, ref nCurrY) && CanMove(nX, nY, nCurrX, nCurrY, false) && !boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir))
            {
                result = true;
                return result;
            }
            if (m_PEnvir.GetNextPosition(nX, nY, btDir, 2, ref nCurrX, ref nCurrY) && CanMove(nX, nY, nCurrX, nCurrY, false) && !boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir))
            {
                result = true;
                return result;
            }
            return result;
        }

        public bool ActThink_FindPosOfSelf(TMapWalkXY[] WalkStep, int nRange, bool boFlag)
        {
            bool result = false;
            byte btDir = 0;
            short nCurrX = 0;
            short nCurrY = 0;
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 8, 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, i, nRange, ref nCurrX, ref nCurrY) && CanMove(nCurrX, nCurrY, false))
                {
                    if (!boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir))
                    {
                        WalkStep[i].nWalkStep = nRange;
                        WalkStep[i].nX = nCurrX;
                        WalkStep[i].nY = nCurrY;
                        WalkStep[i].nMonRange = Math.Abs(nCurrX - m_TargetCret.m_nCurrX) + Math.Abs(nCurrY - m_TargetCret.m_nCurrY);
                        WalkStep[i].nMonCount = GetNearTargetCount(nCurrX, nCurrY);
                        WalkStep[i].nMastrRange = GetMasterRange(nCurrX, nCurrY);
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool ActThink__FindPosOfSelf(TMapWalkXY[] WalkStep, int nRange, bool boFlag)
        {
            byte btDir = 0;
            short nCurrX = 0;
            short nCurrY = 0;
            bool result = false;
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 8, 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, i, nRange, ref nCurrX, ref nCurrY) && CanMove(nCurrX, nCurrY, false))
                {
                    if (!boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir) || ActThink_CanWalkNextPosition(nCurrX, nCurrY, nRange, (byte)i, boFlag))
                    {
                        WalkStep[i].nWalkStep = nRange;
                        WalkStep[i].nX = nCurrX;
                        WalkStep[i].nY = nCurrY;
                        WalkStep[i].nMonRange = Math.Abs(nCurrX - m_TargetCret.m_nCurrX) + Math.Abs(nCurrY - m_TargetCret.m_nCurrY);
                        WalkStep[i].nMonCount = GetNearTargetCount(nCurrX, nCurrY);
                        WalkStep[i].nMastrRange = GetMasterRange(nCurrX, nCurrY);
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool ActThink_FindPosOfTarget(TMapWalkXY[] WalkStep, short nTargetX, short nTargetY, int nRange, bool boFlag)
        {
            bool result = false;
            byte btDir = 0;
            short nCurrX = 0;
            short nCurrY = 0;
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 8, 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (m_PEnvir.GetNextPosition(nTargetX, nTargetY, i, nRange, ref nCurrX, ref nCurrY) && m_PEnvir.CanWalkEx(nCurrX, nCurrY, false))
                {
                    if ((!boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir)) && IsGotoXY(m_nCurrX, m_nCurrY, nCurrX, nCurrY))
                    {
                        WalkStep[i].nWalkStep = nRange;
                        WalkStep[i].nX = nCurrX;
                        WalkStep[i].nY = nCurrY;
                        WalkStep[i].nMonRange = Math.Abs(nCurrX - nTargetX) + Math.Abs(nCurrY - nTargetY);
                        WalkStep[i].nMastrRange = GetMasterRange(nCurrX, nCurrY);
                        WalkStep[i].nMonCount = GetRangeTargetCount(nCurrX, nCurrY, 2);
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool ActThink_FindPos(TMapWalkXY[] WalkStep, int nRange, bool boFlag)
        {
            bool result = false;
            byte btDir = 0;
            short nCurrX = 0;
            short nCurrY = 0;
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 8, 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, i, 2, ref nCurrX, ref nCurrY) && CanMove(nCurrX, nCurrY, false))
                {
                    if (!boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir))
                    {
                        WalkStep[i].nWalkStep = nRange;
                        WalkStep[i].nX = nCurrX;
                        WalkStep[i].nY = nCurrY;
                        WalkStep[i].nMonRange = Math.Abs(nCurrX - m_TargetCret.m_nCurrX) + Math.Abs(nCurrY - m_TargetCret.m_nCurrY);
                        WalkStep[i].nMonCount = GetNearTargetCount(nCurrX, nCurrY);
                        WalkStep[i].nMastrRange = GetMasterRange(nCurrX, nCurrY);
                        result = true;
                    }
                }
            }
            if (result)
            {
                return result;
            }
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 8, 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, i, 1, ref nCurrX, ref nCurrY) && CanMove(nCurrX, nCurrY, false))
                {
                    if (!boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir))
                    {
                        WalkStep[i].nWalkStep = nRange;
                        WalkStep[i].nX = nCurrX;
                        WalkStep[i].nY = nCurrY;
                        WalkStep[i].nMonRange = Math.Abs(nCurrX - m_TargetCret.m_nCurrX) + Math.Abs(nCurrY - m_TargetCret.m_nCurrY);
                        WalkStep[i].nMonCount = GetNearTargetCount(nCurrX, nCurrY);
                        WalkStep[i].nMastrRange = GetMasterRange(nCurrX, nCurrY);
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool ActThink__FindPos(TMapWalkXY[] WalkStep, int nRange, bool boFlag)
        {
            bool result = false;
            byte btDir = 0;
            short nCurrX = 0;
            short nCurrY = 0;
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 8, 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, i, 1, ref nCurrX, ref nCurrY) && CanMove(nCurrX, nCurrY, false))
                {
                    if (!boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir) || ActThink_CanWalkNextPosition(nCurrX, nCurrY, nRange, (byte)i, boFlag))
                    {
                        WalkStep[i].nWalkStep = nRange;
                        WalkStep[i].nX = nCurrX;
                        WalkStep[i].nY = nCurrY;
                        WalkStep[i].nMonRange = Math.Abs(nCurrX - m_TargetCret.m_nCurrX) + Math.Abs(nCurrY - m_TargetCret.m_nCurrY);
                        WalkStep[i].nMonCount = GetNearTargetCount(nCurrX, nCurrY);
                        WalkStep[i].nMastrRange = GetMasterRange(nCurrX, nCurrY);
                        result = true;
                    }
                }
            }
            if (result)
            {
                return result;
            }
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 8, 0);
            for (var i = Grobal2.DR_UP; i <= Grobal2.DR_UPLEFT; i++)
            {
                if (m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, i, 2, ref nCurrX, ref nCurrY) && CanMove(nCurrX, nCurrY, false))
                {
                    if (!boFlag || CanAttack(nCurrX, nCurrY, m_TargetCret, nRange, ref btDir) || ActThink_CanWalkNextPosition(nCurrX, nCurrY, nRange, (byte)i, boFlag))
                    {
                        WalkStep[i].nWalkStep = nRange;
                        WalkStep[i].nX = nCurrX;
                        WalkStep[i].nY = nCurrY;
                        WalkStep[i].nMonRange = Math.Abs(nCurrX - m_TargetCret.m_nCurrX) + Math.Abs(nCurrY - m_TargetCret.m_nCurrY);
                        WalkStep[i].nMonCount = GetNearTargetCount(nCurrX, nCurrY);
                        WalkStep[i].nMastrRange = GetMasterRange(nCurrX, nCurrY);
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool ActThink_WalkToRightPos(int wMagicID)
        {
            bool boFlag;
            int nRange;
            TMapWalkXY[] WalkStep = null;
            TMapWalkXY MapWalkXY;
            bool result = false;
            try
            {
                boFlag = m_btRaceServer == 108 || new ArrayList(new int[] { SpellsDef.SKILL_FIREBALL, SpellsDef.SKILL_FIREBALL2, SpellsDef.SKILL_FIRECHARM }).Contains(wMagicID) || m_btJob == 0;
                if (m_btJob == 0 || wMagicID <= 0)
                {
                    nRange = 1;
                    if (wMagicID == 43)
                    {
                        nRange = 4;
                    }
                    if (wMagicID == 12)
                    {
                        nRange = 2;
                    }
                    if (new ArrayList(new int[] { 60, 61, 62 }).Contains(wMagicID))
                    {
                        nRange = 6;
                    }
                    for (var i = nRange; i >= 1; i--)
                    {
                        if (ActThink_FindPosOfTarget(WalkStep, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, i, boFlag))
                        {
                            MapWalkXY = ActThink_FindGoodPathB(WalkStep, 0);
                            if (MapWalkXY.nWalkStep > 0)
                            {
                                // if RunToTargetXY(MapWalkXY.nX, MapWalkXY.nY) then begin
                                if (GotoNext(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                                {
                                    m_RunPos.btDirection = 0;
                                    result = true;
                                    return result;
                                }
                            }
                        }
                    }
                    for (var i = 2; i >= 1; i--)
                    {
                        if (ActThink_FindPosOfSelf(WalkStep, i, boFlag))
                        {
                            if (m_Master != null)
                            {
                                MapWalkXY = ActThink_FindGoodPathB(WalkStep, 1);
                            }
                            else
                            {
                                MapWalkXY = ActThink_FindGoodPathB(WalkStep, 0);
                            }
                            if (MapWalkXY.nWalkStep > 0)
                            {
                                // if RunToTargetXY(MapWalkXY.nX, MapWalkXY.nY) then begin
                                if (GotoNext(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                                {
                                    m_RunPos.btDirection = 0;
                                    result = true;
                                    return result;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (wMagicID > 0)
                    {
                        nRange = HUtil32._MAX(M2Share.RandomNumber.Random(3), 2);
                    }
                    else
                    {
                        nRange = 1;
                    }
                    boFlag = m_btRaceServer == 108 || new ArrayList(new int[] { SpellsDef.SKILL_FIREBALL, SpellsDef.SKILL_FIREBALL2, SpellsDef.SKILL_FIRECHARM }).Contains(wMagicID) || nRange == 1;
                    for (var i = 2; i >= 1; i--)
                    {
                        if (ActThink_FindPosOfSelf(WalkStep, i, boFlag))
                        {
                            MapWalkXY = ActThink_FindGoodPathA(WalkStep, nRange, 0);
                            if (MapWalkXY.nWalkStep > 0)
                            {
                                if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                                {
                                    m_RunPos.btDirection = 0;
                                    result = true;
                                    return result;
                                }
                            }
                        }
                    }
                    for (var i = 2; i >= 1; i--)
                    {
                        if (ActThink__FindPosOfSelf(WalkStep, i, boFlag))
                        {
                            MapWalkXY = ActThink_FindMinRange(WalkStep);
                            if (MapWalkXY.nWalkStep > 0)
                            {
                                if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                                {
                                    m_RunPos.btDirection = 0;
                                    result = true;
                                    return result;
                                }
                            }
                        }
                    }
                    for (var i = nRange; i >= 1; i--)
                    {
                        if (ActThink_FindPosOfTarget(WalkStep, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, i, boFlag))
                        {
                            MapWalkXY = ActThink_FindGoodPathB(WalkStep, 0);
                            if (MapWalkXY.nWalkStep > 0)
                            {
                                if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                                {
                                    m_RunPos.btDirection = 0;
                                    result = true;
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                M2Share.MainOutMessage("WalkToRightPos:" + m_sCharName);
            }
            return result;
        }

        public bool ActThink_AvoidTarget(short wMagicID)
        {
            short nX = 0;
            short nY = 0;
            bool boFlag;
            TMapWalkXY[] WalkStep = null;
            bool result = false;
            int nRange = HUtil32._MAX(new System.Random(3).Next(), 2);
            boFlag = m_btRaceServer == 108 || new ArrayList(new short[] { SpellsDef.SKILL_FIREBALL, SpellsDef.SKILL_FIREBALL2, SpellsDef.SKILL_FIRECHARM }).Contains(wMagicID);
            byte btDir;
            TMapWalkXY MapWalkXY;
            for (var i = nRange; i >= 1; i--)
            {
                if (ActThink_FindPosOfSelf(WalkStep, i, boFlag))
                {
                    MapWalkXY = ActThink_FindGoodPathB(WalkStep, 0);
                    if (MapWalkXY.nWalkStep > 0)
                    {
                        btDir = M2Share.GetNextDirection(m_nCurrX, m_nCurrY, MapWalkXY.nX, MapWalkXY.nY);
                        if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                        {
                            if (m_btRaceServer != 108)
                            {
                                for (var j = nRange; j >= 1; j--)
                                {
                                    // 再跑1次
                                    if (m_PEnvir.GetNextPosition(MapWalkXY.nX, MapWalkXY.nY, btDir, j, ref nX, ref nY) && m_PEnvir.CanWalkEx(nX, nY, true) && GetNearTargetCount(nX, nY) <= MapWalkXY.nMonCount)
                                    {
                                        GotoNextOne(nX, nY, m_btRaceServer != 108);
                                        break;
                                    }
                                }
                            }
                            m_RunPos.btDirection = 0;
                            result = true;
                            return result;
                        }
                    }
                }
            }
            for (var i = nRange; i >= 1; i--)
            {
                if (ActThink__FindPosOfSelf(WalkStep, i, boFlag))
                {
                    MapWalkXY = ActThink_FindGoodPathB(WalkStep, 0);
                    if (MapWalkXY.nWalkStep > 0)
                    {
                        btDir = M2Share.GetNextDirection(m_nCurrX, m_nCurrY, MapWalkXY.nX, MapWalkXY.nY);
                        if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                        {
                            for (var j = nRange; j >= 1; j--)
                            {
                                // 再跑1次
                                if (m_PEnvir.GetNextPosition(MapWalkXY.nX, MapWalkXY.nY, btDir, j, ref nX, ref nY) && m_PEnvir.CanWalkEx(nX, nY, true) && GetNearTargetCount(nX, nY) <= MapWalkXY.nMonCount)
                                {
                                    MapWalkXY.nX = nX;
                                    MapWalkXY.nY = nY;
                                    GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108);
                                    break;
                                }
                            }
                            m_RunPos.btDirection = 0;
                            result = true;
                            return result;
                        }
                    }
                }
            }
            return result;
        }

        public bool ActThink_FollowTarget(short wMagicID)
        {
            int nRange = 2;
            TMapWalkXY[] WalkStep = null;
            TMapWalkXY MapWalkXY;
            bool result = false;
            bool boFlag = m_btRaceServer == 108 || new ArrayList(new short[] { SpellsDef.SKILL_FIREBALL, SpellsDef.SKILL_FIREBALL2, SpellsDef.SKILL_FIRECHARM }).Contains(wMagicID);
            for (var i = nRange; i >= 1; i--)
            {
                if (ActThink_FindPosOfSelf(WalkStep, i, boFlag))
                {
                    MapWalkXY = ActThink_FindMinRange(WalkStep);
                    if (MapWalkXY.nWalkStep > 0)
                    {
                        if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                        {
                            m_RunPos.btDirection = 0;
                            result = true;
                            return result;
                        }
                    }
                }
            }
            for (var i = nRange; i >= 1; i--)
            {
                if (ActThink__FindPosOfSelf(WalkStep, i, boFlag))
                {
                    MapWalkXY = ActThink_FindMinRange(WalkStep);
                    if (MapWalkXY.nWalkStep > 0)
                    {
                        if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                        {
                            m_RunPos.btDirection = 0;
                            result = true;
                            return result;
                        }
                    }
                }
            }
            return result;
        }

        public bool ActThink_MotaeboPos(short wMagicID)
        {
            bool result = false;
            short nTargetX = 0;
            short nTargetY = 0;
            byte btNewDir;
            if (m_TargetCret == null || m_Master == null)
            {
                return result;
            }
            if (GetPoseCreate() == m_TargetCret || m_TargetCret.GetPoseCreate() == this)
            {
                btNewDir = M2Share.GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
                if (m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, btNewDir, 1, ref nTargetX, ref nTargetY))
                {
                    if (m_PEnvir.CanWalk(nTargetX, nTargetY, true))
                    {
                        result = true;
                        return result;
                    }
                }
            }
            result = ActThink_WalkToRightPos(wMagicID);
            return result;
        }

        public TMapWalkXY ActThink_FindPosOfDir(int nDir, int nRange, bool boFlag)
        {
            TMapWalkXY result = null;
            short nCurrX = 0;
            short nCurrY = 0;
            //FillChar(result, sizeof(TMapWalkXY), 0);
            if (m_PEnvir.GetNextPosition(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY, nDir, nRange, ref nCurrX, ref nCurrY) && CanMove(nCurrX, nCurrY, false) && (boFlag && CanLineAttack(nCurrX, nCurrY) || !boFlag) && IsGotoXY(m_nCurrX, m_nCurrY, nCurrX, nCurrY))
            {
                result = new TMapWalkXY();
                result.nWalkStep = nRange;
                result.nX = nCurrX;
                result.nY = nCurrY;
                result.nMonRange = Math.Abs(nCurrX - m_TargetCret.m_nCurrX) + Math.Abs(nCurrY - m_TargetCret.m_nCurrY);
                result.nMonCount = GetNearTargetCount(nCurrX, nCurrY);
                result.nMastrRange = GetMasterRange(nCurrX, nCurrY);
            }
            return result;
        }

        public byte ActThink_RunPosAttack_GetNextRunPos(byte btDir, bool boTurn)
        {
            byte result = 0;
            if (boTurn)
            {
                switch (btDir)
                {
                    case Grobal2.DR_UP:
                        result = Grobal2.DR_RIGHT;
                        break;
                    case Grobal2.DR_UPRIGHT:
                        result = Grobal2.DR_DOWNRIGHT;
                        break;
                    case Grobal2.DR_RIGHT:
                        result = Grobal2.DR_DOWN;
                        break;
                    case Grobal2.DR_DOWNRIGHT:
                        result = Grobal2.DR_DOWNLEFT;
                        break;
                    case Grobal2.DR_DOWN:
                        result = Grobal2.DR_LEFT;
                        break;
                    case Grobal2.DR_DOWNLEFT:
                        result = Grobal2.DR_UPLEFT;
                        break;
                    case Grobal2.DR_LEFT:
                        result = Grobal2.DR_UP;
                        break;
                    case Grobal2.DR_UPLEFT:
                        result = Grobal2.DR_UPRIGHT;
                        break;
                }
            }
            else
            {
                switch (btDir)
                {
                    case Grobal2.DR_UP:
                        result = Grobal2.DR_LEFT;
                        break;
                    case Grobal2.DR_UPRIGHT:
                        result = Grobal2.DR_UPLEFT;
                        break;
                    case Grobal2.DR_RIGHT:
                        result = Grobal2.DR_UP;
                        break;
                    case Grobal2.DR_DOWNRIGHT:
                        result = Grobal2.DR_UPRIGHT;
                        break;
                    case Grobal2.DR_DOWN:
                        result = Grobal2.DR_RIGHT;
                        break;
                    case Grobal2.DR_DOWNLEFT:
                        result = Grobal2.DR_DOWNRIGHT;
                        break;
                    case Grobal2.DR_LEFT:
                        result = Grobal2.DR_DOWN;
                        break;
                    case Grobal2.DR_UPLEFT:
                        result = Grobal2.DR_DOWNLEFT;
                        break;
                }
            }
            return result;
        }

        public bool ActThink_RunPosAttack(int wMagicID)
        {
            TMapWalkXY[] WalkStep = new TMapWalkXY[2];
            TMapWalkXY MapWalkXY;
            int nRange;
            bool boFlag;
            int nNearTargetCount;
            bool result = false;
            byte btDir = M2Share.GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
            byte btNewDir1 = ActThink_RunPosAttack_GetNextRunPos(btDir, true);
            byte btNewDir2 = ActThink_RunPosAttack_GetNextRunPos(btDir, false);
            //FillChar(WalkStep, sizeof(TMapWalkXY) * 2, 0);
            if (m_btJob == 0)
            {
                nRange = 1;
                if (wMagicID == 43)
                {
                    nRange = 2;
                }
                if (wMagicID == 12)
                {
                    nRange = 2;
                }
                if (new ArrayList(new int[] { 60, 61, 62 }).Contains(wMagicID))
                {
                    nRange = 6;
                }
                WalkStep[0] = ActThink_FindPosOfDir(btNewDir1, nRange, true);
                WalkStep[1] = ActThink_FindPosOfDir(btNewDir2, nRange, true);
            }
            else
            {
                nRange = 2;
                boFlag = false;
                WalkStep[0] = ActThink_FindPosOfDir(btNewDir1, nRange, boFlag);
                WalkStep[1] = ActThink_FindPosOfDir(btNewDir2, nRange, boFlag);
            }
            nNearTargetCount = GetNearTargetCount(m_nCurrX, m_nCurrY);
            MapWalkXY = null;
            if (WalkStep[0].nWalkStep > 0 && WalkStep[1].nWalkStep > 0)
            {
                if (m_RunPos.btDirection > 0)
                {
                    MapWalkXY = WalkStep[1];
                }
                else
                {
                    MapWalkXY = WalkStep[0];
                }
                if (nNearTargetCount < WalkStep[0].nMonCount && nNearTargetCount < WalkStep[1].nMonCount)
                {
                    MapWalkXY = null;
                }
                else if (m_RunPos.btDirection > 0 && nNearTargetCount < WalkStep[1].nMonCount)
                {
                    MapWalkXY = null;
                }
                else if (m_RunPos.btDirection <= 0 && nNearTargetCount < WalkStep[0].nMonCount)
                {
                    MapWalkXY = null;
                }
                if (nNearTargetCount > 0 && MapWalkXY != null && MapWalkXY.nMonCount > nNearTargetCount)
                {
                    MapWalkXY = null;
                }
            }
            else if (WalkStep[0].nWalkStep > 0)
            {
                MapWalkXY = WalkStep[0];
                if (nNearTargetCount < WalkStep[0].nMonCount)
                {
                    MapWalkXY = null;
                }
                m_RunPos.btDirection = 0;
            }
            else if (WalkStep[1].nWalkStep > 0)
            {
                MapWalkXY = WalkStep[1];
                if (nNearTargetCount < WalkStep[1].nMonCount)
                {
                    MapWalkXY = null;
                }
                m_RunPos.btDirection = 1;
            }
            if (MapWalkXY != null)
            {
                if (GotoNextOne(MapWalkXY.nX, MapWalkXY.nY, m_btRaceServer != 108))
                {
                    result = true;
                }
            }
            if (!result)
            {
                m_RunPos.nAttackCount = 0;
            }
            return result;
        }

        private bool ActThink(short wMagicID)
        {
            bool result = false;
            int nCode = 0;
            int nThinkCount = 0;
            try
            {
                while (true)
                {
                    if (m_TargetCret == null || wMagicID > 255)
                    {
                        break;
                    }
                    nThinkCount = nThinkCount + 1;
                    nCode = DoThink(wMagicID);
                    switch (m_btJob)
                    {
                        case 0:
                            switch (nCode)
                            {
                                case 2:
                                    if (ActThink_WalkToRightPos(wMagicID))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        DelTargetCreat();
                                        if (nThinkCount < 2)
                                        {
                                            SearchTarget();
                                            continue;
                                        }
                                    }
                                    break;
                                case 5:
                                    if (ActThink_RunPosAttack(wMagicID))
                                    {
                                        result = true;
                                    }

                                    break;
                            }
                            break;
                        case 1:
                        case 2:
                            switch (nCode)
                            {
                                case 1:
                                    result = ActThink_AvoidTarget(wMagicID);
                                    break;
                                case 2:
                                    if (ActThink_FollowTarget(wMagicID))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        DelTargetCreat();
                                        if (nThinkCount < 2)
                                        {
                                            SearchTarget();
                                            continue;
                                        }
                                    }
                                    break;
                                case 3:
                                case 4:
                                    if (ActThink_WalkToRightPos(wMagicID))
                                    {
                                        result = true;
                                    }
                                    else
                                    {
                                        DelTargetCreat();
                                        if (nThinkCount < 2)
                                        {
                                            SearchTarget();
                                            continue;
                                        }
                                    }

                                    break;
                                case 5:
                                    result = ActThink_RunPosAttack(wMagicID);
                                    break;
                            }
                            break;
                    }
                    break;
                }
            }
            catch
            {
                M2Share.MainOutMessage(format("TAIPlayObject::ActThink Name:{0} Code:{1} ", new object[] { m_sCharName, nCode }));
            }
            return result;
        }
    }
}