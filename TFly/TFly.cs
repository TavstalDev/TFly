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
using Tavstal.TLibrary;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TFly
{
    public class TFly : PluginBase<TFlyConfig>
    {
        public new static TFly Instance { get; private set; }
        public new static TLogger Logger = new TLogger("TFly", false);
        internal List<SteamPlayer> FlyingPlayers = new List<SteamPlayer>();

        public override void OnLoad()
        {
            Instance = this;
            if (Config.DefaultFlySpeed < 0)
            {
                Config.DefaultFlySpeed = 10;
                Config.SaveConfig();
                
            }
            if (Config.FlyUpSpeed > 1 || Config.FlyUpSpeed < 0)
            {
                Config.FlyUpSpeed = 0.5f;
                Config.SaveConfig();
            }
            
            Logger.LogWarning("████████╗███████╗██╗  ██╗   ██╗");
            Logger.LogWarning("╚══██╔══╝██╔════╝██║  ╚██╗ ██╔╝");
            Logger.LogWarning("   ██║   █████╗  ██║   ╚████╔╝ ");
            Logger.LogWarning("   ██║   ██╔══╝  ██║    ╚██╔╝  ");
            Logger.LogWarning("   ██║   ██║     ███████╗██║   ");
            Logger.LogWarning("   ╚═╝   ╚═╝     ╚══════╝╚═╝   ");
            Logger.Log("#########################################");
            Logger.Log("# Thanks for using my plugin");
            Logger.Log("# Plugin Created By Tavstal");
            Logger.Log("# Discord: Tavstal#6189");
            Logger.Log("# Website: https://redstoneplugins.com");
            Logger.Log("# Discord: https://discord.gg/redstoneplugins");
            Logger.Log("#########################################");
            Logger.Log(string.Format("# Build Version: {0}", Version));
            Logger.Log(string.Format("# Build Date: {0}", BuildDate));
            Logger.Log("#########################################");
            Logger.Log("# TFly has been loaded.");
        }

        public override void OnUnLoad()
        {
            PlayerInput.onPluginKeyTick -= KeyDown;
            try
            {
                foreach (SteamPlayer steamPlayer in FlyingPlayers)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    TFlyComponent comp = player.GetComponent<TFlyComponent>();
                    if (comp.IsFlying)
                    {
                        comp.SetFlightMode(false);
                    }
                }
            } catch { }
            Logger.Log("# TShop has been successfully unloaded.");

        }

        public void KeyDown(Player player, uint simulation, byte key, bool state)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
            TFlyComponent comp = uPlayer.GetComponent<TFlyComponent>();

            if (comp.IsFlying)
            {
                if (key == 0 && state)
                {
                    comp.SetFlySpeed(comp.FlySpeed + 1);
                    uPlayer.Player.movement.sendPluginGravityMultiplier(Config.Gravity);
                    uPlayer.Player.movement.sendPluginSpeedMultiplier(comp.FlySpeed);

                    if (Config.GodModeWhenFlyEnabled)
                        uPlayer.GodMode = true;

                    if (Config.FlyAnimationEnabled)
                    {
                        /*uPlayer.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                        {
                        (byte)EPlayerStance.SWIM
                        });*/
                        player.stance.stance = EPlayerStance.SWIM;
                    }
                }
                else if (key == 1 && state)
                {
                    if (comp.FlySpeed - 1 > 1)
                    {
                        if (comp.FlySpeed <= 0)
                            comp.SetFlySpeed(Config.DefaultFlySpeed);
                        else
                            comp.SetFlySpeed(comp.FlySpeed - 1);

                        player.movement.sendPluginGravityMultiplier(Config.Gravity);
                        player.movement.sendPluginSpeedMultiplier(comp.FlySpeed);

                        if (Config.GodModeWhenFlyEnabled)
                            uPlayer.GodMode = true;

                        if (Config.FlyAnimationEnabled)
                        {
                            /*player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                            {
                                (byte)EPlayerStance.SWIM
                            });*/
                            player.stance.stance = EPlayerStance.SWIM;
                        }

                    }
                }
            }
        }

        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
             { "prefix", "&e[TFly]" },
             { "success_fly_start", "&aYour flight mode has been &2enabled&a." },
             { "success_fly_stop", "&aYour flight mode has been &cdisabled&a." },
             { "success_fly_start_another", "&aYou have &2enabled &a{0}'s flight mode." },
             { "success_fly_stop_another", "&aYou have &cdisabled &a{0}'s flight mode." },
             { "success_fly_changed_all", "&aYou have changed everyone's flight mode." },
             { "error_player_not_found", "&cThe specified player was not found."},
             { "error_command_cooldown", "&cYou need to wait {0} second(s) to use this command." }
           };

        private void Update()
        {
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                if (player.Player.input.keys[0])
                {
                    player.Player.movement.transform.position = new Vector3(player.Position.x, player.Position.y + Config.FlyUpSpeed, player.Position.z);
                }
                else if (player.Player.input.keys[5])
                {
                    player.Player.movement.sendPluginGravityMultiplier(1f);
                }
                else
                {
                    if (Config.FlyAnimationEnabled)
                    {
                        /*player.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
                        {
                        (byte)EPlayerStance.SWIM
                        });*/
                        player.Player.stance.stance = EPlayerStance.SWIM;
                    }
                    player.Player.movement.sendPluginGravityMultiplier(Config.Gravity);
                }

                // The player might broke their leg when landing
                if (player.Broken)
                    player.Heal(10, null, false);
            }
        }
    }
}