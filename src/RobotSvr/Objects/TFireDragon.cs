﻿using System.Collections;
using SystemModule;

namespace RobotSvr
{
    public class TFireDragon : TSkeletonArcherMon
    {
        public TFireDragon(RobotClient robotClient) : base(robotClient)
        {
            
        }

        public void LightningTimerTimer(object Sender)
        {
            //int tx;
            //int ty;
            //int kx;
            //int ky;
            //bool bofly;
            //if (this.m_btRace == 120)
            //{
            //    if (LightningTimer.Tag == 0)
            //    {
            //        LightningTimer.Tag = LightningTimer.Tag + 1;
            //        LightningTimer.Interval = 10;
            //        return;
            //    }
            //    tx = MShare.g_MySelf.m_nCurrX;
            //    ty = MShare.g_MySelf.m_nCurrY;
            //    kx =  RandomNumber.GetInstance().Random(7);
            //    ky =  RandomNumber.GetInstance().Random(5);
            //    if (LightningTimer.Tag == 0)
            //    {
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_1, Grobal2.MAGIC_SOULBALL_ATT3_1, this.m_nCurrX, this.m_nCurrY, tx, ty, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_2, Grobal2.MAGIC_SOULBALL_ATT3_2, this.m_nCurrX, this.m_nCurrY, tx - 2, ty, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_3, Grobal2.MAGIC_SOULBALL_ATT3_3, this.m_nCurrX, this.m_nCurrY, tx, ty - 2, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_4, Grobal2.MAGIC_SOULBALL_ATT3_4, this.m_nCurrX, this.m_nCurrY, tx - kx, ty - ky, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //        LightningTimer.Interval = 500;
            //    }
            //    else if (LightningTimer.Tag == 2)
            //    {
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_1, Grobal2.MAGIC_SOULBALL_ATT3_1, this.m_nCurrX, this.m_nCurrY, tx - 2, ty - 2, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_2, Grobal2.MAGIC_SOULBALL_ATT3_2, this.m_nCurrX, this.m_nCurrY, tx + 2, ty - 2, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_3, Grobal2.MAGIC_SOULBALL_ATT3_3, this.m_nCurrX, this.m_nCurrY, tx + kx, ty, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //        ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_4, Grobal2.MAGIC_SOULBALL_ATT3_4, this.m_nCurrX, this.m_nCurrY, tx - kx, ty, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //    }
            //    ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_5, Grobal2.MAGIC_SOULBALL_ATT3_5, this.m_nCurrX, this.m_nCurrY, tx + kx, ty - ky, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //    ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_1, Grobal2.MAGIC_SOULBALL_ATT3_1, this.m_nCurrX, this.m_nCurrY, tx - kx - 2, ty + ky, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //    ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_2, Grobal2.MAGIC_SOULBALL_ATT3_2, this.m_nCurrX, this.m_nCurrY, tx - kx, ty - ky, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //    ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_3, Grobal2.MAGIC_SOULBALL_ATT3_3, this.m_nCurrX, this.m_nCurrY, tx + kx + 2, ty + ky, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //    ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_4, Grobal2.MAGIC_SOULBALL_ATT3_4, this.m_nCurrX, this.m_nCurrY, tx + kx, ty, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //    ClMain.g_PlayScene.NewMagic(this, Grobal2.MAGIC_SOULBALL_ATT3_5, Grobal2.MAGIC_SOULBALL_ATT3_5, this.m_nCurrX, this.m_nCurrY, tx - kx, ty, 0, magiceff.TMagicType.mtThunder, false, 30, ref bofly);
            //    LightningTimer.Interval = LightningTimer.Interval + 100;
            //    LightningTimer.Tag = LightningTimer.Tag + 1;
            //    if (LightningTimer.Tag > 7)
            //    {
            //        LightningTimer.Interval = 10;
            //        LightningTimer.Tag = 0;
            //        LightningTimer.Enabled = false;
            //    }
            //}
        }

        public override void Run()
        {
            int prv;
            long m_dwEffectFrameTimetime;
            long m_dwFrameTimetime;
            if (m_btRace != 120 && m_boDeath) return;
            if (m_nCurrentAction == Grobal2.SM_WALK || m_nCurrentAction == Grobal2.SM_BACKSTEP ||
                m_nCurrentAction == Grobal2.SM_RUN || m_nCurrentAction == Grobal2.SM_HORSERUN) return;
            m_boMsgMuch = false;
            if (m_MsgList.Count >= MShare.MSGMUCH) m_boMsgMuch = true;
            if (m_boRunSound) m_boRunSound = false;
            if (m_boUseEffect)
            {
                if (m_boMsgMuch)
                    m_dwEffectFrameTimetime = HUtil32.Round(m_dwEffectFrameTime * 2 / 3);
                else
                    m_dwEffectFrameTimetime = m_dwEffectFrameTime;
                if (MShare.GetTickCount() - m_dwEffectStartTime > m_dwEffectFrameTimetime)
                {
                    m_dwEffectStartTime = MShare.GetTickCount();
                    if (m_nEffectFrame < m_nEffectEnd)
                    {
                        m_nEffectFrame++;
                    }
                    else
                    {
                        if (new ArrayList(new[] { 118, 119, 120 }).Contains(m_btRace))
                        {
                            if (m_boDeath)
                                m_boUseEffect = false;
                            else
                                m_boUseEffect = true;
                        }
                        else
                        {
                            m_boUseEffect = false;
                        }
                    }
                }
            }

            prv = m_nCurrentFrame;
            if (m_nCurrentAction != 0)
            {
                if (m_nCurrentFrame < m_nStartFrame || m_nCurrentFrame > m_nEndFrame) m_nCurrentFrame = m_nStartFrame;
                if (m_boMsgMuch)
                    m_dwFrameTimetime = HUtil32.Round(m_dwFrameTime * 2 / 3);
                else
                    m_dwFrameTimetime = m_dwFrameTime;
                if (MShare.GetTickCount() - m_dwStartTime > m_dwFrameTimetime)
                {
                    if (m_nCurrentFrame < m_nEndFrame)
                    {
                        m_nCurrentFrame++;
                        m_dwStartTime = MShare.GetTickCount();
                    }
                    else
                    {
                        m_nCurrentAction = 0;
                        m_boUseEffect = false;
                        m_boNowDeath = false;
                    }
                }
                m_nCurrentDefFrame = 0;
                m_dwDefFrameTime = MShare.GetTickCount();
            }
            else
            {
                if (m_btRace == 120)
                {
                    if (MShare.GetTickCount() - m_dwDefFrameTime > 150)
                    {
                        m_dwDefFrameTime = MShare.GetTickCount();
                        m_nCurrentDefFrame++;
                        if (m_nCurrentDefFrame >= m_nDefFrameCount) m_nCurrentDefFrame = 0;
                    }

                    DefaultMotion();
                }
                else if (MShare.GetTickCount() - m_dwSmoothMoveTime > 200)
                {
                    if (MShare.GetTickCount() - m_dwDefFrameTime > 500)
                    {
                        m_dwDefFrameTime = MShare.GetTickCount();
                        m_nCurrentDefFrame++;
                        if (m_nCurrentDefFrame >= m_nDefFrameCount) m_nCurrentDefFrame = 0;
                    }

                    DefaultMotion();
                }
            }

            if (prv != m_nCurrentFrame) m_dwLoadSurfaceTime = MShare.GetTickCount();
        }
    }
}