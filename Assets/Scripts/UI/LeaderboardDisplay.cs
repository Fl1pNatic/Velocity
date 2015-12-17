﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Api;

public class LeaderboardDisplay : MainSubMenu
{
    public InputField mapNameInput;
    public List<LeaderboardPanel> entryPanels; //Must always have ELEMENTS_PER_SITE elements!

    private const int ELEMENTS_PER_SITE = 10;

    private string lastLoadedMap = "";
    private int startIndex = 0;

    void Awake()
    {
        if (mapNameInput)
        {
            mapNameInput.onEndEdit.AddListener(ChangeMap);
        }
    }

    private void ChangeMap(string mapName)
    {
        startIndex = 0;
        LoadMap(mapName);
    }

    public void LoadMap(string mapName)
    {
        Leaderboard.GetEntries(mapName, startIndex, ELEMENTS_PER_SITE, DisplayData);
        lastLoadedMap = mapName;
    }

    private void DisplayData(LeaderboardEntry[] entries)
    {
        for(int i = 0; i < ELEMENTS_PER_SITE; i++)
        {
            if (entries.Length <= i)
            {
                entryPanels[i].time = "";
                entryPanels[i].player = "";
                entryPanels[i].rank = "";
                entryPanels[i].SetButtonAction(delegate { });
                entryPanels[i].SetButtonActive(false);
            }
            else
            {
                entryPanels[i].time = entries[i].time.ToString("0.0000");
                entryPanels[i].player = entries[i].playerName;
                entryPanels[i].rank = entries[i].rank.ToString();
                int id = entries[i].id;
                entryPanels[i].SetButtonAction(() => Leaderboard.GetDemo(id, ProcessDownloadedDemo));
                entryPanels[i].SetButtonActive(true);
            }
        }
    }

    private void ProcessDownloadedDemo(Demo demo)
    {
        print("Player: " + demo.getPlayerName());
        print("Level: " + demo.getLevelName());
    }

    public void AddIndex(int add)
    {
        startIndex += add;
        if (startIndex < 0)
            startIndex = 0;
        LoadMap(lastLoadedMap);
    }
}