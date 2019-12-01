using Rocket.API;
using UnityEngine;

namespace TPlugins.Fly
{
    public class TFlyConfig : IRocketPluginConfiguration
    {
        public bool UIEnabled;
        public ushort UIId;
        public bool FlyAnimationEnabled;
        public bool GodModeWhenFlyEnabled;
        public KeyCode AscendFlySpeedKeyCode;
        public KeyCode DescendFlySpeedKeyCode;
        public double CooldownInSeconds;
        public float DefaultSpeedInFly;
        public float FlyUpSpeed;
        public readonly float Gravity = 0f;
        public string Permission;
        public string PermissionAdmin;

        public void LoadDefaults()
        {
            UIEnabled = true;
            UIId = 12500;
            FlyAnimationEnabled = true;
            GodModeWhenFlyEnabled = true;
            AscendFlySpeedKeyCode = KeyCode.RightArrow;
            DescendFlySpeedKeyCode = KeyCode.LeftArrow;
            CooldownInSeconds = 30;
            DefaultSpeedInFly = 10f;
            FlyUpSpeed = 0.3f;
            Permission = "TFly.Fly";
            PermissionAdmin = "TFly.Fly.Admin";
        }
    }
}
