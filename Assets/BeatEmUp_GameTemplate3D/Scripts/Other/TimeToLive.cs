using UnityEngine;

public class TimeToLive : MonoBehaviour {

	public float LifeTime = 1;

	private void Start(){
		Invoke("DestroyGO", LifeTime);
	}

	private void DestroyGO(){
		Destroy(gameObject);
	}
}
