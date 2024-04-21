#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDRoll : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [System.Serializable]
#endif
        public struct MetaData
        {
            public Transform? targetTransform;
        }

        public void Init(MetaData metaData) => targetTransform = metaData.targetTransform;

        Transform? targetTransform;
        float roll = 0;
        void LateUpdate()
        {
            if (targetTransform == null)
                return;

            Quaternion r = targetTransform.rotation * Quaternion.Euler(-90, 0, 0);
            if (targetTransform.eulerAngles.x <= 0.1f)
                r *= Quaternion.Euler(0.01f, 0, 0);
            else if (targetTransform.eulerAngles.x >= 359.9f)
                r *= Quaternion.Euler(-0.01f, 0, 0);

            if (targetTransform.eulerAngles.x >= 180f && targetTransform.eulerAngles.x <= 180.1f)
                r *= Quaternion.Euler(0.01f, 0, 0);
            else if (targetTransform.eulerAngles.x >= 179.9f && targetTransform.eulerAngles.x <= 180f)
                r *= Quaternion.Euler(-0.01f, 0, 0);

            roll = Mathf.LerpAngle(roll, -Mathf.Atan2(2.0f * ((r.x * r.y) + (r.z * r.w)), 1 - (2 * ((r.x * r.x) + (r.z * r.z)))) * Mathf.Rad2Deg, 7.5f * Time.smoothDeltaTime);
            if ((roll >= 0.01f && roll <= 0.01f) || !float.IsNormal(roll))
                roll = 0;

            transform.localEulerAngles = new Vector3(0, 0, roll);
        }
    }
}