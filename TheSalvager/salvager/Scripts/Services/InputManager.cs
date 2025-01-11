using System;
using System.Collections.Generic;
using System.Diagnostics;
using Constants;
using Godot;
using Services.Abstractions;

namespace Services;

public partial class InputManager : Node2D
{
    public Action ClickedOnCharacter;

    public override void _Ready()
    {
        base._Ready();

        // on escape key press go back to main menu
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GoBack();
        }
    }

    private void GoBack()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MenuUIScene.tscn");
    }

    public override void _Input(InputEvent @event)
    {
        // Check if the input is a left mouse button click
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed &&
            mouseEvent.ButtonIndex == MouseButton.Left)
        {
            // Get the clicked node under the mouse
            Node clickedNode = GetNodeUnderMouse();
            if (clickedNode is CharacterBody2D)
            {
                GD.Print($"Clicked on {clickedNode.Name}");
            }
        }
    }
    
    private Node GetNodeUnderMouse()
    {
        // Get the viewport and mouse position
        Viewport viewport = GetViewport();
        Vector2 mousePos = GetGlobalMousePosition();

        // Perform an intersection point query
        var spaceState = viewport.GetWorld2D().DirectSpaceState;
        var query = new PhysicsPointQueryParameters2D()
        {
            Position = mousePos,
            CollisionMask = (uint)CollisionMasks.Character,
            CollideWithBodies = true,
            CollideWithAreas = true
        };
        var result = spaceState.IntersectPoint(query, 32);

        int i = 0;
        foreach (var dic in result)
        {
            i++;
            foreach (var dicE in dic)
            {
                GD.Print($"{i} - {dicE.Key}: {dicE.Value}");
            }
        }

        return null;
    }
}