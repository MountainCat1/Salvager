using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Services
{
    public interface ISelectionManager
    {
        public event Action OnSelectionChanged;
        public List<Creature> SelectedCreatures { get; }
    }

    public partial class SelectionManager : Node2D, ISelectionManager
    {
        public event Action OnSelectionChanged;
        
        [Inject] private readonly ICreatureManager _creatureManager = null!;
        
        public List<Creature> SelectedCreatures { get; private set; } = new List<Creature>();

        private Vector2 _selectionStart;
        private Vector2 _selectionEnd;
        private bool _isSelecting = false;
        private Color _selectionBoxColor = new Color(0, 0, 1, 0.3f); // Semi-transparent blue

        public override void _Ready()
        {
            // Initialize or bind additional logic if needed
        }

        public override void _Process(double delta)
        {
            HandleInput();
        }

        public override void _Draw()
        { 
            if (_isSelecting)
            {
                var rect = CreateRect(_selectionStart, _selectionEnd);
                DrawRect(rect, _selectionBoxColor, filled: true);
                DrawRect(rect, Colors.White, filled: false); // Outline
            }
        }

        private Rect2 CreateRect(Vector2 start, Vector2 end)
        {
            return new Rect2(start.X, start.Y, end.X - start.X, end.Y - start.Y);
        }

        private void HandleInput()
        {
            if (Input.IsActionJustPressed("selection_start")) // Define "selection_start" in Input Map
            {
                _selectionStart = GetGlobalMousePosition();
                _isSelecting = true;
            }

            if (Input.IsActionPressed("selection_start"))
            {
                _selectionEnd = GetGlobalMousePosition();
                QueueRedraw(); // Trigger redraw
            }

            if (Input.IsActionJustReleased("selection_start"))
            {
                _selectionEnd = GetGlobalMousePosition();
                _isSelecting = false;
                QueueRedraw(); // Trigger redraw

                UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            // Normalize the selection box to work in all directions
            var selectionBox = CreateRect(
                new Vector2(Mathf.Min(_selectionStart.X, _selectionEnd.X), Mathf.Min(_selectionStart.Y, _selectionEnd.Y)),
                new Vector2(Mathf.Max(_selectionStart.X, _selectionEnd.X), Mathf.Max(_selectionStart.Y, _selectionEnd.Y))
            );

            // Clear previous selection
            SelectedCreatures.Clear();

            // Find creatures within the selection box
            foreach (Creature creature in _creatureManager.AllCreatures) // Add creatures to a group named "creatures"
            {
                if (selectionBox.HasPoint(creature.GlobalPosition))
                {
                    SelectedCreatures.Add(creature);
                }
            }

            // Notify listeners about the selection change
            OnSelectionChanged?.Invoke();
            
            GD.Print("Selected creatures: " + SelectedCreatures.Count);
            GD.Print($"Selected creatures: {string.Join(", ", SelectedCreatures.Select(X => X.Name))}");
        }
    }
}
