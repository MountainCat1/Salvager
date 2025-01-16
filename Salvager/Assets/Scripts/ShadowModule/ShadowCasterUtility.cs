using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ShadowModule
{
    public static class ShadowCasterUtility
    {
        // Reflection-related fields
        private static readonly BindingFlags accessFlagsPrivate = BindingFlags.NonPublic | BindingFlags.Instance;

        private static readonly FieldInfo meshField =
            typeof(ShadowCaster2D).GetField("m_Mesh", accessFlagsPrivate);

        private static readonly FieldInfo shapePathField =
            typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);

        private static readonly MethodInfo onEnableMethod =
            typeof(ShadowCaster2D).GetMethod("OnEnable", accessFlagsPrivate);


        public static ShadowCaster2D UpdateShadowCasterShape(GameObject target, Vector2[] positions,
            bool selfShadows = true)
        {
            return UpdateShadowCasterShape(target, positions.Select(x => new Vector3(x.x, x.y, 0)).ToArray(), selfShadows);
        }

        /// <summary>
        /// Updates (or adds) a ShadowCaster2D component on the target GameObject
        /// by assigning the specified positions to its shape path.
        /// </summary>
        /// <param name="target">The GameObject to which the ShadowCaster2D will be attached/updated.</param>
        /// <param name="positions">An array of Vector3 positions for the shadow shape.</param>
        /// <param name="selfShadows">Whether the ShadowCaster2D should cast shadows on itself.</param>
        /// <returns>The updated ShadowCaster2D component.</returns>
        public static ShadowCaster2D UpdateShadowCasterShape(GameObject target, Vector3[] positions,
            bool selfShadows = true)
        {
            if (target == null || positions == null || positions.Length == 0)
            {
                Debug.LogWarning("ShadowCasterUtility: Invalid target or positions array.");
                return null;
            }

            // Get or add the ShadowCaster2D component
            ShadowCaster2D shadowCaster = target.GetComponent<ShadowCaster2D>();
            if (shadowCaster == null)
            {
                shadowCaster = target.AddComponent<ShadowCaster2D>();
            }

            // Apply user preference for self-shadows
            shadowCaster.selfShadows = selfShadows;

            // Use reflection to set the shadow shape path
            shapePathField.SetValue(shadowCaster, positions);

            // Force the ShadowCaster2D to rebuild its mesh
            meshField.SetValue(shadowCaster, null);

            // Invoke the private OnEnable method to update the ShadowCaster2D
            onEnableMethod.Invoke(shadowCaster, new object[0]);

            return shadowCaster;
        }
    }
}