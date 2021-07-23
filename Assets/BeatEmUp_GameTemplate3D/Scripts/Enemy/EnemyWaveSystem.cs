using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class EnemyWaveSystem : MonoBehaviour
{
    public int maxAttackers = 10; //the maximum number of enemies that can attack the player simultaneously
    public EnemyGenerator enemyGenerator;

    [Header("List of enemy Waves")] [FormerlySerializedAs("EnemyWaves")]
    public EnemyWave[] enemyWaves;

    public int currentWave;

    [Header("Slow Motion Settings")]
    public bool activateSlowMotionOnLastHit;

    public float effectDuration = 1.5f;

    private void OnEnable()
    {
        EnemyActions.OnUnitDestroy += OnUnitDestroy;
    }

    private void OnDisable()
    {
        EnemyActions.OnUnitDestroy -= OnUnitDestroy;
    }

    private void Awake()
    {
        // Return if this component disabled
        if (!enabled) return;

        // Making sure what we haven't any enemies
        DestroyAllEnemies();

        // Setting enemies based on points system
        enemyGenerator.SetWaveEnemies(0);
        enemyGenerator.SetWaveEnemies(1);
        enemyGenerator.SetWaveEnemies(2);

        // Setting maxAttackers
        SetMaxAttackers(GlobalGameSettings.currentLevelId);
        
        // LeaderboardManager.LoadScores();
    }

    public void SetMaxAttackers(int levelID)
    {
        maxAttackers = 3 + ((levelID + 1) / 4);
    }

    private void Start()
    {
        currentWave = 0;
        UpdateAreaColliders();
        StartNewWave();
    }

    //Disable all the enemies
    private void DisableAllEnemies()
    {
        foreach (EnemyWave wave in enemyWaves)
        {
            for (int i = 0; i < wave.EnemyList.Count; i++)
            {
                if (wave.EnemyList[i] != null)
                {
                    //deactivate enemy
                    wave.EnemyList[i].SetActive(false);
                }
                else
                {
                    //remove empty fields from the list
                    wave.EnemyList.RemoveAt(i);
                }
            }

            foreach (GameObject g in wave.EnemyList)
            {
                if (g != null) g.SetActive(false);
            }
        }
    }

    //Destroy all the enemies
    private void DestroyAllEnemies()
    {
        foreach (EnemyWave wave in enemyWaves)
        {
            for (int i = wave.EnemyList.Count - 1; i >= 0; i--)
            {
                if (wave.EnemyList[i] != null) Destroy(wave.EnemyList[i]);
                wave.EnemyList.RemoveAt(i);
            }
        }
    }

    //Start a new enemy wave
    public void StartNewWave()
    {
        //hide UI hand pointer
        HandPointer hp = GameObject.FindObjectOfType<HandPointer>();
        if (hp != null) hp.DeActivateHandPointer();

        //activate enemies
        foreach (GameObject g in enemyWaves[currentWave].EnemyList)
        {
            if (g != null) g.SetActive(true);
        }

        Invoke("SetEnemyTactics", .1f);
    }

    //Update Area Colliders
    private void UpdateAreaColliders()
    {
        //switch current area collider to a trigger
        if (currentWave > 0)
        {
            BoxCollider areaCollider = enemyWaves[currentWave - 1].AreaCollider;
            if (areaCollider != null)
            {
                areaCollider.enabled = true;
                areaCollider.isTrigger = true;
                AreaColliderTrigger act = areaCollider.gameObject.AddComponent<AreaColliderTrigger>();
                act.EnemyWaveSystem = this;
            }
        }

        //set next collider as camera area restrictor
        if (enemyWaves[currentWave].AreaCollider != null)
        {
            enemyWaves[currentWave].AreaCollider.gameObject.SetActive(true);
        }

        CameraFollow cf = GameObject.FindObjectOfType<CameraFollow>();
        if (cf != null) cf.CurrentAreaCollider = enemyWaves[currentWave].AreaCollider;

        //show UI hand pointer
        HandPointer hp = GameObject.FindObjectOfType<HandPointer>();
        if (hp != null) hp.ActivateHandPointer();
    }

    //An enemy has been destroyed
    private void OnUnitDestroy(GameObject g)
    {
        if (enemyWaves.Length > currentWave)
        {
            enemyWaves[currentWave].RemoveEnemyFromWave(g);
            if (enemyWaves[currentWave].waveComplete())
            {
                currentWave += 1;
                if (allWavesCompleted)
                {
                    StartCoroutine(LevelComplete());
                }
                else
                {
                    UpdateAreaColliders();
                }
            }
        }
    }

    //True if all the waves are completed

    private bool allWavesCompleted
    {
        get
        {
            int waveCount = enemyWaves.Length;
            int waveFinished = 0;

            for (int i = 0; i < waveCount; i++)
            {
                if (enemyWaves[i].waveComplete()) waveFinished += 1;
            }

            return waveCount == waveFinished;
        }
    }

    //Update enemy tactics
    private void SetEnemyTactics()
    {
        EnemyManager.SetEnemyTactics();
    }

    //Level complete
    private IEnumerator LevelComplete()
    {
        //activate slow motion effect
        if (activateSlowMotionOnLastHit)
        {
            var slowMotionDelay = Camera.main.GetComponent<CamSlowMotionDelay>();
            if (slowMotionDelay != null)
            {
                slowMotionDelay.StartSlowMotionDelay(effectDuration);
                yield return new WaitForSeconds(effectDuration);
            }
        }

        //Timeout before continuing
        yield return new WaitForSeconds(1f);

        //Fade to black
        var uiManager = GameObject.FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UI_fader.Fade(UIFader.FADE.FadeOut, 2f, 0);
            yield return new WaitForSeconds(2f);
        }

        //Disable players
        var playersObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var pObj in playersObjects)
        {
            Destroy(pObj);
        }


        // if this is last opened level
        if (PlayerPrefs.GetInt("LastOpenedLevel", 1) == GlobalGameSettings.currentLevelId + 1)
        {
            PlayerPrefs.SetInt("LastOpenedLevel", GlobalGameSettings.currentLevelId + 2);
            PlayerPrefs.Save();
        }

        GlobalGameSettings.shouldGiveReward = true;
        uiManager.ShowMenu("LevelRewardScrn");



        //GetComponent<UISceneLoader>().LoadScene("03_LevelSelection");

        //Go to next level or show GAMEOVER screen
        /*
		if (GlobalGameSettings.currentLevelId+2 < MaxLevel) {
            GlobalGameSettings.currentLevelId++;
		} else {
			//Show game over screen
			if (UI != null) {
				UI.DisableAllScreens ();
				UI.ShowMenu ("LevelComplete");
			}
		}
        */
    }
}