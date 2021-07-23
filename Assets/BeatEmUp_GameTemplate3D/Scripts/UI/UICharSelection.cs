using System.Collections;
using UnityEngine;

public class UICharSelection : UISceneLoader {

	private UICharSelectionPortrait[] portraits;

	private void OnEnable(){
		InputManager.onInputEvent += OnInputEvent;
	}

	private void OnDisable() {
		InputManager.onInputEvent += OnInputEvent;
	}

	private void Start(){
		
		//find all character portraits
		portraits = GetComponentsInChildren<UICharSelectionPortrait>();

		//select first portrait by default
		GetComponentInChildren<UICharSelectionPortrait>().OnClick();
	}

	private void OnInputEvent(string action, BUTTONSTATE buttonState){

		//move left
		//if(action == "Left" && buttonState == BUTTONSTATE.PRESS) OnLeftButtonDown();

		//move right
		//if(action == "Right" && buttonState == BUTTONSTATE.PRESS) OnRightButtonDown();

	}

	//select portrait on the left
	private void OnLeftButtonDown(){
		int selectedPortrait = getSelectedPortrait();
		portraits[selectedPortrait].Selected = false; //disable the current selection
		if(selectedPortrait-1 >= 0) portraits[selectedPortrait-1].OnClick(); //select previous portrait
	}

	//select portrait on the right
	private void OnRightButtonDown(){
		int selectedPortrait = getSelectedPortrait();
		portraits[selectedPortrait].Selected = false; //disable the current selection
		if(selectedPortrait+1 < portraits.Length) portraits[selectedPortrait+1].OnClick(); //select next portrait
	}

	//returns the index of the current selected portrait
	private int getSelectedPortrait(){
		for(int i = 0; i < portraits.Length; i++) {
			if(portraits[i].Selected) return i;
		}
		return 0;
	}

	//select a player
	public void SelectPlayer(int playerID, GameObject playerPrefab){
		GlobalGameSettings.Player1CharacterID = playerID;
		GlobalGameSettings.Player1Prefab = playerPrefab;
	}
}
