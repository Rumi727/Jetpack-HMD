#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using TMPro;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDPitchMeter : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [System.Serializable]
#endif
        public struct MetaData
        {
            public Transform? targetTransform;

            public float posSpacing;
            public int numberSpacing;

            public GameObject? minusPitchMeterPrefab;
            public GameObject? plusPitchMeterPrefab;
        }

        public void Init(MetaData metaData)
        {
            if (metaData.targetTransform == null || metaData.minusPitchMeterPrefab == null || metaData.plusPitchMeterPrefab == null)
                return;

            targetTransform = metaData.targetTransform;

            posSpacing = metaData.posSpacing;
            numberSpacing = metaData.numberSpacing;

            for (int i = 1; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);

            if (metaData.numberSpacing <= 0)
                return;

            float pos = 0;
            int number = 0;
            while (number < 180)
            {
                pos += posSpacing;
                number += numberSpacing;

                GameObject minusPitchMeter = Instantiate(metaData.minusPitchMeterPrefab, transform);

                {
                    RectTransform meterRectTransform = (RectTransform)minusPitchMeter.transform;
                    meterRectTransform.anchoredPosition = new Vector2(0, -pos);

                    TMP_Text leftText = minusPitchMeter.transform.GetChild(0).GetComponent<TMP_Text>();
                    TMP_Text rightText = minusPitchMeter.transform.GetChild(1).GetComponent<TMP_Text>();

                    leftText.text = number.ToString();
                    rightText.text = number.ToString();
                }

                GameObject plusPitchMeter = Instantiate(metaData.plusPitchMeterPrefab, transform);

                {
                    RectTransform meterRectTransform = (RectTransform)plusPitchMeter.transform;
                    meterRectTransform.anchoredPosition = new Vector2(0, pos);

                    TMP_Text leftText = plusPitchMeter.transform.GetChild(0).GetComponent<TMP_Text>();
                    TMP_Text rightText = plusPitchMeter.transform.GetChild(1).GetComponent<TMP_Text>();

                    leftText.text = number.ToString();
                    rightText.text = number.ToString();
                }
            }
        }

        Transform? targetTransform;

        float posSpacing = 0;
        int numberSpacing = 0;
        float pitch = 0;
        void LateUpdate()
        {
            if (targetTransform == null)
                return;

            Quaternion r = targetTransform.rotation * Quaternion.Euler(-90, 0, 0);
            if (targetTransform.eulerAngles.x <= 0.1f)
                r *= Quaternion.Euler(0.04f, 0, 0);
            else if (targetTransform.eulerAngles.x >= 359.9f)
                r *= Quaternion.Euler(-0.04f, 0, 0);

            float result = Mathf.Asin(2.0f * ((r.x * r.w) - (r.y * r.z))) * Mathf.Rad2Deg;
            if ((result + 90 >= -0.08f && result + 90 <= 0.08f) || !float.IsNormal(pitch))
                pitch = Mathf.LerpAngle(pitch, -90, 7.5f * Time.smoothDeltaTime);
            else
                pitch = Mathf.LerpAngle(pitch, result, 7.5f * Time.smoothDeltaTime);

            ((RectTransform)transform).anchoredPosition = new Vector2(0, pitch * posSpacing / numberSpacing);
        }
    }
}