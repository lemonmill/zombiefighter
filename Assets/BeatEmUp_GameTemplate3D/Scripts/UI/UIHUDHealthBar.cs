using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIHUDHealthBar : MonoBehaviour {

	public Text nameField;
	public Image playerPortrait;
	public Slider HpSlider;
	public bool isPlayer;
	private IDisposable playerSpawnedSubscription;
	
	
	private void OnEnable() {
		HealthSystem.onHealthChange += UpdateHealth;
		if(isPlayer)
		{
			playerSpawnedSubscription = 
				PlayerMovement
					.playerSpawned
					.Subscribe(SetPlayerPortraitAndName);
		}
	}

	private void OnDisable() {
		HealthSystem.onHealthChange -= UpdateHealth;
		if (isPlayer)
			playerSpawnedSubscription.Dispose();
	}

	private void Start(){
		if(!isPlayer) Invoke("HideOnDestroy", Time.deltaTime); //hide enemy healthbar at start
	}

	private void UpdateHealth(float percentage, GameObject go){
		if(isPlayer && go.CompareTag("Player")){
			HpSlider.value = percentage;
		} 	

		if(!isPlayer && go.CompareTag("Enemy")){
			HpSlider.gameObject.SetActive(true);
			HpSlider.value = percentage;
			nameField.text = go.GetComponent<EnemyActions>().enemyName;
			if(percentage == 0) Invoke("HideOnDestroy", 2);
		}
	}

	private void HideOnDestroy(){
		HpSlider.gameObject.SetActive(false);
		nameField.text = "";
	}

	//loads the HUD icon of the player from the player prefab (Healthsystem)
	private void SetPlayerPortraitAndName(HealthSystem playerHS){
		if (playerHS == null) return;
		
		if(playerPortrait != null){
			//set portrait
			Sprite HUDPortrait = playerHS.HUDPortrait;
			playerPortrait.overrideSprite = HUDPortrait;

			//set name
			nameField.text = playerHS.PlayerName;
		}
	}
}
