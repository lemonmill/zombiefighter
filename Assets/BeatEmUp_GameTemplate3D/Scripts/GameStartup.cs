using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class GameStartup : MonoBehaviour
    {
        public GameObject ItemNearPlayerPrefab; // spawb item near the player 

        // Start is called before the first frame update
        void Start()
        {
            SpawnItem();
        }

        void SpawnItem()
        {
            var ppoint = FindObjectOfType<PlayerSpawnPoint>();

            Vector2 rdir = Random.insideUnitCircle;
            Vector3 offset = new Vector3(rdir.x, 0.1f, rdir.y);
            var pos = ppoint.transform.position + offset;
            Instantiate(ItemNearPlayerPrefab, pos, Quaternion.identity);
        }
    }
}