using System.Collections.Generic;
using Items.Weapons;
using Managers;
using TMPro;
using UnityEngine;
using Utilities;
using Zenject;

namespace UI
{
    public interface IFloatingTextManager
    {
        void SpawnFloatingText(Vector3 position, string text, FloatingTextType type);
    }

    public enum FloatingTextType
    {
        Damage,
        Heal,
        Miss,
        Interaction,
        InteractionCompleted
    }

    public struct FloatingTextSettings
    {
        public Color Color;
        public float Size;
        public FontStyles FontStyle;
    }

    public class FloatingTextManager : MonoBehaviour, IFloatingTextManager
    {
        [Inject] IProjectileManager _projectileManager;
        [Inject] ICreatureEventProducer _creatureEventProducer;
        [Inject] IPoolingManager _poolingManager;

        [SerializeField] private Transform popupParent;
        [SerializeField] private FloatingTextUI floatingTextPrefab;

        private Dictionary<FloatingTextType, FloatingTextSettings> _fontStyles = new()
        {
            {
                FloatingTextType.Damage,
                new FloatingTextSettings { Color = Color.red, Size = 0.5f, FontStyle = FontStyles.Normal }
            },
            {
                FloatingTextType.Heal,
                new FloatingTextSettings { Color = Color.white, Size = 0.5f, FontStyle = FontStyles.Normal }
            },
            {
                FloatingTextType.Interaction,
                new FloatingTextSettings { Color = Color.white, Size = 1f, FontStyle = FontStyles.Normal }
            },
            {
                FloatingTextType.InteractionCompleted,
                new FloatingTextSettings { Color = Color.green, Size = 1f, FontStyle = FontStyles.Normal }
            },
            {
                FloatingTextType.Miss,
                new FloatingTextSettings { Color = Color.yellow, Size = 0.75f, FontStyle = FontStyles.Italic }
            }
        };

        private void Start()
        {
            _creatureEventProducer.CreatureHit += OnCreatureHit;
            _projectileManager.ProjectileMissed += OnProjectileMissed;
        }


        public void SpawnFloatingText(Vector3 position, string text, FloatingTextType type)
        {
            position = position.RoundToNearest(1f / 16f);
            
            Debug.Log($"Spawned floating text at {position} with text: {text}");

            var popup = _poolingManager.SpawnObject<FloatingTextUI>(floatingTextPrefab, position, parent: popupParent);

            var settings = _fontStyles[type];
            popup.Setup(text, settings.Color, settings.Size, settings.FontStyle);
            popup.Run();
        }

        private void OnProjectileMissed(Projectile projectile, AttackContext attackContext, Entity entity)
        {
            SpawnFloatingText(projectile.transform.position, "Miss", FloatingTextType.Miss);
        }

        private void OnCreatureHit(Creature creature, HitContext hitCtx)
        {
            SpawnFloatingText(hitCtx.Target.transform.position, StringifyFloat(hitCtx.Damage), FloatingTextType.Damage);
        }

        private string StringifyFloat(float damage)
        {
            return damage.ToString(Mathf.Approximately(Mathf.Round(damage * 10) / 10f, damage) ? "F1" : "F0");
        }
    }
}