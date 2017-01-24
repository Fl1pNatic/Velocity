﻿using Game;
using UnityEngine;

namespace UI.MenuWindows
{
    public class EscWindow : DefaultMenuWindow
    {
        private void Update()
        {
            if (Input.GetButtonDown("Menu"))
                GameMenu.SingletonInstance.CloseWindow();
        }

        public void ToMainMenu()
        {
            GameInfo.info.LoadLevel("MainMenu");
        }

        public void Quit()
        {
            GameInfo.info.Quit();
        }
    }
}