#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using System.Linq;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public static class JetpackHMDUtility
    {
        public static Transform? GetChild(this Transform transform, string name)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform? child = transform.GetChild(i);
                if (child != null && child.name == name)
                    return child;
            }

            return null;
        }

        public static float? GetAltitude(this Transform transform)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 1000, ~LayerMask.GetMask("Player", "Enemies", "LineOfSight", "PlaceableShipObjects", "PhysicsObject", "MapHazards"), QueryTriggerInteraction.Ignore);
            if (hits.Length > 0)
                return transform.position.y - hits.Aggregate((x, y) => x.point.y > y.point.y ? x : y).point.y;

            return null;
        }
    }
}