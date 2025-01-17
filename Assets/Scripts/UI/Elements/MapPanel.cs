﻿using Api;
using Game;
using UI.MenuWindows;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace UI.Elements
{
    public class MapPanel : MonoBehaviour
    {
        [SerializeField] private Text nameField;
        [SerializeField] private Text authorField;
        [SerializeField] private RawImage previewImage;
        [SerializeField] private Text pbField;
        [SerializeField] private Text wrField;
        [SerializeField] private Text wrPlayerField;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button pbButton;
        [SerializeField] private Button wrButton;

        public void Set(int slot, MapData map, string pb)
        {


            nameField.text = map.name;
            authorField.text = map.author;
            previewImage.texture = map.previewImage;
            pbField.text = pb;
            loadButton.onClick.AddListener(() => OnPlayableMapClick(map));
            pbButton.onClick.AddListener(OpenLeaderboardButton.CreateClickListener(map, 0)); // TODO load at player index instead of 0
            wrButton.onClick.AddListener(OpenLeaderboardButton.CreateClickListener(map, 0));

            StartCoroutine(UnityUtils.RunWhenDone(Leaderboard.GetRecord(map), (request) =>
            {
                if (!request.Error && request.Result.Length > 0)
                    SetWrText(request.Result[0]);
            }));
        }

        private void OnPlayableMapClick(MapData map)
        {
            GameMenu.SingletonInstance.AddWindow(Window.LOADING);
            SceneManager.sceneLoaded += OnSceneLoaded;
            GameInfo.info.MapManager.LoadMap(map);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            WorldInfo.info.CreatePlayer(false);
        }

        private void SetWrText(LeaderboardEntry entry)
        {
            wrField.text = entry.Time.ToString("0.0000");
            wrPlayerField.text = entry.PlayerName;
        }
    }
}