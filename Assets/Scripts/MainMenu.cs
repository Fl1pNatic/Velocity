﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	public List<string> mapNames = new List<string>();

	private bool drawMainButtons = true;
	private bool drawSelectNewGame = false;
	private bool drawSelectLoadGame = false;
	private bool drawNameField = false;
	private bool drawSelectMap = false;
	private bool drawSettings = false;
	private string nameFieldText = "name";
	private int selectedNewGameIndex = -1;
	private GUISkin skin;

	private List<string> saveNames = new List<string>();

	private enum State
	{
		main = 1,
		newGame = 2,
		loadGame = 3,
		enterName = 4,
		selectMap = 5,
		settings = 6
	}

	void Start()
	{
		GameInfo.info.setMenuState(GameInfo.MenuState.inactive);
		GameInfo.info.menuLocked = true;
		skin = GameInfo.info.skin;
	}

	private void setState(State state)
	{
		updateSaveInfos();

		drawMainButtons = false;
		drawSelectNewGame = false;
		drawSelectLoadGame = false;
		drawNameField = false;
		drawSelectMap = false;
		drawSettings = false;

		switch(state)
		{
			case State.main:
				drawMainButtons = true;
				break;
			case State.newGame:
				drawSelectNewGame = true;
				break;
			case State.loadGame:
				drawSelectLoadGame = true;
				break;
			case State.enterName:
				drawNameField = true;
				break;
			case State.selectMap:
				drawSelectMap = true;
				break;
			case State.settings:
				drawSettings = true;
				break;
		}
	}

	private void updateSaveInfos()
	{
		int saveCount = 3;

		saveNames.Clear();

		for(int i = 1; i <= saveCount; i++)
		{
			SaveData data = new SaveData(i);
			string name = data.getPlayerName();
			if(name.Equals(""))
			{
				name = "Empty Save";
			}
			saveNames.Add(name);
		}
	}

	void OnGUI()
	{
		//Center
		Rect centerMenuPos = new Rect(Screen.width / 2f - 75f, Screen.height / 2f - 75f, 150f, 150f);
		
		//Main buttons
		if(drawMainButtons)
		{
			GUILayout.BeginArea(centerMenuPos, skin.box);
			if(GUILayout.Button("New", skin.button)) { setState(State.newGame); }
			if(GUILayout.Button("Load", skin.button)) { setState(State.loadGame); }
			if(GUILayout.Button("Settings", skin.button)) { setState(State.settings); }
			if(GUILayout.Button("Quit", skin.button)) { Application.Quit(); }
			GUILayout.EndArea();
		}

		//New game
		if(drawSelectNewGame)
		{
			GUI.Box(centerMenuPos, "", skin.box);
			GUILayout.BeginArea(centerMenuPos);
			GUILayout.Label("New", skin.label);
			if(GUILayout.Button("1: " + saveNames[0], skin.button)) { selectedNewGameIndex = 1; setState(State.enterName); }
			if(GUILayout.Button("2: " + saveNames[1], skin.button)) { selectedNewGameIndex = 2; setState(State.enterName); }
			if(GUILayout.Button("3: " + saveNames[2], skin.button)) { selectedNewGameIndex = 3; setState(State.enterName); }
			if(GUILayout.Button("Back", skin.button)) { setState(State.main); }
			GUILayout.EndArea();
		}

		//Load game
		if(drawSelectLoadGame)
		{
			GUILayout.BeginArea(centerMenuPos, skin.box);
			GUILayout.Label("Load", skin.label);
			if(GUILayout.Button("1: " + saveNames[0], skin.button)) { loadGame(1); }
			if(GUILayout.Button("2: " + saveNames[1], skin.button)) { loadGame(2); }
			if(GUILayout.Button("3: " + saveNames[2], skin.button)) { loadGame(3); }
			if(GUILayout.Button("Back", skin.button)) { setState(State.main); }
			GUILayout.EndArea();
		}

		//select map
		if(drawSelectMap)
		{
			Rect mapSelectPos = new Rect(Screen.width / 4f, Screen.height / 2f - 100f, Screen.width / 2f, 200f);
			Rect mapInfoPos = new Rect(mapSelectPos.x, mapSelectPos.y - 35f, mapSelectPos.width, 30f);
			float boxWidth = mapSelectPos.width / 3f;
			int counter = 0;

			//Info box
			GUILayout.BeginArea(mapInfoPos, skin.box);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Current User: " + GameInfo.info.getCurrentSave().getPlayerName() + " | Select a map.", skin.label);
			if(GUILayout.Button("Back", skin.button, GUILayout.MaxWidth(100f))) { setState(State.loadGame); }
			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			//Big box for the list of maps
			GUI.Box(mapSelectPos, "", skin.box);

			//Create three coloumns
			for(int i = 0; i < 3; i++)
			{
				Rect boxRect = new Rect(mapSelectPos.x + i*boxWidth, mapSelectPos.y, boxWidth, mapSelectPos.height);
				GUILayout.BeginArea(boxRect);

				//Fill them with buttons
				for(int j = 0; j < 3; j++)
				{
					if(counter + j < mapNames.Count)
					{
						if(GUILayout.Button(mapNames[counter + j], skin.button)) { loadMap(mapNames[counter + j]); }
					}
				}
				counter += 3;

				GUILayout.EndArea();
			}
		}

		//Enter name for new game
		if(drawNameField)
		{
			GUI.Box(centerMenuPos, "", skin.box);
			GUILayout.BeginArea(centerMenuPos);
			GUILayout.Label("Enter name");
			nameFieldText = GUILayout.TextField(nameFieldText);
			if(GUILayout.Button("Back", skin.button)) { setState(State.newGame); }
			if(GUILayout.Button("OK", skin.button))
			{
				newGame(selectedNewGameIndex);
			}
			GUILayout.EndArea();
		}

		if(drawSettings)
		{
			//Start GUI
			GUILayout.BeginArea(new Rect(Screen.width / 2f - 200f, Screen.height / 2f - 50f, 400f, 100f), skin.box);
			GUILayout.BeginVertical();

			//FOV
			GUILayout.BeginHorizontal();
			GUILayout.Label("FOV", skin.label);
			GameInfo.info.fov = GUILayout.HorizontalSlider(GameInfo.info.fov, 60f, 120f, skin.horizontalSlider, skin.horizontalSliderThumb);
			GameInfo.info.fov = Mathf.RoundToInt(GameInfo.info.fov);
			GUILayout.Label(GameInfo.info.fov.ToString(), skin.label);
			GUILayout.EndHorizontal();

			//Sensitivity
			GUILayout.BeginHorizontal();
			GUILayout.Label("Mouse Sensitivity", skin.label);
			GameInfo.info.mouseSpeed = GUILayout.HorizontalSlider(GameInfo.info.mouseSpeed, 0.5f, 10f, skin.horizontalSlider, skin.horizontalSliderThumb);
			GameInfo.info.mouseSpeed = floor(GameInfo.info.mouseSpeed, 1);
			GUILayout.Label(GameInfo.info.mouseSpeed.ToString(), skin.label);
			GUILayout.EndHorizontal();
			
			//Volume
			GUILayout.BeginHorizontal();
			GUILayout.Label("Volume", skin.label);
			GameInfo.info.volume = GUILayout.HorizontalSlider(GameInfo.info.volume, 0f, 1f, skin.horizontalSlider, skin.horizontalSliderThumb);
			GameInfo.info.volume = floor(GameInfo.info.volume, 2);
			GUILayout.Label(GameInfo.info.volume.ToString(), skin.label);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("OK", skin.button)) { GameInfo.info.savePlayerSettings(); setState(State.main); }
			if(GUILayout.Button("Cancel", skin.button)) { setState(State.main); }
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	//Returns rounded value of a float
	private float floor(float input, int decimalsAfterPoint)
	{
		string floatText = input.ToString();
		if(floatText.ToLower().Contains("e"))
		{
			return 0f;
		}
		else
		{
			if(floatText.Contains("."))
			{
				int decimalCount = floatText.Substring(floatText.IndexOf(".")).Length;
				if(decimalCount <= decimalsAfterPoint)
				{
					return input;
				}
				else
				{
					return float.Parse(floatText.Substring(0, floatText.IndexOf(".") + decimalsAfterPoint + 1));
				}
			}
			else
			{
				return input;
			}
		}
	}

	private void newGame(int index)
	{
		SaveData data = new SaveData(index, nameFieldText);
		GameInfo.info.setCurrentSave(data);
	}

	private void loadGame(int index)
	{
		SaveData data = new SaveData(index);
		GameInfo.info.setCurrentSave(data);
		setState(State.selectMap);

	}

	private void loadMap(string name)
	{
		Application.LoadLevel(name);
	}
}
