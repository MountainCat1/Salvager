using Components;

public class TouchWeapon : Weapon
{
    protected override void Attack(AttackContext ctx)
    {
        var damage = WeaponItemData.GetApplied(WeaponPropertyModifiers.Damage, BaseDamage);
        
        var hitCtx = new HitContext()
        {
            Attacker = ctx.Attacker,
            Damage = CalculateDamage(damage, ctx),
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