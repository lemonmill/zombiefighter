using UnityEngine;

public class PlaySFXOnStart : MonoBehaviour {

	public string sfx;

	private void Start () {
		GlobalAudioPlayer.PlaySFXAtPosition (sfx, transform.position, transform);
	}
}
