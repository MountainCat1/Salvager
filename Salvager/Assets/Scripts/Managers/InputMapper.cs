﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public class MoveCommandContext
{
    public Vector2 To { get; set; }
}

public interface IInputMapper
{
    public event Action<Vector2> OnWorldPressed1;
    public event Action<Vector2> OnWorldPressed2;
    ICollection<Entity> GetEntitiesUnderMouse();
    Entity GetEntityUnderMouse();
}

public class InputMapper : MonoBehaviour, IInputMapper
{
    public event Action<Vector2> OnWorldPressed1;
    public event Action<Vector2> OnWorldPressed2;

    [Inject] private IInputManager _inputManager;
    private Camera _camera;

    [Inject]
    private void Construct()
    {
        _camera = Camera.main;

        _inputManager.Pointer2Pressed += OnPointer2Pressed;
        _inputManager.Pointer1Pressed += OnPointer1Pressed;
    }

    private void OnPointer1Pressed(Vector2 obj)
    {
        var worldPosition = _camera.ScreenToWorldPoint(obj);
        OnWorldPressed1?.Invoke(worldPosition);
    }

    private void OnPointer2Pressed(Vector2 position)
    {
        var worldPosition = _camera.ScreenToWorldPoint(position);
        OnWorldPressed2?.Invoke(worldPosition);
    }
    
    public ICollection<Entity> GetEntitiesUnderMouse()
    {
        var entities = new List<Entity>();
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var entity = hit.collider.GetComponent<Entity>();
            if (entity != null)
            {
                entities.Add(entity);
            }
        }

        return entities;
    }
    
    [CanBeNull]
    public Entity GetEntityUnderMouse()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var entity = hit.collider.GetComponent<Entity>();
            if (entity != null)
            {
                return entity;
            }
        }

        return null;
    }
}