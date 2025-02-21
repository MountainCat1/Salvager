using System;
using Data;

[Serializable]
public class ShopData
{
    public int itemCount;
    public InventoryData inventory;
    public float priceMultiplier;

    public decimal GetBuyPrice(ItemData itemData)
    {
        decimal price = itemData.Value * (decimal)priceMultiplier;
        return Math.Ceiling(price);
    }

    public decimal GetSellPrice(ItemData itemData)
    {
        decimal price = itemData.Value * (decimal)priceMultiplier * 0.5m;
        return Math.Floor(price);
    }

    public decimal GetFuelPrice()
    {
        return Math.Ceiling(10 * (decimal)priceMultiplier);
    }

    public decimal GetJuicePrice()
    {
        return Math.Ceiling(5 * (decimal)priceMultiplier);
    }
}