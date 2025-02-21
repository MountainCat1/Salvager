using System;

[Serializable]
public class ShopData
{
    public int itemCount;
    public InventoryData inventory;
    public float priceMultiplier;

    public decimal GetBuyPrice(decimal baseCost)
    {
        decimal price = baseCost * (decimal)priceMultiplier;
        return Math.Ceiling(price);
    }

    public decimal GetSellPrice(decimal baseCost)
    {
        decimal price = baseCost * (decimal)priceMultiplier * 0.5m;
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