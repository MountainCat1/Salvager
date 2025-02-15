using System;
using Data;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Zenject;

namespace UI
{
    public class ItemShopEntryUI : ItemEntryUI
    {
        [Inject] private IItemManager _itemManager;
        [Inject] private IDataResolver _dataResolver;
        
        [SerializeField] private TextMeshProUGUI priceDisplay;


        public void SetShopData(ShopData shopData)
        {
            priceDisplay.text = $"{shopData.GetBuyPrice(ItemData)}$";
        }
    }
}