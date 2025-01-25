using Items.Weapons;
using Managers;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class FloatingTextManager : MonoBehaviour
    {
        [Inject] IProjectileManager _projectileManager;
        [Inject] ICreatureEventProducer _creatureEventProducer;
        [Inject] IPoolingManager _poolingManager;

        [SerializeField] private Transform popupParent;
        [SerializeField] private FloatingTextUI floatingTextPrefab;
        [SerializeField] private Color negativeColor = Color.red;
        [SerializeField] private Color missColor = Color.yellow;
        [SerializeField] private Color positiveColor = Color.green;
        
        private void Start()
        {
            _creatureEventProducer.CreatureHit += OnCreatureHit;
            _projectileManager.ProjectileMissed += OnProjectileMissed;
        }
        


        public void SpawnFloatingText(Vector3 position, string text, Color color, float size = 1f, FontStyles fontStyle = FontStyles.Normal)
        {
            Debug.Log($"Spawned floating text at {position} with text: {text}");
            
            var popup = _poolingManager.SpawnObject<FloatingTextUI>(floatingTextPrefab, position, parent: popupParent);
            
            popup.Setup(text, color, size, fontStyle);
            popup.Run();
        }

        private void OnProjectileMissed(Projectile projectile, AttackContext attackContext, Entity entity)
        {
            SpawnFloatingText(projectile.transform.position, "Miss", missColor, size: 1.5f, fontStyle: FontStyles.Italic);
        }
        
        private void OnCreatureHit(Creature creature, HitContext hitCtx)
        {
            var color = hitCtx.Damage > 0 ? negativeColor : positiveColor;
            SpawnFloatingText(hitCtx.Target.transform.position, StringifyFoat(hitCtx.Damage), color);
        }
        
        private string StringifyFoat(float damage)
        {
            return damage.ToString(Mathf.Approximately(Mathf.Round(damage * 10) / 10f, damage) ? "F1" : "F0");
        }
    }
}