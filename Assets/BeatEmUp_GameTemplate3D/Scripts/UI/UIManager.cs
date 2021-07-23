using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIFader UI_fader;
    public UI_Screen[] UIMenus;

    private const string MONEY_COUNTER_NAME = "MoneyCounter";
    private const string TOUCH_CONTROLS_NAME = "TouchScreenControls";

    public static UIManager instance;
    private void Awake()
    {
        instance = this;
        
        DisableAllScreens();

        //don't destroy
        DontDestroyOnLoad(gameObject);
    }

    //shows a menu by name
    public void ShowMenu(string name, bool disableAllScreens)
    {
        if (disableAllScreens) DisableAllScreens();

        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Name == name)
            {
                if (UI.UI_Gameobject != null)
                {
                    UI.UI_Gameobject.SetActive(true);
                    SetTouchScreenControls(UI);
                    SetMoneyCounter(UI);
                }
                else
                {
                    Debug.Log("no menu found with name: " + name);
                }
            }
        }

        //fadeIn
        if (UI_fader != null) UI_fader.gameObject.SetActive(true);
        UI_fader.Fade(UIFader.FADE.FadeIn, .5f, .3f);
    }

    public void ShowMenu(string name)
    {
        ShowMenu(name, true);
    }

    private bool switchingMenus;
    public void ShowMenuWithFade(string name)
    {
        if (!switchingMenus) StartCoroutine(ShowMenuWithFadeCoroutine(name));
    }
    private IEnumerator ShowMenuWithFadeCoroutine(string name)
    {
        switchingMenus = true;
        UI_fader.Fade(UIFader.FADE.FadeOut, 1, 0);
        yield return new WaitForSeconds(1);
        ShowMenu(name);
        switchingMenus = false;
    }

    //close a menu by name
    public void CloseMenu(string name)
    {
        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Name == name) UI.UI_Gameobject.SetActive(false);
        }
    }

    //disable all the menus
    public void DisableAllScreens()
    {
        foreach (UI_Screen UI in UIMenus)
        {
            if (UI.UI_Gameobject != null)
                UI.UI_Gameobject.SetActive(false);
            else
                Debug.Log("Null ref found in UI with name: " + UI.UI_Name);
        }
    }

    //show or hide touch screen controls
    private void SetTouchScreenControls(UI_Screen UI)
    {
        if (UI.UI_Name == TOUCH_CONTROLS_NAME) return;
        InputManager inputManager = GameObject.FindObjectOfType<InputManager>();
        if (inputManager != null && inputManager.inputType == INPUTTYPE.TOUCHSCREEN)
        {
            if (UI.showTouchControls)
            {
                ShowMenu(TOUCH_CONTROLS_NAME, false);
            }
            else
            {
                CloseMenu(TOUCH_CONTROLS_NAME);
            }
        }
    }

    //show or hide money counter
    private void SetMoneyCounter(UI_Screen UI)
    {
        if (UI.UI_Name == MONEY_COUNTER_NAME) return;
        InputManager inputManager = GameObject.FindObjectOfType<InputManager>();
        if (UI.showMoneyCounter)
        {
            ShowMenu(MONEY_COUNTER_NAME, false);
        }
        else
        {
            CloseMenu(MONEY_COUNTER_NAME);
        }
    }
}

[System.Serializable]
public class UI_Screen
{
    public string UI_Name;
    public GameObject UI_Gameobject;
    public bool showTouchControls;
    public bool showMoneyCounter;
}