using Rocket.API;
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
        public Dictionary<CSteamID, DateTime> Cooldowns = new Dictionary<CSteamID, DateTime>();
        public string Version = "3.0.0";

        protected override void Load()
        {
            Instance = this;
            if (Configuration.Instance.DefaultSpeedInFly < 0)
            {
                Configuration.Instance.DefaultSpeedInFly = 10;
                Configuration.Save();
            }
            if (Configuration.Instance.FlyUpSpeed > 1 || Configuration.Instance.FlyUpSpeed < 0)
            {
                Configuration.Instance.FlyUpSpeed = 0.5f;
                Configuration.Save();
            }
            PlayerInput.onPluginKeyTick += KeyDown;
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
            PlayerInput.onPluginKeyTick -= KeyDown;
            InFly.Clear();
            Cooldowns.Clear();
            try
            {
                UnturnedPlayer p = UnturnedPlayer.FromPlayer(Player.player);
                TFlyComponent cp = p.GetComponent<TFlyComponent>();
                if (cp.isFlying)
                {
                    FlyMode(p, false);
                }
            } catch { }
            Logger.Log("TFly is successfully unloaded!", color: ConsoleColor.Green);
        }

        public void KeyDown(Player player, uint simulation, byte key, bool state)
        {
            UnturnedPlayer p = UnturnedPlayer.FromPlayer(player);
            TFlyComponent cp = p.GetComponent<TFlyComponent>();

            if (cp.isFlying)
            {
                if (key == 0 && state)
                {
                    if (cp.FlySpeed == 0)
                    {
                        cp.FlySpeed = Configuration.Instance.DefaultSpeedInFly;
                    }
                    cp.FlySpeed = cp.FlySpeed + 1;
                    p.Player.movement.sendPluginGravityMultiplier(Configuration.Instance.Gravity);
                    p.Player.movement.sendPluginSpeedMultiplier(cp.FlySpeed);

                    if (Configuration.Instance.GodModeWhenFlyEnabled)
                        p.GodMode = true;

                    if (Configuration.Instance.FlyAnimationEnabled)
                    {
                        p.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                        {
                        (byte)EPlayerStance.SWIM
                        });
                    }

                    if (Configuration.Instance.UIEnabled)
                        EffectManager.sendUIEffect(Configuration.Instance.UIId, 8731, p.CSteamID, true, Translate("Your_Flying_Speed", cp.FlySpeed.ToString()));
                }
                if (key == 1 && state)
                {
                    if (cp.FlySpeed - 1 > 1)
                    {
                        if (cp.FlySpeed == 0)
                        {
                            cp.FlySpeed = Configuration.Instance.DefaultSpeedInFly;
                        }
                        cp.FlySpeed = cp.FlySpeed - 1;
                        p.Player.movement.sendPluginGravityMultiplier(Configuration.Instance.Gravity);
                        p.Player.movement.sendPluginSpeedMultiplier(cp.FlySpeed);

                        if (Configuration.Instance.GodModeWhenFlyEnabled)
                            p.GodMode = true;

                        if (Configuration.Instance.FlyAnimationEnabled)
                        {
                            p.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                            {
                        (byte)EPlayerStance.SWIM
                            });
                        }

                        if (Configuration.Instance.UIEnabled)
                            EffectManager.sendUIEffect(Configuration.Instance.UIId, 8731, p.CSteamID, true, Translate("Your_Flying_Speed", cp.FlySpeed.ToString()));
                    }
                }
            }
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
                    { "Cooldown", "[TFly] You need to wait {0} second(s) to use this command!" },
                    { "Your_Flying_Speed", "Your flying speed is {0}" }
                };
            }
        }

        public void FlyMode(UnturnedPlayer player, bool enabled)
        {
            TFlyComponent cp = player.GetComponent<TFlyComponent>();

            float flyspeed = Configuration.Instance.DefaultSpeedInFly;
            if (cp.FlySpeed != 0)
            {
                flyspeed = cp.FlySpeed;
            }

            if (cp.isFlying && !enabled)
            {
                cp.isFlying = false;
                InFly.Remove(player.CSteamID);
                player.Player.movement.sendPluginGravityMultiplier(1f);
                player.Player.movement.sendPluginSpeedMultiplier(1f);

                if (Configuration.Instance.GodModeWhenFlyEnabled)
                    player.GodMode = false;

                if (Configuration.Instance.FlyAnimationEnabled)
                {
                    player.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                    {
                        (byte)EPlayerStance.STAND
                    });
                }
                if (Configuration.Instance.UIEnabled)
                    EffectManager.askEffectClearByID(Configuration.Instance.UIId, player.CSteamID);
                UnturnedChat.Say(player, Translate("Fly_Stop"));
            }
            else if (!cp.isFlying && enabled)
            {
                cp.isFlying = true;
                InFly.Add(player.CSteamID);
                player.Player.movement.sendPluginGravityMultiplier(Configuration.Instance.Gravity);
                player.Player.movement.sendPluginSpeedMultiplier(flyspeed);

                if (Configuration.Instance.GodModeWhenFlyEnabled)
                    player.GodMode = true;

                if (Configuration.Instance.FlyAnimationEnabled)
                {
                    player.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                    {
                        (byte)EPlayerStance.SWIM
                    });
                }
                if (Configuration.Instance.UIEnabled)
                    EffectManager.sendUIEffect(Configuration.Instance.UIId, 8731, player.CSteamID, true, Translate("Your_Flying_Speed", cp.FlySpeed.ToString()));
                UnturnedChat.Say(player, Translate("Fly_Start"));
            }
        }

        public void Update()
        {
            foreach (CSteamID id in InFly)
            {
                var v = UnturnedPlayer.FromCSteamID(id);
                if (v.Player.input.keys[0])
                {
                    v.Player.movement.transform.position = new Vector3(v.Position.x, v.Position.y + Configuration.Instance.FlyUpSpeed, v.Position.z);
                }
                else if (v.Player.input.keys[5])
                {
                    v.Player.movement.sendPluginGravityMultiplier(1f);
                }
                else
                {
                    if (Configuration.Instance.FlyAnimationEnabled)
                    {
                        v.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                        {
                        (byte)EPlayerStance.SWIM
                        });
                    }
                    v.Player.movement.sendPluginGravityMultiplier(Configuration.Instance.Gravity);
                }
                if (v.Broken)
                {
                    v.Broken = false;
                }
            }
        }
    }
}