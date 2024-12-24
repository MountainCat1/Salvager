using System;
using System.Collections.Generic;
using CreatureControllers;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class PlayerController : MonoBehaviour
{
    [Inject] IInputMapper _inputMapper;
    [Inject] IInputManager _inputManager;
    
    [SerializeField] private List<Creature> SelectedCreatures;


    private void Start()
    {
        _inputMapper.OnWorldPressed2 += OnMoveCommand;
    }

    private void OnMoveCommand(Vector2 postiion)
    {
        foreach (var creature in SelectedCreatures)
        {
            var controller = creature.Controller as UnitController;
            
            if(!controller)
                continue;
            
            controller.SetMoveTarget(postiion);
        }
    }
}