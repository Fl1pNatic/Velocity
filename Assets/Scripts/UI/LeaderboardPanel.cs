﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class LeaderboardPanel : MonoBehaviour
    {
        private Text timeObj;
        private Text playerObj;
        private Text rankObj;
        private Button donwloadButtonObj;

        private void Awake()
        {
            timeObj = transform.Find("Time").GetComponent<Text>();
            playerObj = transform.Find("Player").GetComponent<Text>();
            rankObj = transform.Find("Rank").GetComponent<Text>();
            donwloadButtonObj = transform.Find("GetDemo").GetComponent<Button>();
        }

        public string Time
        {
            get { return timeObj.text; }
            set { timeObj.text = value; }
        }
        public string Player
        {
            get { return playerObj.text; }
            set { playerObj.text = value; }
        }
        public string Rank
        {
            get { return rankObj.text; }
            set { rankObj.text = value; }
        }
    
        public void SetButtonAction(UnityAction action)
        {
            donwloadButtonObj.onClick.AddListener(action);
        }

        public void SetButtonActive(bool value)
        {
            donwloadButtonObj.interactable = value;
        }
    }
}
