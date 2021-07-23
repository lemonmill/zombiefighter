using UnityEngine;
using UnityEngine.UI;

public class UICharBuying : MonoBehaviour
{
    public int playerID;
    public int cost;
    
    [Header("References")]
    public GameObject buyButton;
    public Button characterSelectionButton;
    public Button upgradeButton;


    public void UpdateVisual()
    {
        bool isBuyed = SavableSettings.instance.charactersUpgrades[playerID].isCharacterBuyed;
        
        if (buyButton)
        {
            buyButton.SetActive(!isBuyed);
        }

        if (upgradeButton)
            upgradeButton.interactable = isBuyed;

        characterSelectionButton.interactable = isBuyed;
    }

    public void BuyCharacter()
    {
        if (SavableSettings.instance.BuyWithoutSave(cost))
        {
            SavableSettings.instance.charactersUpgrades[playerID].isCharacterBuyed = true;
            SavableSettings.instance.Save();
            UpdateVisual();
        }
    }

    private void OnEnable()
    {
        UpdateVisual();
    }
}