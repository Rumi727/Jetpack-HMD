using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rumi.JetpackHMD
{
    [BepInPlugin(modGuid, modName, modVersion)]
    [BepInDependency("ainavt.lc.lethalconfig")]
    public class JetpackHMD : BaseUnityPlugin
    {
        public const string modGuid = "Rumi.JetpackHMD";
        public const string modName = "JetpackHMD";
        public const string modVersion = "1.0.5";
        
        public static Assembly currentAssembly => _currentAssembly ??= Assembly.GetExecutingAssembly();
        static Assembly? _currentAssembly;

        public static string assemblyName => _assemblyName ??= currentAssembly.FullName.Split(',')[0];
        static string? _assemblyName;

        public static ManualLogSource? logger { get; private set; }
        public static ConfigFile? config { get; private set; }

        public static JetpackHMDConfig? uiConfig { get; private set; }

        public static Harmony harmony { get; } = new Harmony(modGuid);

        public static JetpackHMDManager? manager { get; private set; }

        void Awake()
        {
            logger = Logger;
            config = Config;

            uiConfig = new JetpackHMDConfig(config);

            logger?.LogInfo($"Asset Bundle Loading...");
            mainAssetBundle ??= LoadMainAssetBundle();

            logger?.LogInfo($"Patch...");
            harmony.PatchAll(typeof(JetpackHMDPatches));

            logger?.LogInfo($"Event...");
            SceneManager.sceneLoaded += OnSceneRelayLoaded;

            logger?.LogInfo($"Plugin {modName} is loaded!");
        }

        static void OnSceneRelayLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == "SampleSceneRelay")
                UILoad();
        }

        public static void UILoad()
        {
#pragma warning disable IDE0031 // Null 전파 사용
            JetpackHMDManager.MetaData metaData = new JetpackHMDManager.MetaData()
            {
                getEnableEvent = () => JetpackHMDPatches.playerControllerB != null && JetpackHMDPatches.playerControllerB.jetpackControls,
                getScaleEvent = () => uiConfig?.scale ?? 1,

                roll = new JetpackHMDRoll.MetaData()
                {
                    getTargetTransform = () => JetpackHMDPatches.playerControllerB != null ? JetpackHMDPatches.playerControllerB.transform : null
                },
                pitchMeter = new JetpackHMDPitchMeter.MetaData()
                {
                    getTargetTransform = () => JetpackHMDPatches.playerControllerB != null ? JetpackHMDPatches.playerControllerB.transform : null,

                    posSpacing = uiConfig?.pitchMeterPosSpacing ?? 75,
                    numberSpacing = uiConfig?.pitchMeterNumberSpacing ?? 10,

                    minusPitchMeterPrefab = LoadPrefab("Minus Pitch Meter"),
                    plusPitchMeterPrefab = LoadPrefab("Plus Pitch Meter")
                },

                speedText = new JetpackHMDSpeedText.MetaData()
                {
                    getSpeedEvent = () =>
                    {
                        if (JetpackHMDPatches.playerControllerB != null)
                            return JetpackHMDPatches.playerControllerB.thisController.velocity.magnitude;

                        return 0;
                    },
                    multiplier = uiConfig?.speedometerMultiplier ?? 1
                },
                speedometer = new JetpackHMDSpeedometer.MetaData()
                {
                    getSpeedEvent = () =>
                    {
                        if (JetpackHMDPatches.playerControllerB != null)
                            return JetpackHMDPatches.playerControllerB.thisController.velocity.magnitude;

                        return 0;
                    },
                    multiplier = uiConfig?.speedometerMultiplier ?? 1,

                    numberSpacing = uiConfig?.speedometerNumberSpacing ?? 10,
                    speedMeterPrefab = LoadPrefab("Speedometer")
                },

                altitude = new JetpackHMDAltitude.MetaData()
                {
                    getTargetTransform = () => JetpackHMDPatches.playerControllerB != null ? JetpackHMDPatches.playerControllerB.transform : null,
                    multiplier = uiConfig?.altimeterMultiplier ?? 1
                },
                altimeter = new JetpackHMDAltimeter.MetaData()
                {
                    getTargetTransform = () => JetpackHMDPatches.playerControllerB != null ? JetpackHMDPatches.playerControllerB.transform : null,
                    multiplier = uiConfig?.altimeterMultiplier ?? 1,

                    numberSpacing = uiConfig?.altimeterNumberSpacing ?? 10,
                    altimeterPrefab = LoadPrefab("Altimeter")
                },

                headingIndicator = new JetpackHMDHeadingIndicator.MetaData()
                {
                    getTargetTransform = () => JetpackHMDPatches.playerControllerB != null ? JetpackHMDPatches.playerControllerB.transform : null
                },

                accelerationIndicator = new JetpackHMDAccelerationIndicator.MetaData()
                {
                    getSpeedEvent = () =>
                    {
                        if (JetpackHMDPatches.playerControllerB != null)
                            return JetpackHMDPatches.playerControllerB.externalForces.magnitude;

                        return 0;
                    },
                    multiplier = uiConfig?.speedometerMultiplier ?? 1
                },
                verticalSpeedIndicator = new JetpackHMDVerticalSpeedIndicator.MetaData()
                {
                    getSpeedEvent = () =>
                    {
                        if (JetpackHMDPatches.playerControllerB != null)
                            return JetpackHMDPatches.playerControllerB.thisController.velocity.y;

                        return 0;
                    },
                    multiplier = uiConfig?.speedometerMultiplier ?? 1
                }
            };
#pragma warning restore IDE0031 // Null 전파 사용

            GameObject? lethalHUD = GameObject.Find("IngamePlayerHUD");
            GameObject? jetpackHMDGameObject = LoadPrefab("Jetpack HMD");

            if (lethalHUD == null)
            {
                logger?.LogFatal($"{nameof(lethalHUD)} is null");
                return;
            }
            if (jetpackHMDGameObject == null)
            {
                logger?.LogFatal($"{nameof(jetpackHMDGameObject)} == null");
                return;
            }

            if (manager == null)
                manager = Instantiate(jetpackHMDGameObject, lethalHUD.transform).AddComponent<JetpackHMDManager>();

            manager.Init(metaData);
        }

        static GameObject? LoadPrefab(string prefabName)
        {
            try
            {
                if (mainAssetBundle == null)
                {
                    logger?.LogError($"{mainAssetBundle} is null");
                    return null;
                }

                return mainAssetBundle.LoadAsset<GameObject>(prefabName);
            }
            catch (Exception e)
            {
                logger?.LogError(e);
                logger?.LogError($"\"{prefabName}\" Prefab load failure");

                return null;
            }
        }



        public const string mainAssetBundleName = "JetpackHMD";
        public static AssetBundle? mainAssetBundle { get; private set; }

        public static AssetBundle? LoadMainAssetBundle()
        {
            string path = "Rumi.JetpackHMD" + "." + mainAssetBundleName;

            try
            {
                using Stream assetStream = currentAssembly.GetManifestResourceStream(path);
                return AssetBundle.LoadFromStream(assetStream);
            }
            catch (Exception e)
            {
                logger?.LogFatal(e);
                logger?.LogFatal($"Main Asset bundle loading failed (Path : {path}");

                return null;
            }
        }
    }
}
