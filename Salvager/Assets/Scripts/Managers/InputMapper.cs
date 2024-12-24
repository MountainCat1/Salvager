using System;
using UnityEngine;
using Zenject;

public interface IInputMapper
{
    public event Action<AttackContext> OnPlayerCharacterAttack;
}

public class InputMapper : MonoBehaviour, IInputMapper
{
    public event Action<AttackContext> OnPlayerCharacterAttack;
    
    [Inject] private IInputManager _inputManager;
    private Camera _camera;

    [Inject]
    private void Construct()
    {
        _camera = Camera.main;
    }
}