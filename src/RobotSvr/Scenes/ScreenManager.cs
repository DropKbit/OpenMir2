using System;
using System.Drawing;

namespace RobotSvr
{
    public class ScreenManager
    {
        private readonly RobotClient robotClient;
        public SceneBase CurrentScene = null;

        public ScreenManager(RobotClient robotClient)
        {
            this.robotClient = robotClient;
            CurrentScene = null;
        }

        public void ChangeScene(SceneType scenetype)
        {
            if (CurrentScene != null)
            {
                CurrentScene.CloseScene();
            }
            switch (scenetype)
            {
                case SceneType.stIntro:
                    CurrentScene = robotClient.IntroScene;
                    robotClient.IntroScene.m_dwStartTime = MShare.GetTickCount() + 2000;
                    break;
                case SceneType.stLogin:
                    CurrentScene = robotClient.LoginScene;
                    break;
                case SceneType.stSelectCountry:
                    break;
                case SceneType.stSelectChr:
                    CurrentScene = robotClient.SelectChrScene;
                    break;
                case SceneType.stNewChr:
                    break;
                case SceneType.stLoading:
                    break;
                case SceneType.stPlayGame:
                    CurrentScene = robotClient.g_PlayScene;
                    break;
            }
            if (CurrentScene != null)
            {
                CurrentScene.OpenScene();
            }
        }

        public void AddSysMsg(string msg)
        {

        }

        public void AddSysMsgCenter(string msg, Color fc, Color bc, int sec)
        {
        }

        public void AddSysMsgCenter(string msg, int fc, int bc, int sec)
        {

        }

        public void AddChatBoardString(string str, Color fcolor, Color bcolor)
        {
            Console.WriteLine(str);
        }

        public void AddChatBoardString(string str, int fcolor, Color bcolor)
        {
            Console.WriteLine(str);
        }

        public void AddChatBoardString(string str, int fcolor, int bcolor)
        {
            Console.WriteLine(str);
        }

        public void ClearChatBoard()
        {

        }

        public void ClearHint()
        {

        }

    }
}

