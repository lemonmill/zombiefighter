using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace IAP
{
    public class ProductButton : MonoBehaviour
    {
        public ShopProductNames productName;// = ShopProductNames.GetSoft1;
        public string defaultProductName = "";
        [Space]
        public string addCoinsFormat = "Buy Coins: {0}";
        public Text addCoinsText;
        [Space]
        public string priceFormat = "Price: {0}";
        public Text priceText;
        [Space] 
        public bool getLocalizedTitle = false;
        public string productNameFormat = "{0}";
        public Text productNameText;
        [Space]
        public bool disableIfBought;
        public CanvasGroup canvasGroup;
        public float enabledAlpha  = 1;
        public float disabledAlpha = .75f;
        public string boughtNameFormat = "{0} (Bought!)";

        private IDisposable waitForInitializeTask;
        
        public void OnEnable() => UpdateOnInitialized();

        public void OnDisable()
        {
            waitForInitializeTask?.Dispose();
        }

        private void UpdateOnInitialized()
        {
            if (global::IAPManager.Instance.IsInitialized())
            {
                UpdateVisual();
                return;
            }
            waitForInitializeTask?.Dispose();
            waitForInitializeTask =
                Observable.Timer(TimeSpan.FromSeconds(.25f)).Subscribe(_ =>
                {
                    if (!global::IAPManager.Instance.IsInitialized()) return;
                    
                    waitForInitializeTask?.Dispose();
                    UpdateVisual();
                });
        }

        public void UpdateVisual()
        {
            bool isInitialized = global::IAPManager.Instance.IsInitialized();

            string productTitle;
            if (isInitialized && getLocalizedTitle)
                productTitle = global::IAPManager.Instance.GetLocalizedTitle(productName);
            else
                productTitle = defaultProductName;
            
            
            
            if (!isInitialized)
            {
                if (canvasGroup!=null)
                {
                    canvasGroup.interactable = false;
                    canvasGroup.alpha = disabledAlpha;
                }
                return;
            }
            
            if (productNameText!=null)
                productNameText.text = String.Format(productNameFormat, productTitle);
            
            if (addCoinsText!=null)
                addCoinsText.text = String.Format(addCoinsFormat, 
                    global::IAPManager.Instance.GetValue(productName));
            
            if (priceText!=null)    
                priceText.text    = String.Format(priceFormat,    
                    global::IAPManager.Instance.GetLocalizedPriceString(productName));
            
            if (disableIfBought && canvasGroup!=null)
            {
                var bought = global::IAPManager.Instance.IsActive(productName);
                Debug.Log(bought);
                canvasGroup.interactable = !bought;
                canvasGroup.alpha = bought ? disabledAlpha : enabledAlpha;
                if (bought && productNameText != null) productNameText.text = String.Format(boughtNameFormat, productTitle);
            }
        }
    
        public void Buy()
        {
            global::IAPManager.Instance.BuyProduct(productName, (status, message, product) =>
            {
                if (status == IAPOperationStatus.Success)
                {
                    switch (productName)
                    {
                        // case ShopProductNames.GetSoft1:
                        //     SavableSettings.instance.AddMoney(product.value);
                        //     break;
                        // case ShopProductNames.RemoveAds:
                        //     IAPManager.boughtRemoveAds = true;
                        //     break;
                    }
                    
                    SavableSettings.instance.Save();
                }
                if (status == IAPOperationStatus.Fail)
                    Debug.LogWarning("Buying error: " + message);
                
                UpdateVisual();
            });
        }
    }
}
