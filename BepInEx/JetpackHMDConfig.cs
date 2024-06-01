using BepInEx.Configuration;
using LethalConfig.ConfigItems.Options;
using LethalConfig.ConfigItems;
using LethalConfig;
using System.IO;
using System;

namespace Rumi.JetpackHMD
{
    public class JetpackHMDConfig
    {
        public float scale
        {
            get => _scale?.Value ?? 0.75f;
            set => _scale.Value = value;
        }
        readonly ConfigEntry<float> _scale;



        public float pitchMeterPosSpacing
        {
            get => _pitchMeterPosSpacing?.Value ?? 75;
            set => _pitchMeterPosSpacing.Value = value;
        }
        readonly ConfigEntry<float> _pitchMeterPosSpacing;

        public int pitchMeterNumberSpacing
        {
            get => _pitchMeterNumberSpacing?.Value ?? 10;
            set => _pitchMeterNumberSpacing.Value = value;
        }
        readonly ConfigEntry<int> _pitchMeterNumberSpacing;



        public float speedometerMultiplier
        {
            get => _speedometerMultiplier?.Value ?? 1;
            set => _speedometerMultiplier.Value = value;
        }
        readonly ConfigEntry<float> _speedometerMultiplier;

        public int speedometerNumberSpacing
        {
            get => _speedometerNumberSpacing?.Value ?? 10;
            set => _speedometerNumberSpacing.Value = value;
        }
        readonly ConfigEntry<int> _speedometerNumberSpacing;



        public float altimeterMultiplier
        {
            get => _altimeterMultiplier?.Value ?? 1;
            set => _altimeterMultiplier.Value = value;
        }
        readonly ConfigEntry<float> _altimeterMultiplier;

        public int altimeterNumberSpacing
        {
            get => _altimeterNumberSpacing?.Value ?? 10;
            set => _altimeterNumberSpacing.Value = value;
        }
        readonly ConfigEntry<int> _altimeterNumberSpacing;

        public int colorR
        {
            get => _colorR?.Value ?? 0;
            set => _colorR.Value = value;
        }
        readonly ConfigEntry<int> _colorR;

        public int colorG
        {
            get => _colorG?.Value ?? 255;
            set => _colorG.Value = value;
        }
        readonly ConfigEntry<int> _colorG;

        public int colorB
        {
            get => _colorB?.Value ?? 0;
            set => _colorB.Value = value;
        }
        readonly ConfigEntry<int> _colorB;

        public int colorA
        {
            get => _colorA?.Value ?? 255;
            set => _colorA.Value = value;
        }
        readonly ConfigEntry<int> _colorA;

        public bool enableTulipSnakes
        {
            get => _enableTulipSnakes?.Value ?? true;
            set => _enableTulipSnakes.Value = value;
        }
        readonly ConfigEntry<bool> _enableTulipSnakes;



        public JetpackHMDConfig(ConfigFile config)
        {
            _scale = config.Bind("General", "Scale", 1.5f, "Adjust the overall UI scale");

            _colorR = config.Bind("General", "R Color", 0, "");
            _colorR.SettingChanged += (sender, e) => JetpackHMD.UILoad();

            _colorG = config.Bind("General", "G Color", 255, "");
            _colorG.SettingChanged += (sender, e) => JetpackHMD.UILoad();

            _colorB = config.Bind("General", "B Color", 0, "");
            _colorB.SettingChanged += (sender, e) => JetpackHMD.UILoad();

            _colorA = config.Bind("General", "A Color", 255, "");
            _colorA.SettingChanged += (sender, e) => JetpackHMD.UILoad();

            _enableTulipSnakes = config.Bind("General", "Tulip Snake Enable", true, "Displays UI even when flying as Tulip Snake");
            _enableTulipSnakes.SettingChanged += (sender, e) => JetpackHMD.UILoad();

            //Pitch Meter
            {
                _pitchMeterPosSpacing = config.Bind("Pitch Meter", "Pos Spacing", 75f);
                _pitchMeterPosSpacing.SettingChanged += (sender, e) => JetpackHMD.UILoad();

                _pitchMeterNumberSpacing = config.Bind("Pitch Meter", "Number Spacing", 10);
                _pitchMeterNumberSpacing.SettingChanged += (sender, e) => JetpackHMD.UILoad();
            }

            //Speedometer
            {
                _speedometerMultiplier = config.Bind("Speedometer", "Multiplier", 1f);
                _speedometerMultiplier.SettingChanged += (sender, e) => JetpackHMD.UILoad();

                _speedometerNumberSpacing = config.Bind("Speedometer", "Number Spacing", 10);
                _speedometerNumberSpacing.SettingChanged += (sender, e) => JetpackHMD.UILoad();
            }

            //Altimeter
            {
                _altimeterMultiplier = config.Bind("Altimeter", "Multiplier", 1f);
                _altimeterMultiplier.SettingChanged += (sender, e) => JetpackHMD.UILoad();

                _altimeterNumberSpacing = config.Bind("Altimeter", "Number Spacing", 10);
                _altimeterNumberSpacing.SettingChanged += (sender, e) => JetpackHMD.UILoad();
            }

            try
            {
                LethalConfigPatch();
            }
            catch (FileNotFoundException e)
            {
                JetpackHMD.logger?.LogError(e);
                JetpackHMD.logger?.LogWarning("Lethal Config Patch Fail! (This is not a bug and occurs when LethalConfig is not present)");
            }
            catch (Exception e)
            {
                JetpackHMD.logger?.LogError(e);
                JetpackHMD.logger?.LogError("Lethal Config Patch Fail!");
            }
        }

        void LethalConfigPatch()
        {
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_scale, new FloatSliderOptions() { Min = 0.5f, Max = 2f, RequiresRestart = false }));

            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(_colorR, new IntSliderOptions() { Min = 0, Max = 255, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(_colorG, new IntSliderOptions() { Min = 0, Max = 255, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(_colorB, new IntSliderOptions() { Min = 0, Max = 255, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(_colorA, new IntSliderOptions() { Min = 0, Max = 255, RequiresRestart = false }));

            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(_enableTulipSnakes, false));

            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_pitchMeterPosSpacing, new FloatSliderOptions() { Min = 10f, Max = 200f, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(_pitchMeterNumberSpacing, new IntSliderOptions() { Min = 1, Max = 360, RequiresRestart = false }));

            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_speedometerMultiplier, new FloatSliderOptions() { Min = 0.125f, Max = 4f, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(_speedometerNumberSpacing, new IntSliderOptions() { Min = 1, Max = 100, RequiresRestart = false }));

            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(_altimeterMultiplier, new FloatSliderOptions() { Min = 0.125f, Max = 4f, RequiresRestart = false }));
            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(_altimeterNumberSpacing, new IntSliderOptions() { Min = 1, Max = 100, RequiresRestart = false }));
        }
    }
}
