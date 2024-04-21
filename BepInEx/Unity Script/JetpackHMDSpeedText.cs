#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using System;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDSpeedText : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [Serializable]
#endif
        public struct MetaData
        {
            public Func<float> getSpeedEvent;
            public float multiplier;
        }

        public void Init(MetaData metaData)
        {
            Transform? offset = transform.GetChild("Offset");
            if (offset == null)
                return;

            getSpeedEvent = metaData.getSpeedEvent;
            multiplier = metaData.multiplier;

            digit100 = offset.GetChild(0);
            digit10 = offset.GetChild(1);
            digit1 = offset.GetChild(2);
        }

        event Func<float>? getSpeedEvent;
        float multiplier = 1;
        Transform? digit100;
        Transform? digit10;
        Transform? digit1;
        float realSpeed = 0;
        void LateUpdate()
        {
            if (digit100 == null || digit10 == null || digit1 == null)
                return;
            
            const float fontHeight = 17.7f;
            realSpeed = Mathf.Lerp(realSpeed, Mathf.Clamp(getSpeedEvent?.Invoke() * multiplier ?? 0, 0, 999), 7.5f * Time.smoothDeltaTime);

            {
                RectTransform digit = (RectTransform)digit100;
                float speed = realSpeed * 0.01f;
                speed = Mathf.Floor(speed) + Mathf.Clamp01((Mathf.Repeat(speed, 1) - 0.99f) * 100);

                if (speed >= 1)
                    digit.anchoredPosition = new Vector2(-22, -(Mathf.Repeat(speed, 10) + 2) * fontHeight);
                else
                    digit.anchoredPosition = new Vector2(-22, -speed * fontHeight);
            }

            {
                RectTransform digit = (RectTransform)digit10;
                float speed = realSpeed * 0.1f;
                speed = Mathf.Floor(speed) + Mathf.Clamp01((Mathf.Repeat(speed, 1) - 0.9f) * 10);

                if (speed >= 1)
                    digit.anchoredPosition = new Vector2(-11, -(Mathf.Repeat(speed, 10) + 2) * fontHeight);
                else
                    digit.anchoredPosition = new Vector2(-11, -speed * fontHeight);
            }

            ((RectTransform)digit1).anchoredPosition = new Vector2(0, -Mathf.Repeat(Mathf.Clamp(realSpeed, 0, float.MaxValue), 10) * fontHeight);
        }
    }
}