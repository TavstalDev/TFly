using Newtonsoft.Json;
using Tavstal.TLibrary.Compatibility;
using UnityEngine;

namespace Tavstal.TFly
{
    public class TFlyConfig : ConfigurationBase
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
        [JsonProperty(Order = 10)]
        public string Permission;
        [JsonProperty(Order = 11)]
        public string PermissionAdmin;
        [JsonIgnore]
        public readonly float Gravity = 0f;

        public override void LoadDefaults()
        {
            FlyAnimationEnabled = true;
            GodModeWhenFlyEnabled = true;
            AscendFlySpeedKeyCode = KeyCode.RightArrow;
            DescendFlySpeedKeyCode = KeyCode.LeftArrow;
            CooldownInSeconds = 30;
            DefaultFlySpeed = 10f;
            FlyUpSpeed = 0.3f;
            Permission = "TFly.Fly";
            PermissionAdmin = "TFly.Fly.Admin";
        }

        public TFlyConfig() { }
        public TFlyConfig(string fileName, string path) : base(fileName, path) { }
    }
}
