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
        _inputManager.Pointer1Hold += OnCharacterAttack;
        _camera = Camera.main;
    }

    private void OnCharacterAttack(Vector2 pointerPosition)
    {
        var pointerRealPosition = _camera.ScreenToWorldPoint(pointerPosition);
        var playerCharacter = GetPlayerCharacter();
        var direction = (pointerRealPosition - playerCharacter.transform.position).normalized;
        var attackContext = new AttackContext
        {
            Attacker = playerCharacter,
            Direction = direction,
        };
        
        OnPlayerCharacterAttack?.Invoke(attackContext);
    }

    private PlayerCharacter GetPlayerCharacter()
    {
        return FindObjectOfType<PlayerCharacter>();
    }

}