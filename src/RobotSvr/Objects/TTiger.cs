﻿using SystemModule;

namespace RobotSvr
{
    public class TTiger : TActor
    {
        protected int ax = 0;
        protected int ay = 0;
        protected byte firedir;

        public TTiger(RobotClient robotClient) : base(robotClient)
        {
            m_boUseEffect = false;
        }

        public override void Run()
        {
            long m_dwEffectframetimetime;
            if (m_nCurrentAction == Grobal2.SM_WALK || m_nCurrentAction == Grobal2.SM_BACKSTEP ||
                m_nCurrentAction == Grobal2.SM_RUN || m_nCurrentAction == Grobal2.SM_HORSERUN) return;
            if (m_boUseEffect)
            {
                m_dwEffectframetimetime = m_dwEffectFrameTime;
                if (MShare.GetTickCount() - m_dwEffectStartTime > m_dwEffectframetimetime)
                {
                    m_dwEffectStartTime = MShare.GetTickCount();
                    if (m_nEffectFrame < m_nEffectEnd)
                        m_nEffectFrame++;
                    else
                        m_boUseEffect = false;
                }
            }

            base.Run();
        }
    }
}