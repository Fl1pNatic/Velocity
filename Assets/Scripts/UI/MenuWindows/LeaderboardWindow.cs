﻿using System.Collections.Generic;
using Api;
using Demos;
using Game;
using UnityEngine.UI;
using Util;

namespace UI.MenuWindows
{
    public class LeaderboardWindow : DefaultMenuWindow
    {
        public InputField mapNameInput;
        public List<LeaderboardPanel> entryPanels;

        private MapData loadedMap;
        private int startIndex = 0;

        private void Awake()
        {
            mapNameInput.onEndEdit.AddListener(ChangeMap);
        }

        private void ChangeMap(string mapName)
        {
            startIndex = 0;
            MapData newMap = GameInfo.info.MapManager.DefaultMaps.Find(map => map.name == mapName);
            if (newMap != null)
                LoadMap(newMap);
            else
                DisplayData(new LeaderboardEntry[0]);
        }

        public void LoadMap(MapData map, int index = 0)
        {
            // pages should always start with a multiple of 10
            startIndex = (index / 10) * 10;

            loadedMap = map;
            StartCoroutine(UnityUtils.RunWhenDone(Leaderboard.GetEntries(map, startIndex, entryPanels.Count), (request) =>
            {
                if (!request.Error)
                    DisplayData(request.Result);
                else
                    GameMenu.SingletonInstance.ShowError(request.ErrorText);
            }));
        }

        public void AddIndex(int add)
        {
            startIndex += add;
            if (startIndex < 0)
                startIndex = 0;
            LoadMap(loadedMap);
        }

        private void DisplayData(LeaderboardEntry[] entries)
        {
            for(int i = 0; i < entryPanels.Count; i++)
            {
                LeaderboardPanel panel = entryPanels[i];

                if (entries.Length <= i)
                {
                    panel.Time = "";
                    panel.Player = "";
                    panel.Rank = "";
                    panel.SetButtonActive(false);
                }
                else
                {
                    panel.Time = entries[i].Time.ToString("0.0000");
                    panel.Player = entries[i].PlayerName;
                    panel.Rank = entries[i].Rank.ToString();
                    panel.SetButtonActive(true);

                    int player = entries[i].PlayerID;
                    int map = entries[i].MapID;
                    panel.SetButtonAction(() =>
                    {
                        StartCoroutine(UnityUtils.RunWhenDone(Leaderboard.GetDemo(player, map), request =>
                        {
                            if (request.Error)
                            {
                                GameMenu.SingletonInstance.ShowError(request.ErrorText);
                            }
                            else
                            {
                                Demo demo = new Demo(request.BinaryResult);
                                demo.SaveToFile();
                            }
                        }));
                    });
                }
            }
        }
    }
}