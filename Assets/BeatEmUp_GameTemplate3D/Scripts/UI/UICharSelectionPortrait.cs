using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICharSelectionPortrait : MonoBehaviour {

	public bool Selected;

	[Header("The Player Character ID")]
	public int PlayerID;
	
	[Header("The Player Character Prefab")]
	public GameObject PlayerPrefab;

	[Header("HUD Portrait")]
	public Sprite HUDPortrait;

	[Space]
	[SerializeField]
	private UICharBuying buyingComponent;

	public void OnClick(){
		Selected = true;

		GlobalGameSettings.Player1CharacterID = PlayerID;

		//set selected player prefab
		UICharSelection characterSelectionScrn = GameObject.FindObjectOfType<UICharSelection>();
		if(characterSelectionScrn) characterSelectionScrn.SelectPlayer(PlayerID, PlayerPrefab);
	}

	private void OnEnable()
	{
		buyingComponent?.UpdateVisual();
	}
}