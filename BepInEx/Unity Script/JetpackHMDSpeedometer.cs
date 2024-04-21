#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using System;
using TMPro;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDSpeedometer : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [Serializable]
#endif
        public struct MetaData
        {
            public Func<float> getSpeedEvent;
            public float multiplier;

            public int numberSpacing;

            public GameObject? speedMeterPrefab;
        }

        public void Init(MetaData metaData)
        {
            if (metaData.speedMeterPrefab == null)
                return;

            Transform? offset = transform.GetChild("Offset");
            if (offset == null)
                return;

            getSpeedEvent = metaData.getSpeedEvent;
            multiplier = metaData.multiplier;

            numberSpacing = metaData.numberSpacing;

            for (int i = 1; i < offset.childCount; i++)
                Destroy(offset.GetChild(i).gameObject);

            if (numberSpacing <= 0)
                return;

            float pos = 0;
            int number = numberSpacing;
            while (number <= 200)
            {
                RectTransform meterRectTransform = (RectTransform)Instantiate(metaData.speedMeterPrefab, offset).transform;

                meterRectTransform.anchoredPosition = new Vector2(0, pos);
                meterRectTransform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();

                pos += posSpacing;
                number += numberSpacing;
            }
        }

        event Func<float>? getSpeedEvent;
        float multiplier;

        const float posSpacing = 66;
        int numberSpacing = 0;

        float speed = 0;
        void LateUpdate()
        {
            speed = Mathf.Lerp(speed, Mathf.Clamp(getSpeedEvent?.Invoke() * multiplier ?? 0, 0, 200), 7.5f * Time.smoothDeltaTime);
            ((RectTransform)transform).anchoredPosition = new Vector2(0, -speed * posSpacing / numberSpacing);
        }
    }
}