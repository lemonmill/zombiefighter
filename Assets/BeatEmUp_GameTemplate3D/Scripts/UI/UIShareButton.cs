using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class UIShareButton : MonoBehaviour
{
    public string marker = "default";
    public int reward = 50;
    
    public Button _selfButton;
    public Text _rewardTextField;
    

    private void Start()
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        var isAlreadyActivated = SavableSettings.instance.markers.Contains(marker);
        _selfButton.interactable = !isAlreadyActivated;
        if (isAlreadyActivated)
            _rewardTextField.text = "Thank you!";
        else
            _rewardTextField.text = "+ " + reward + "$";

    }
    
    public void Interact()
    {
        var savableSettings = SavableSettings.instance;

        Application.OpenURL(marker);
        
        if (savableSettings.markers.Contains(marker))
            return;

        savableSettings.markers.Add(marker);
        savableSettings.AddMoney(reward);
        
        UpdateVisual();
    }
}