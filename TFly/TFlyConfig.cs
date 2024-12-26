using Newtonsoft.Json;
using Tavstal.TLibrary.Models.Plugin;
using UnityEngine;

namespace Tavstal.TFly
{
    public class FlyConfig : ConfigurationBase
    {
        [JsonProperty(Order = 3)]
        public bool FlyAnimationEnabled;
        [JsonProperty(Order = 4)]
        public bool GodModeWhenFlyEnabled;
        [JsonProperty(Order = 5)]
        public KeyCode AscendFlySpeedKeyCode;
        [JsonProperty(Order = 6)]
        public KeyCode DescendFlySpeedKeyCode;
        [JsonProperty(Order = 7)]
        public double CooldownInSeconds;
        [JsonProperty(Order = 8)]
        public float DefaultFlySpeed;
        [JsonProperty(Order = 9)]
        public float FlyUpSpeed;
        [JsonIgnore]
        public readonly float Gravity = 0f;

        public override void LoadDefaults()
        {
            DebugMode = false;
            Locale = "en";
            DownloadLocalePacks = true;
            FlyAnimationEnabled = true;
            GodModeWhenFlyEnabled = true;
            AscendFlySpeedKeyCode = KeyCode.RightArrow;
            DescendFlySpeedKeyCode = KeyCode.LeftArrow;
            CooldownInSeconds = 30;
            DefaultFlySpeed = 10f;
            FlyUpSpeed = 0.3f;
        }

        public FlyConfig() { }
        public FlyConfig(string fileName, string path) : base(fileName, path) { }
    }
}
