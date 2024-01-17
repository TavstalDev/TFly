using Rocket.API;
using Tavstal.TLibrary.Compatibility;
using UnityEngine;

namespace Tavstal.TFly
{
    public class TFlyConfig : ConfigurationBase
    {
        public bool FlyAnimationEnabled;
        public bool GodModeWhenFlyEnabled;
        public KeyCode AscendFlySpeedKeyCode;
        public KeyCode DescendFlySpeedKeyCode;
        public double CooldownInSeconds;
        public float DefaultFlySpeed;
        public float FlyUpSpeed;
        public readonly float Gravity = 0f;
        public string Permission;
        public string PermissionAdmin;

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
    }
}
