﻿using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    /// <summary>
    /// 拒绝发言
    /// </summary>
    [GameCommand("PrvMsg", "拒绝发言", M2Share.g_sGameCommandPrvMsgHelpMsg, 10)]
    public class PrvMsgCommand : BaseCommond
    {
        [DefaultCommand]
        public void PrvMsg(string[] @Params, TPlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            var sHumanName = @Params.Length > 0 ? @Params[0] : "";
            if (string.IsNullOrEmpty(sHumanName) || !string.IsNullOrEmpty(sHumanName) && sHumanName[1] == '?')
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            for (var i = PlayObject.m_BlockWhisperList.Count - 1; i >= 0; i--)
            {
                if (PlayObject.m_BlockWhisperList.Count <= 0)
                {
                    break;
                }
                //if ((PlayObject.m_BlockWhisperList[i]).CompareTo((sHumanName)) == 0)
                //{
                //    PlayObject.m_BlockWhisperList.RemoveAt(i);
                //    PlayObject.SysMsg(string.Format(M2Share.g_sGameCommandPrvMsgUnLimitMsg, sHumanName), TMsgColor.c_Green, TMsgType.t_Hint);
                //    return;
                //}
            }
            PlayObject.m_BlockWhisperList.Add(sHumanName);
            PlayObject.SysMsg(string.Format(M2Share.g_sGameCommandPrvMsgLimitMsg, sHumanName), MsgColor.Green, MsgType.Hint);
        }
    }
}