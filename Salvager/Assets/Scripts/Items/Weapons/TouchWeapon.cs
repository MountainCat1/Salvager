using Components;

public class TouchWeapon : Weapon
{
    protected override void Attack(AttackContext ctx)
    {
        var hitCtx = new HitContext()
        {
            Attacker = ctx.Attacker,
            Damage = CalculateDamage(Damage, ctx),
            Target = ctx.Target,
            PushFactor = PushFactor
        }; 
        
        OnHit(ctx.Target, hitCtx);
    }

    protected override void OnHit(IDamageable damageable, HitContext hitContext)
    {
        base.OnHit(damageable, hitContext);
    }
}