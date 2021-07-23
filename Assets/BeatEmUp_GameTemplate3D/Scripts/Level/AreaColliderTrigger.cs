using UnityEngine;

public class AreaColliderTrigger : MonoBehaviour {

	public EnemyWaveSystem EnemyWaveSystem;

	private void OnTriggerEnter(Collider coll){
		if (coll.CompareTag ("Player")) {
			if (EnemyWaveSystem != null)
				EnemyWaveSystem.StartNewWave ();
			Destroy (gameObject);
		}
	}
}
