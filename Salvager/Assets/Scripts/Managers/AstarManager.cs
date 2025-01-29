using UnityEngine;
using Zenject;

namespace Managers
{
    public class AstarManager : MonoBehaviour
    {
        [SerializeField] private AstarPath astarPath;

        [Inject] private IMapGenerator _mapGenerator;
        
        private void Start()
        {
            _mapGenerator.MapGeneratedLate += OnMapGenerated;
        }

        private void OnMapGenerated()
        {
            astarPath.Scan();
        }
    }
}