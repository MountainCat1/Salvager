using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming

[Serializable]
public class LocationData
{
    // World data
    public Guid Id;
    public string Name;
    public Vector2 Position;
    public Guid[] NeighbourIds;
    public float EnemySpawnManaPerSecond;
    
    [NonSerialized] public List<LocationData> Neighbours = new();
    
    // Player data
    public bool Visited;
    
    // Land data
    public GenerateMapSettingsData MapSettings;
    public ICollection<LocationFeatureData> Features = new List<LocationFeatureData>();
    
    // Shop data
    public ShopData ShopData = null;
    
    // Misc 
    public LocationType Type;
}