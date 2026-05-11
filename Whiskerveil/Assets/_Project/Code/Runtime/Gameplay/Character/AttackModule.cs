using _Project.Code.Runtime.CommonServices.RolePicker;
using UnityEngine;

namespace _Project.Code.Runtime.Gameplay.Character.Modules
{
    public class AttackModule
    {
        private readonly Transform _attackPoint;
        private readonly float _attackRadius;
        private readonly LayerMask _targetMask;

        private readonly float _cooldown;

        private float _lastAttackTime;

        public AttackModule(
            Transform attackPoint,
            float attackRadius,
            LayerMask targetMask,
            float cooldown)
        {
            _attackPoint = attackPoint;
            _attackRadius = attackRadius;
            _targetMask = targetMask;
            _cooldown = cooldown;
        }

        public bool TryAttack(out ulong hiderId)
        {
            hiderId = default;

            if (!CanAttack())
                return false;

            _lastAttackTime = Time.time;

            Collider[] hits = Physics.OverlapSphere(
                _attackPoint.position,
                _attackRadius,
                _targetMask);

            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out Character character))
                {
                    if (character.Role != GameRole.Hider)
                        continue;

                    hiderId = character.OwnerClientId;
                    return true;
                }
            }

            return false;
        }

        private bool CanAttack() =>
            Time.time >= _lastAttackTime + _cooldown;
#if UNITY_EDITOR
        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }
#endif
    }
}
