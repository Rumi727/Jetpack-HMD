#nullable enable
using System;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDHeadingIndicator : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [Serializable]
#endif
        public struct MetaData
        {
            public Func<Transform?> getTargetTransform;
        }

        public void Init(MetaData metaData) => getTargetTransform = metaData.getTargetTransform;

        event Func<Transform?>? getTargetTransform;
        float realYaw = 0;
        void Update()
        {
            Transform? targetTransform = getTargetTransform?.Invoke();
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