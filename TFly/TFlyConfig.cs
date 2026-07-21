using Newtonsoft.Json;
using Tavstal.TLibrary.Models.Config;
using Tavstal.TLibrary.Models.Logging;
using YamlDotNet.Serialization;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tavstal.TFly
{
    public class FlyConfig : YamlConfiguration
    {
        [YamlMember(Order = 3, Description = "Forces swimming animation while flying for visual effect")]
        public bool FlyAnimationEnabled;
        [YamlMember(Order = 4, Description = "Grants invincibility to the player while flying")]
        public bool GodModeWhenFlyEnabled;
        [YamlMember(Order = 5, Description = "Cooldown in seconds between toggling flight mode")]
        public double CooldownInSeconds;
        [YamlMember(Order = 6, Description = "Base movement speed multiplier when flying")]
        public float DefaultFlySpeed;
        [YamlMember(Order = 7, Description = "Vertical movement increment per tick while flying up")]
        public float FlyUpSpeed;
        [YamlIgnore]
        public readonly float Gravity = 0f;

        public override void LoadDefaults()
        {
            General = new GeneralConfig
            {
                MessageIcon = "https://raw.githubusercontent.com/TavstalDev/TFly/refs/heads/master/assets/icon.png"
            };
            FlyAnimationEnabled = true;
            GodModeWhenFlyEnabled = true;
            CooldownInSeconds = 30;
            DefaultFlySpeed = 10f;
            FlyUpSpeed = 0.3f;
        }

        public FlyConfig() { }
        public FlyConfig(string fileName, string path) : base(fileName, path) { }
    }
}
