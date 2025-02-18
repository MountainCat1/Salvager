using UnityEngine;
using Zenject;

namespace Managers
{
    public interface IAstarManager
    {
        void Scan();
    }

    public class AstarManager : MonoBehaviour, IAstarManager
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
        
        public void Scan()
        {
            Debug.Log("Calling astarPath.Scan()");
            AstarPath.active.Scan();
        }
    }
}