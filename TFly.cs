using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using UnityEngine;
using System.Collections.Generic;
using System;
using Logger = Rocket.Core.Logging.Logger;
using Steamworks;
using Rocket.Unturned.Chat;
using SDG.Unturned;


namespace TPlugins.Fly
{
    public class TFly : RocketPlugin<TFlyConfig>
    {
        public static TFly Instance;
        public List<CSteamID> InFly = new List<CSteamID>();
        public string Version = "2.5.0";

        protected override void Load()
        {
            Instance = this;
            if (Configuration.Instance.SpeedInFly > 10 || Configuration.Instance.SpeedInFly < 0)
            {
                Configuration.Instance.SpeedInFly = 5;
                Configuration.Save();
            }
            if (Configuration.Instance.FlyUpAndDownSpeed > 1 || Configuration.Instance.FlyUpAndDownSpeed < 0)
            {
                Configuration.Instance.FlyUpAndDownSpeed = 0.5f;
                Configuration.Save();
            }
            Logger.Log("####################################", color: ConsoleColor.Yellow);
            Logger.Log("#   Thanks for downloading TFly    #", color: ConsoleColor.Yellow);
            Logger.Log("#    Plugin Created By TPlugins    #", color: ConsoleColor.Yellow);
            Logger.Log("#      Discord: TPlugins#6189      #", color: ConsoleColor.Yellow);
            Logger.Log("#       Plugin Version: " + Version + "       #", color: ConsoleColor.Yellow);
            Logger.Log("####################################", color: ConsoleColor.Yellow);
            Logger.Log("");
            Logger.Log("TFly is successfully loaded!", color: ConsoleColor.Green);
        }

        protected override void Unload()
        {
            Logger.Log("TFly is successfully unloaded!", color: ConsoleColor.Green);
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"Player_Not_Found", "[TFly] The specified player has not been found!!"},
                    { "Fly_Usage", "Usage: /fly " },
                    { "Fly_Start", "[TFly] The fly mode turn ons for you." },
                    { "Fly_Stop", "[TFly] The fly mode turn offs for you." },
                    { "Fly_Start_Another", "[TFly] The fly mode turn ons for {0}!" },
                    { "Fly_Stop_Another", "[TFly] The fly mode turn offs for {0}!" },
                    { "Fly_changed_all", "[TFly] The fly mode changed for all online player!" },
                };
            }
        }

        public void FlyMode(UnturnedPlayer player, bool enabled)
        {
            TFlyComponent cp = player.GetComponent<TFlyComponent>();

            if (cp.isFlying && !enabled)
            {
                cp.isFlying = false;
                InFly.Remove(player.CSteamID);
                player.Player.movement.tellPluginGravityMultiplier(player.CSteamID, 1);
                player.Player.movement.tellPluginSpeedMultiplier(player.CSteamID, 1);
                player.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                {
                        (byte)EPlayerStance.SWIM
                });
                UnturnedChat.Say(player, Translate("Fly_Stop"));
            }
            else if (!cp.isFlying && enabled)
            {
                cp.isFlying = true;
                InFly.Add(player.CSteamID);
                player.Player.movement.tellPluginGravityMultiplier(player.CSteamID, 0);
                player.Player.movement.tellPluginSpeedMultiplier(player.CSteamID, Configuration.Instance.SpeedInFly * 3);
                player.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                {
                        (byte)EPlayerStance.STAND
                });
                UnturnedChat.Say(player, Translate("Fly_Start"));
            }
        }

        public void Update()
        {
            foreach (var id in InFly)
            {
                var v = UnturnedPlayer.FromCSteamID(id);
                if (v.Player.input.keys[0])
                {
                    v.Player.movement.transform.position = new Vector3(v.Position.x, v.Position.y + Configuration.Instance.FlyUpAndDownSpeed, v.Position.z);
                }
                else if (v.Player.input.keys[5])
                {
                    v.Player.movement.transform.position = new Vector3(v.Position.x, v.Position.y - Configuration.Instance.FlyUpAndDownSpeed, v.Position.z);
                }
            }
        }
    }
}