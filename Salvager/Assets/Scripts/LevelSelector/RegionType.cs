using System.Collections.Generic;
using UnityEngine;

namespace LevelSelector
{
    [CreateAssetMenu(fileName = "RegionType", menuName = "Custom/RegionType")]
    public class RegionType : ScriptableObject
    {
        [SerializeField] public string typeName;
        [SerializeField] [TextArea] public string typeDescription;
        [SerializeField] public WeightedLocationFeature originLocationFeatures;
        [SerializeField] public WeightedLocationFeature endLocationFeatures;
        [SerializeField] public WeightedLocationFeature weightedLocationFeatures;
        [SerializeField] public WeightedLocationFeature weightedSecondaryLocationFeatures;
        [SerializeField] public int maxSecondaryFeatures = 3;
        [SerializeField] public int minSecondaryFeatures = 1;
    }
}