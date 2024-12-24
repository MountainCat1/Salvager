using System;
using System.Collections.Generic;
using System.Linq;
using CreatureControllers;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class PlayerController : MonoBehaviour
{
    [Inject] IInputMapper _inputMapper;
    [Inject] IInputManager _inputManager;
    [Inject] ISelectionManager _selectionManager;    


    private void Start()
    {
        _inputMapper.OnWorldPressed2 += OnMoveCommand;
    }

    private void OnMoveCommand(Vector2 postiion)
    {
        foreach (var creature in GetSelectedCreatures())
        {
            var controller = creature.Controller as UnitController;
            
            if(!controller)
                continue;
            
            controller.SetMoveTarget(postiion);
        }
    }
    
    private IEnumerable<Creature> GetSelectedCreatures()
    {
        return _selectionManager.SelectedCreatures;
    }
}