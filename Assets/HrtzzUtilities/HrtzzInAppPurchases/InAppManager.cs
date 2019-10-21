#if UNITY_IAP
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

[RequireComponent(typeof(UnityInAppPurchaser))]
public class InAppManager : HrtzzSingletonClass<InAppManager>
{
    UnityInAppPurchaser unityPurchaser;
    public List<InAppProduct> inAppItems;

    [Space(20)]
    public OnInitializationSuccess onInitialisationSuccess;
    public OnInitializationFailed onInitialisationFailed;
    public OnPurchaseSuccess onPurchaseSuccess;
    public OnPurchaseFailed onPurchaseFailed;

    public delegate void BuyProductDelegate(string productSKU);
    public static event BuyProductDelegate OnBuyProductEvent;

    protected override void Awake()
    {
        base.Awake();
        unityPurchaser = GetComponent<UnityInAppPurchaser>();
    }

    private void OnEnable()
    {
        OnBuyProductEvent += unityPurchaser.BuyProductWithID;
    }

    public void PurcahseProduct(string productSKU)
    {
        if (OnBuyProductEvent != null)
        {
            OnBuyProductEvent(productSKU);
        }
    }

    private void OnDisable()
    {
        OnBuyProductEvent -= unityPurchaser.BuyProductWithID;
    }
}

[System.Serializable]
public class InAppProduct
{
    public string productName;
    public string productSKU;
    public ProductType productType;
}

[System.Serializable]
public class OnInitializationFailed : UnityEvent<InitializationFailureReason> { }

[System.Serializable]
public class OnInitializationSuccess : UnityEvent<Product[]> { }

[System.Serializable]
public class OnPurchaseSuccess : UnityEvent<Product> { }

[System.Serializable]
public class OnPurchaseFailed : UnityEvent<Product, PurchaseFailureReason> { }
#endif