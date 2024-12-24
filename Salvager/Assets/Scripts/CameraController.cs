using System;
using Managers;
using UnityEngine;
using UnityEngine.Animations;
using Zenject;

public class CameraController : MonoBehaviour
{
    [SerializeField] private ParentConstraint parentConstraint;
    [SerializeField] private Animator animator;
    
    [Inject] IPlayerCharacterProvider _playerProvider;
    
    private static readonly int PlayerDeath = Animator.StringToHash("PlayerDeath");

    private void Start()
    {
        var player = _playerProvider.Get();
        player.Death += OnPlayerDeath;
    }

    private void OnPlayerDeath(DeathContext obj)
    {
        parentConstraint.constraintActive = false;
        animator.SetTrigger(PlayerDeath);
    }
}
