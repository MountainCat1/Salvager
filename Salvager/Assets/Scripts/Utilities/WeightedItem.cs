using System;
using System.Collections.Generic;
using LevelSelector.Managers;
using UnityEngine;

[Serializable]
public abstract class WeightedBagBase
{
    public abstract float TotalWeight { get; }

    public abstract void RecalculateTotalWeight();

    public abstract object GetRandomItemObject();
}

[Serializable]
public abstract class WeightedBag<T> : WeightedBagBase
{
    [SerializeField] private List<WeightedItem<T>> items = new();
    private float totalWeight;

    public override float TotalWeight => totalWeight;

    public override void RecalculateTotalWeight()
    {
        totalWeight = 0f;
        foreach (var entry in items)
        {
            totalWeight += entry.Weight;
        }
    }

    public void AddItem(T item, float weight)
    {
        items.Add(new WeightedItem<T>(item, weight));
        RecalculateTotalWeight();
    }

    public void RemoveItem(T item)
    {
        items.RemoveAll(i => EqualityComparer<T>.Default.Equals(i.Item, item));
        RecalculateTotalWeight();
    }

    public T GetRandomItem()
    {
        RecalculateTotalWeight();
        
        if (items.Count == 0)
            throw new InvalidOperationException("The weighted bag is empty.");

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var entry in items)
        {
            cumulativeWeight += entry.Weight;
            if (randomValue <= cumulativeWeight)
                return entry.Item;
        }

        return default; // Should never reach this point
    }

    public ICollection<T> GetRandomItems(int count)
    {
        List<T> result = new();
        for (int i = 0; i < count; i++)
        {
            result.Add(GetRandomItem());
        }
        return result;
    }


    // Needed to satisfy the base class, so it can return a generic item
    public override object GetRandomItemObject() => GetRandomItem();
}

[Serializable]
public class WeightedItem<T>
{
    [SerializeField] private T item;
    [SerializeField] private float weight;

    public T Item => item;
    public float Weight => weight;

    public WeightedItem(T item, float weight = 1)
    {
        this.item = item;
        this.weight = weight;
    }
}


[Serializable]
public class WeightedLocationFeature : WeightedBag<LocationFeature>
{
}