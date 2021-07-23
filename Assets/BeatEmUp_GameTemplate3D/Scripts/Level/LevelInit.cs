using UnityEngine;

public class LevelInit : MonoBehaviour
{
    [Space(5)]
    [Header("Settings")]
    public string showMenuAtStart = "";
    public bool createInputManager;
    public bool createUI;
    public bool createAudioPlayer;
    public bool createMusicPlayer = true;
    public bool createGameCamera;
    public bool createAdsManager;
    private GameSettings settings;

    private void Awake()
    {
        //set settings
        settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
        if (settings != null)
        {
            Time.timeScale = settings.timeScale;
            Application.targetFrameRate = settings.framerate;
        }

        //create Audio Player
        if (!GameObject.FindObjectOfType<BeatEmUpTemplate.AudioPlayer>() && createAudioPlayer)
            GameObject.Instantiate(Resources.Load("AudioPlayer"), Vector3.zero, Quaternion.identity);

        //create Music Player
        if (!GameObject.FindObjectOfType<MusicPlayer>() && createMusicPlayer)
            GameObject.Instantiate(Resources.Load("MusicPlayer"), Vector3.zero, Quaternion.identity);

        //create InputManager
        if (!GameObject.FindObjectOfType<InputManager>() && createInputManager)
            GameObject.Instantiate(Resources.Load("InputManager"), Vector3.zero, Quaternion.identity);
        GameObject.FindObjectOfType<InputManager>().CheckInputType();

        //create UI
        if (!GameObject.FindObjectOfType<UIManager>() && createUI)
            GameObject.Instantiate(Resources.Load("UI"), Vector3.zero, Quaternion.identity);

        //create Game Camera
        if (!GameObject.FindObjectOfType<CameraFollow>() && createGameCamera)
            GameObject.Instantiate(Resources.Load("GameCamera"), Vector3.zero, Quaternion.identity);

        //create Ads
        if (!GameObject.FindObjectOfType<AdMob>() && createAdsManager)
            GameObject.Instantiate(Resources.Load("AdMob"), Vector3.zero, Quaternion.identity);

        //open a menu at level start
        if (!string.IsNullOrEmpty(showMenuAtStart)) ShowMenuAtStart();
    }

    private void ShowMenuAtStart()
    {
        GameObject.FindObjectOfType<UIManager>().ShowMenu(showMenuAtStart);
    }
}