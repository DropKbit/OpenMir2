﻿using SystemModule;

namespace RobotSvr
{
    public class TRedThunderZuma : TGasKuDeGi
    {
        public bool boCasted;

        public TRedThunderZuma(RobotClient robotClient) : base(robotClient)
        {
            boCasted = false;
        }

        public override void Run()
        {
            if (m_nCurrentFrame - m_nStartFrame == 2)
            {
                if (m_nCurrentAction == Grobal2.SM_LIGHTING)
                {
                    if (boCasted)
                    {
                        boCasted = false;
                        //ClMain.g_PlayScene.NewMagic(this, 80, 80, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY,m_nTargetRecog, magiceff.TMagicType.mtRedThunder, false, 30, ref bofly);
                    }
                }
            }

            base.Run();
        }
    }
}