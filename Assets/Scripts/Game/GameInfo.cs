﻿using System;
using System.Collections.Generic;
using Api;
using Demos;
using UI;
using UI.MenuWindows;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class GameInfo : MonoBehaviour
    {
        public static GameInfo info;

        public string secretKey = "";
        public TextAsset helpFile;
        public List<string> soundNames;
        public List<AudioClip> soundClips;

        [SerializeField] private MapManager mapManager;
        public MapManager MapManager { get { return mapManager; } }

        public float circleSpeed1 = 10f;
        public float circleSpeed2 = 20f;
        public float circleSpeed3 = 30f;

        public bool InEditor { get; private set; }
        public bool LastRunWasPb { get; private set; }
        public bool CheatsActive { get; set; }

        private GameInfoFx fx;
        private Demo currentDemo;
        private GameObject myCanvas;
        private SaveData currentSave;

        public SaveData CurrentSave
        {
            get { return currentSave; }
            set
            {
                currentSave = value;
                if (value != null)
                {
                    PlayerPrefs.SetString("lastplayer", currentSave.Name);
                }
            }
        }

        private void Awake()
        {
            if (info == null)
            {
                info = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            myCanvas = transform.Find("Canvas").gameObject;

            fx = new GameInfoFx(myCanvas.transform.FindChild("FxImage").GetComponent<Image>());
            SceneManager.sceneLoaded += (scene, mode) => fx.StartColorFade(Color.black, new Color(0f, 0f, 0f, 0f), 0.5f);

            if (!PlayerPrefs.HasKey("lastplayer"))
                CurrentSave = new SaveData(PlayerPrefs.GetString("lastplayer"));
        }

        private void Update()
        {
            Settings.Input.ExecuteBoundActions();

            //Update effects
            fx.Update();
        }

        //Load a level
        public void PlayLevel(MapData map)
        {
            InEditor = false;
            fx.StartColorFade(new Color(0f, 0f, 0f, 0f), Color.black, 0.5f, () =>
            {
                GameMenu.SingletonInstance.CloseAllWindows();
                MapManager.LoadMap(map);
            });
        }

        public void LoadEditor(string editorLevelName)
        {
            InEditor = true;
            SceneManager.LoadScene("LevelEditor");
        }

        public void LoadMainMenu()
        {
            GameMenu.SingletonInstance.CloseAllWindows();
            SceneManager.LoadScene("MainMenu");
        }

        //Player hit the goal
        public void RunFinished(TimeSpan time, Demo demo)
        {
            decimal decimalTime = time.Ticks / (decimal) 10000000;
            currentDemo = demo;
            LastRunWasPb = CurrentSave.SaveIfPersonalBest(decimalTime, SceneManager.GetActiveScene().name);

            GameMenu.SingletonInstance.CloseAllWindows();
            EndLevelWindow window = (EndLevelWindow) GameMenu.SingletonInstance.AddWindow(Window.END_LEVEL);
            window.Initialize(currentDemo);

            if (!WorldInfo.info.RaceScript.RunVaild)
            {
                print("Invalid run!");
                return;
            }
            if (currentSave == null)
            {
                print("Invalid save!");
                return;
            }
            if (!currentSave.Account.IsLoggedIn)
            {
                print("Account not logged in!");
                return;
            }

            Leaderboard.SendEntry(currentSave.Account.Name, decimalTime, SceneManager.GetActiveScene().name, currentSave.Account.Token, currentDemo);
        }

        //Leave the game
        public void Quit()
        {
            Application.Quit();
        }

        public void DeletePlayer(string name)
        {
            SaveData sd = new SaveData(name);
            sd.DeleteData();

            //Log out from current player if we deleted that one
            if (CurrentSave != null && CurrentSave.Name == name)
                CurrentSave = null;
        }

        public void SetCursorLock(bool value)
        {
            CursorLockMode mode = CursorLockMode.None;
            if (value)
                mode = CursorLockMode.Locked;
            Cursor.lockState = mode;
            Cursor.visible = !value;
        }
    }
}