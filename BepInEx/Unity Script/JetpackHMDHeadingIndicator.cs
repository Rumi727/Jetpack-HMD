#nullable enable
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDHeadingIndicator : MonoBehaviour
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
        float realYaw = 0;
        void Update()
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

            realYaw = Mathf.LerpAngle(realYaw, Mathf.Atan2(2 * ((r.y * r.w) + r.x * r.z), 1.0f - 2.0f * (r.x * r.x + r.y * r.y)) * Mathf.Rad2Deg, 7.5f * Time.smoothDeltaTime);

            transform.localEulerAngles = new Vector3(0, 0, realYaw);
        }
    }
}