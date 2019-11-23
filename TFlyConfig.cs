using Rocket.API;

namespace TPlugins.Fly
{
    public class TFlyConfig : IRocketPluginConfiguration
    {
        public string Permission;
        public float SpeedInFly;
        public float FlyUpAndDownSpeed;

        public void LoadDefaults()
        {
            Permission = "TFly.Fly";
            SpeedInFly = 5;
            FlyUpAndDownSpeed = 0.5f;
        }
    }
}
