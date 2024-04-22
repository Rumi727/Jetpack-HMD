#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDAltimeter : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [Serializable]
#endif
        public struct MetaData
        {
            public Func<Transform?> getTargetTransform;
            public float multiplier;

            public int numberSpacing;

            public GameObject? altimeterPrefab;

            public Color color;
        }

        public void Init(MetaData metaData)
        {
            if (metaData.altimeterPrefab == null)
                return;

            Transform? offset = transform.GetChild("Offset");
            if (offset == null)
                return;

            getTargetTransform = metaData.getTargetTransform;
            multiplier = metaData.multiplier;

            numberSpacing = metaData.numberSpacing;

            for (int i = 1; i < offset.childCount; i++)
                Destroy(offset.GetChild(i).gameObject);

            if (numberSpacing <= 0)
                return;

            float pos = 0;
            int number = numberSpacing;
            while (number <= 500)
            {
                RectTransform meterRectTransform = (RectTransform)Instantiate(metaData.altimeterPrefab, offset).transform;
                meterRectTransform.anchoredPosition = new Vector2(0, pos);

                Image image = meterRectTransform.GetComponent<Image>();
                image.color = metaData.color;

                TMP_Text text = meterRectTransform.GetChild(0).GetComponent<TMP_Text>();
                text.text = number.ToString();
                text.color = metaData.color;

                pos += posSpacing;
                number += numberSpacing;
            }
        }

        event Func<Transform?>? getTargetTransform;
        float multiplier;

        const float posSpacing = 66;
        int numberSpacing = 0;

        float altitude = 0;
        void LateUpdate()
        {
            Transform? targetTransform = getTargetTransform?.Invoke();
            if (targetTransform == null)
                return;

            altitude = Mathf.Lerp(altitude, Mathf.Clamp(targetTransform.GetAltitude() * multiplier ?? 500, 0, 500), 7.5f * Time.smoothDeltaTime);
            ((RectTransform)transform).anchoredPosition = new Vector2(0, -altitude * posSpacing / numberSpacing);
        }
    }
}