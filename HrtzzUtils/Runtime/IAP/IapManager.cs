#if IAP_IMPORTED
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

[RequireComponent(typeof(UnityInAppPurchaser))]
public class IapManager : AutoReferenceer<IapManager>
{
	private UnityInAppPurchaser m_UnityInAppPurchaser;
	public HashSet<InAppItem> inAppItems = new HashSet<InAppItem>();

	[Header("Purchaser Callbacks")]
	public OnPurchaserInitializationSuccess onInitialisationSuccess;

	public OnInitializationFailed onInitialisationFailed;
	public OnPurchaseSuccess onPurchaseSuccess;
	public OnPurchaseFailed onPurchaseFailed;

	protected override void Awake()
	{
		base.Awake();
		m_UnityInAppPurchaser = GetComponent<UnityInAppPurchaser>();
	}

	public void InitialiseInAppItems()
	{
		m_UnityInAppPurchaser.InitPurchaser();
	}

	public void PurchaseProduct(string productSku)
	{
		m_UnityInAppPurchaser.BuyProductWithId(productSku);
	}
}

[Serializable]
public class OnInitializationFailed : UnityEvent<InitializationFailureReason>
{
}

[Serializable]
public class OnPurchaserInitializationSuccess : UnityEvent<Product[]>
{
}

[Serializable]
public class OnPurchaseSuccess : UnityEvent<Product>
{
}

[Serializable]
public class OnPurchaseFailed : UnityEvent<Product, PurchaseFailureReason>
{
}
#endif