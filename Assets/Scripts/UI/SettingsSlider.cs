﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsSlider : MonoBehaviour
    {
        public Slider slider;
        public Text display;
        public string settingName;

        void Awake()
        {
            slider.onValueChanged.AddListener(OnValueChanged);
            MainMenu.SettingsOpened += OnSettingsOpened;
        }

        private void OnValueChanged(float value)
        {
            SetSetting(value);
            DisplaySetting();
        }

        public void OnSettingsOpened(object sender, EventArgs e)
        {
            DisplaySetting();
        }

        private void SetSetting(float value)
        {
            Game.SetSettingFloat(settingName, value);
        }

        private void DisplaySetting()
        {
            slider.value = Game.GetSettingFloat(settingName);
            display.text = Game.GetSettingValueName(settingName);
        }
    }
}