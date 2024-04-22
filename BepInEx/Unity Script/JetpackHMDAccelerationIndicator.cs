#nullable enable
using System;
using TMPro;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDAccelerationIndicator : MonoBehaviour
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
            getSpeedEvent = metaData.getSpeedEvent;
            multiplier = metaData.multiplier;

            text = GetComponent<TMP_Text>();
        }

        event Func<float>? getSpeedEvent;
        float multiplier = 1;

        TMP_Text? text;
        float realSpeed = 0;
        void Update()
        {
            if (text == null)
                return;

            realSpeed = Mathf.Lerp(realSpeed, getSpeedEvent?.Invoke() * multiplier ?? 0, 7.5f * Time.smoothDeltaTime);
            text.text = "<voffset=2.5><size=11>ACC</size></voffset> " + Mathf.Round(realSpeed);
        }
    }
}