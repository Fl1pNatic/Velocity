﻿using Demos;
using Game;
using UnityEngine;

namespace UI.MenuWindows
{
    public class EndLevelWindow : DefaultMenuWindow
    {
        private Demo demo;

        public void Initialize(Demo demo)
        {
            this.demo = demo;
            PlayRaceDemo();
        }

        public void RestartRun()
        {
            GameMenu.SingletonInstance.CloseAllWindows();
            GameMenu.SingletonInstance.AddWindow(Window.PLAY);
            WorldInfo.info.RaceScript.PrepareNewRun();
        }

        public void PlayRaceDemo()
        {
            WorldInfo.info.PlayDemo(demo, true, false);
        }

        //Save demo to ".vdem" file, does not work in web player
        public void SaveLastDemo()
        {
            demo.SaveToFile(Application.dataPath);
        }

        public void ToMainMenu()
        {
            GameInfo.info.LoadMainMenu();
        }
    }
}
