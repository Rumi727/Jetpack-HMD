#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using System;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDAltitude : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [Serializable]
#endif
        public struct MetaData
        {
            public Func<Transform?> getTargetTransform;
            public float multiplier;
        }

        public void Init(MetaData metaData)
        {
            Transform? offset = transform.GetChild("Offset");
            if (offset == null)
                return;

            getTargetTransform = metaData.getTargetTransform;
            multiplier = metaData.multiplier;

            digit100 = offset.GetChild(0);
            digit10 = offset.GetChild(1);
            digit1 = offset.GetChild(2);
            digit0_01 = offset.GetChild(3);
        }

        event Func<Transform?>? getTargetTransform;
        float multiplier = 1;
        Transform? digit100;
        Transform? digit10;
        Transform? digit1;
        Transform? digit0_01;
        float realAltitude = 0;
        void LateUpdate()
        {
            Transform? targetTransform = getTargetTransform?.Invoke();
            if (targetTransform == null || digit100 == null || digit10 == null || digit1 == null || digit0_01 == null)
                return;
            
            const float fontHeight = 17.7f;
            const float fontHeight0_01 = 7.965f;

            realAltitude = Mathf.Lerp(realAltitude, Mathf.Clamp(targetTransform.GetAltitude() * multiplier ?? 999, 0, 999), 7.5f * Time.smoothDeltaTime);

            {
                RectTransform digit = (RectTransform)digit100;
                float altitude = realAltitude * 0.01f;
                altitude = Mathf.Floor(altitude) + Mathf.Clamp01((Mathf.Repeat(altitude, 1) - 0.99f) * 100);

                if (altitude >= 1)
                    digit.anchoredPosition = new Vector2(-22, -(Mathf.Repeat(altitude, 10) + 2) * fontHeight);
                else
                    digit.anchoredPosition = new Vector2(-22, -altitude * fontHeight);
            }

            {
                RectTransform digit = (RectTransform)digit10;
                float altitude = realAltitude * 0.1f;
                altitude = Mathf.Floor(altitude) + Mathf.Clamp01((Mathf.Repeat(altitude, 1) - 0.9f) * 10);

                if (altitude >= 1)
                    digit.anchoredPosition = new Vector2(-11, -(Mathf.Repeat(altitude, 10) + 2) * fontHeight);
                else
                    digit.anchoredPosition = new Vector2(-11, -altitude * fontHeight);
            }

            ((RectTransform)digit1).anchoredPosition = new Vector2(0, -Mathf.Repeat(Mathf.Clamp(realAltitude, 0, float.MaxValue), 10) * fontHeight);

            {
                RectTransform digit = (RectTransform)digit0_01;
                float altitude = Mathf.Clamp(realAltitude * 5, 0, float.MaxValue);
                
                digit.anchoredPosition = new Vector2(11.5f, -Mathf.Repeat(altitude, 5) * fontHeight0_01);
            }
        }
    }
}