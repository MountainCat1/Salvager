
using Godot;

public partial class AttackContext
{
    public Vector2 Direction { get; set; }
    public Creature? Target { get; set; }
    public Creature Attacker { get; set; }
}