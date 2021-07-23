using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IAP
{
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager instance;

        private const string REMOVE_ADS_MARKER = "7239513";
        public static bool boughtRemoveAds
        {
            get => SavableSettings.instance.markers.Contains(REMOVE_ADS_MARKER);
            set
            {
                var containsMarker = SavableSettings.instance.markers.Contains(REMOVE_ADS_MARKER);
                
                if (value && !containsMarker)
                    SavableSettings.instance.markers.Add(REMOVE_ADS_MARKER);
                
                if (!value && containsMarker)
                    SavableSettings.instance.markers.Remove(REMOVE_ADS_MARKER);
            }
        }
        
        private void Awake()
        {
            
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            global::IAPManager.Instance.InitializeIAPManager(InitializeResultCallback);
        }

        private void InitializeResultCallback(IAPOperationStatus status, string message, List<StoreProduct> shopProducts)
        {
            if (status == IAPOperationStatus.Success)
            {
                boughtRemoveAds = shopProducts.First(x => x.idGooglePlay == "com.lemon.streetzombie.ads").active;
            }
            else
            {
                Debug.Log("Easy IAP initialize error: " + message);
            }
        }

        public static void RemoveAds()
        {
            global::IAPManager.Instance.BuyProduct(ShopProductNames.RemoveAds, (status, message, product) =>
            {
                if (status == IAPOperationStatus.Success) boughtRemoveAds = true;
                if (status == IAPOperationStatus.Fail)    Debug.LogWarning("Buying RemoveAds error: " + message);
            });
        }
    }
}