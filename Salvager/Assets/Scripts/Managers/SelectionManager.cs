using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public interface ISelectionManager
    {
        IEnumerable<Creature> SelectedCreatures { get; }
    }
    
    public class SelectionManager : MonoBehaviour, ISelectionManager
    {
        public IEnumerable<Creature> SelectedCreatures => GetSelectedCreatures();
        
        [SerializeField] private List<Creature> _selectedCreatures;
        
        private IEnumerable<Creature> GetSelectedCreatures()
        {
            _selectedCreatures = _selectedCreatures.Where(creature => creature != null).ToList(); // TODO: we should probably not create new list every time
            
            return _selectedCreatures;
        }

    }
}