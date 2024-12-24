using UnityEngine;

namespace CreatureControllers
{
    public class RangeEnemyController : AiController
    {
        private Creature _target;

        [SerializeField] private bool moveOnAttackCooldown = false;

        private void Update()
        {
            Creature.SetMovement(Vector2.zero);

            if (!_target)
            {
                _target = GetNewTarget();

                if (!_target)
                {
                    return;
                }
            }

            var attackContext = new AttackContext()
            {
                Direction = (_target.transform.position - Creature.transform.position).normalized,
                Target = _target,
                Attacker = Creature
            };


            if (Creature.Weapon.GetOnCooldown(attackContext) && !moveOnAttackCooldown)
                return;


            if (IsInRange(_target, Creature.Weapon.Range) && PathClear(_target, 0.5f)) // TODO: Magic number, its the radius of the creature of a size of a human
            {
                PerformAttack(attackContext);
                return;
            }

            PerformMovementTowardsTarget(_target);
        }

        private void PerformAttack(AttackContext ctx)
        {
            Creature.Weapon.ContiniousAttack(ctx);
        }
    }
}