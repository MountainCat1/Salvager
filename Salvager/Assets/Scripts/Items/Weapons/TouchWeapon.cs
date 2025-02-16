public class TouchWeapon : Weapon
{
    protected override void Attack(AttackContext ctx)
    {
        var hitCtx = new HitContext()
        {
            Attacker = ctx.Attacker,
            Damage = CalculateDamage(BaseDamage, ctx),
            Target = ctx.Target,
            PushFactor = PushFactor
        }; 
        
        OnHit(ctx.Target, hitCtx);
    }

    protected override void OnHit(Creature target, HitContext hitContext)
    {
        base.OnHit(target, hitContext);
    }
}