#if UNITY_2017_1_OR_NEWER
#nullable enable
#endif
using System;
using System.Linq;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    public sealed class JetpackHMDManager : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        [Serializable]
#endif
        public struct MetaData
        {
            public Func<bool> getEnableEvent;
            public Func<float> getScaleEvent;

            public JetpackHMDRoll.MetaData roll;
            public JetpackHMDPitchMeter.MetaData pitchMeter;

            public JetpackHMDSpeedText.MetaData speedText;
            public JetpackHMDSpeedometer.MetaData speedometer;

            public JetpackHMDAltitude.MetaData altitude;
            public JetpackHMDAltimeter.MetaData altimeter;

            public JetpackHMDAccelerationIndicator.MetaData accelerationIndicator;
            public JetpackHMDVerticalSpeedIndicator.MetaData verticalSpeedIndicator;

            public JetpackHMDHeadingIndicator.MetaData headingIndicator;
        }

#if UNITY_2017_1_OR_NEWER
        public bool enable = false;
        public float scale = 1;
        public float speed = 0;
        public float altitude = 0;
        public MetaData metaData;

        void Awake()
        {
            metaData.getEnableEvent += () => enable;

            metaData.getScaleEvent += () => scale;

            metaData.speedText.getSpeedEvent += () => speed;
            metaData.speedometer.getSpeedEvent += () => speed;

            metaData.accelerationIndicator.getSpeedEvent += () => speed;
            metaData.verticalSpeedIndicator.getSpeedEvent += () => speed;
        }
#endif
        void OnEnable() => Canvas.preWillRenderCanvases += ScaleUpdate;
        void OnDisable() => Canvas.preWillRenderCanvases -= ScaleUpdate;

        CanvasGroup? canvasGroup;
        void Update()
        {
            if (canvasGroup == null)
                return;

            if (metaData.getEnableEvent?.Invoke() ?? false)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, 4 * Time.smoothDeltaTime);
                if (canvasGroup.alpha >= 0.99f)
                    canvasGroup.alpha = 1;

                Transform? enableObject = transform.GetChild(0);
                if (enableObject != null && !enableObject.gameObject.activeSelf)
                    enableObject.gameObject.SetActive(true);
            }
            else
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0, 4 * Time.smoothDeltaTime);

                Transform? enableObject = transform.GetChild(0);
                if (canvasGroup.alpha <= 0 && enableObject != null && enableObject.gameObject.activeSelf)
                    enableObject.gameObject.SetActive(false);
            }
        }

#if UNITY_2017_1_OR_NEWER && UNITY_EDITOR
        MetaData lastMetaData;
#endif
        CanvasGroup? jetpackMeterContainer;
        bool jetpackWarningCompatibleDisable = true;
        void LateUpdate()
        {
#if UNITY_2017_1_OR_NEWER && UNITY_EDITOR
            if (!metaData.Equals(lastMetaData))
                Init(metaData);

            lastMetaData = metaData;
#endif
            if (canvasGroup == null)
                return;

            if (!jetpackWarningCompatibleDisable)
            {
                GameObject? jetpackMeterContainerObject = null;
                {
                    GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        GameObject gameObject = gameObjects[i];
                        if (gameObject.name == "jetpackMeterContainer")
                            jetpackMeterContainerObject = gameObject;
                    }
                }

                if (jetpackMeterContainerObject != null)
                {
                    jetpackMeterContainer = jetpackMeterContainerObject.GetComponent<CanvasGroup>();

                    if (jetpackMeterContainer != null)
                    {
                        RectTransform rectTransform = (RectTransform)jetpackMeterContainer.transform;

                        rectTransform.anchorMin = new Vector2(0.59f, 0.5f);
                        rectTransform.anchorMax = new Vector2(0.59f, 0.5f);

                        rectTransform.sizeDelta = new Vector2(18, 78.5f);
                        rectTransform.pivot = new Vector2(0, 0.5f);
                    }
                }

                jetpackWarningCompatibleDisable = true;
            }

            if (jetpackMeterContainer != null)
            {
                if (canvasGroup.alpha > 0 && !jetpackMeterContainer.gameObject.activeSelf)
                    jetpackMeterContainer.gameObject.SetActive(true);

                jetpackMeterContainer.alpha = canvasGroup.alpha;

                float scale = Mathf.Clamp(metaData.getScaleEvent?.Invoke() ?? 1, 0.5f, 2f) * 0.5f;
                ((RectTransform)jetpackMeterContainer.transform).anchoredPosition = new Vector2(72 * scale, 0);
            }
        }

        void ScaleUpdate()
        {
            float scale = Mathf.Clamp(metaData.getScaleEvent?.Invoke() ?? 1, 0.5f, 2f) * 0.5f;

            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.localScale = Vector3.one * scale;

            Vector3 size = ((RectTransform)rectTransform.parent).rect.size / scale;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

#if !UNITY_2017_1_OR_NEWER
        MetaData metaData;
#endif
        public void Init(MetaData metaData)
        {
#if !UNITY_2017_1_OR_NEWER
            this.metaData = metaData;
#endif
            canvasGroup = GetComponent<CanvasGroup>();
            jetpackWarningCompatibleDisable = false;

            ScaleUpdate();

            Transform? enableObject = transform.GetChild(0);
            if (enableObject != null)
            {
                Transform? rollMask = enableObject.GetChild("Roll Mask");
                if (rollMask != null)
                {
                    Transform? roll = rollMask.GetChild("Roll");
                    if (roll != null)
                    {
                        if (!roll.gameObject.TryGetComponent<JetpackHMDRoll>(out JetpackHMDRoll? jetpackHMDRoll))
                            jetpackHMDRoll = roll.gameObject.AddComponent<JetpackHMDRoll>();

                        jetpackHMDRoll.Init(metaData.roll);

                        Transform? pitchMeter = roll.GetChild("Pitch Meter");
                        if (pitchMeter != null)
                        {
                            if (!pitchMeter.gameObject.TryGetComponent<JetpackHMDPitchMeter>(out JetpackHMDPitchMeter? jetpackHMDPitchMeter))
                                jetpackHMDPitchMeter = pitchMeter.gameObject.AddComponent<JetpackHMDPitchMeter>();

                            jetpackHMDPitchMeter.Init(metaData.pitchMeter);
                        }
                    }
                }

                Transform? speedMask = enableObject.GetChild("Speed Mask");
                if (speedMask != null)
                {
                    Transform? speedText = speedMask.GetChild("Speed Text");
                    if (speedText != null)
                    {
                        if (!speedText.gameObject.TryGetComponent<JetpackHMDSpeedText>(out JetpackHMDSpeedText? jetpackHMDSpeedText))
                            jetpackHMDSpeedText = speedText.gameObject.AddComponent<JetpackHMDSpeedText>();

                        jetpackHMDSpeedText.Init(metaData.speedText);
                    }

                    Transform? speedMeterMask = speedMask.GetChild("Speedometer Mask");
                    if (speedMeterMask != null)
                    {
                        Transform? speedMeter = speedMeterMask.GetChild("Speedometer");
                        if (speedMeter != null)
                        {
                            if (!speedMeter.gameObject.TryGetComponent<JetpackHMDSpeedometer>(out JetpackHMDSpeedometer? jetpackHMDSpeedMeter))
                                jetpackHMDSpeedMeter = speedMeter.gameObject.AddComponent<JetpackHMDSpeedometer>();

                            jetpackHMDSpeedMeter.Init(metaData.speedometer);
                        }
                    }
                }

                Transform? altitudeMask = enableObject.GetChild("Altitude Mask");
                if (altitudeMask != null)
                {
                    Transform? altitude = altitudeMask.GetChild("Altitude");
                    if (altitude != null)
                    {
                        if (!altitude.gameObject.TryGetComponent<JetpackHMDAltitude>(out JetpackHMDAltitude? jetpackHMDAltitude))
                            jetpackHMDAltitude = altitude.gameObject.AddComponent<JetpackHMDAltitude>();

                        jetpackHMDAltitude.Init(metaData.altitude);
                    }

                    Transform? altimeterMask = altitudeMask.GetChild("Altimeter Mask");
                    if (altimeterMask != null)
                    {
                        Transform? altimeter = altimeterMask.GetChild("Altimeter");
                        if (altimeter != null)
                        {
                            if (!altimeter.gameObject.TryGetComponent<JetpackHMDAltimeter>(out JetpackHMDAltimeter? jetpackHMDAltimeter))
                                jetpackHMDAltimeter = altimeter.gameObject.AddComponent<JetpackHMDAltimeter>();

                            jetpackHMDAltimeter.Init(metaData.altimeter);
                        }
                    }
                }

                Transform? accelerationIndicator = enableObject.GetChild("Acceleration Indicator");
                if (accelerationIndicator != null)
                {
                    if (!accelerationIndicator.TryGetComponent<JetpackHMDAccelerationIndicator>(out var jetpackHMDAccelerationIndicator))
                        jetpackHMDAccelerationIndicator = accelerationIndicator.gameObject.AddComponent<JetpackHMDAccelerationIndicator>();

                    jetpackHMDAccelerationIndicator.Init(metaData.accelerationIndicator);
                }

                Transform? verticalSpeedIndicator = enableObject.GetChild("Vertical Speed Indicator");
                if (verticalSpeedIndicator != null)
                {
                    if (!verticalSpeedIndicator.TryGetComponent<JetpackHMDVerticalSpeedIndicator>(out var jetpackHMDVerticalSpeedIndicator))
                        jetpackHMDVerticalSpeedIndicator = verticalSpeedIndicator.gameObject.AddComponent<JetpackHMDVerticalSpeedIndicator>();

                    jetpackHMDVerticalSpeedIndicator.Init(metaData.verticalSpeedIndicator);
                }

                Transform? headingIndicatorMask = enableObject.GetChild("Heading Indicator Mask");
                if (headingIndicatorMask != null)
                {
                    Transform? headingIndicator = headingIndicatorMask.GetChild("Heading Indicator");
                    if (headingIndicator != null)
                    {
                        if (!headingIndicator.gameObject.TryGetComponent<JetpackHMDHeadingIndicator>(out JetpackHMDHeadingIndicator? jetpackHMDHeadingIndicator))
                            jetpackHMDHeadingIndicator = headingIndicator.gameObject.AddComponent<JetpackHMDHeadingIndicator>();

                        jetpackHMDHeadingIndicator.Init(metaData.headingIndicator);
                    }
                }
            }
        }
    }
}