#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using TMPro;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDAltimeter : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [System.Serializable]
#endif
        public struct MetaData
        {
            public Transform? targetTransform;
            public float multiplier;

            public int numberSpacing;

            public GameObject? altimeterPrefab;
        }

        public void Init(MetaData metaData)
        {
            if (metaData.altimeterPrefab == null)
                return;

            Transform? offset = transform.GetChild("Offset");
            if (offset == null)
                return;

            targetTransform = metaData.targetTransform;
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
                meterRectTransform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();

                pos += posSpacing;
                number += numberSpacing;
            }
        }

        Transform? targetTransform;
        float multiplier;

        const float posSpacing = 66;
        int numberSpacing = 0;

        float altitude = 0;
        void LateUpdate()
        {
            if (targetTransform == null)
                return;

            altitude = Mathf.Lerp(altitude, Mathf.Clamp(targetTransform.GetAltitude() * multiplier ?? 500, 0, 500), 7.5f * Time.smoothDeltaTime);
            ((RectTransform)transform).anchoredPosition = new Vector2(0, -altitude * posSpacing / numberSpacing);
        }
    }
}