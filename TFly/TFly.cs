using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Extensions;
using UnityEngine;

namespace Tavstal.TFly
{
    // ReSharper disable once InconsistentNaming
    public class TFly : PluginBase<FlyConfig>
    {
        public static TFly Instance { get; private set; }
        private List<SteamPlayer> _flyingPlayers = new List<SteamPlayer>();

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
            Logger.Log($"# Build Version: {Version}");
            Logger.Log($"# Build Date: {BuildDate}");
            Logger.Log("#########################################");
            Logger.Log("# TFly has been loaded.");
        }

        public override void OnUnLoad()
        {
            PlayerInput.onPluginKeyTick -= KeyDown;
            try
            {
                foreach (SteamPlayer steamPlayer in _flyingPlayers)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    FlyComponent comp = player.GetComponent<FlyComponent>();
                    if (comp.IsFlying)
                    {
                        comp.SetFlightMode(false);
                    }
                }
            } catch { /* ignore */ }
            Logger.Log("# TShop has been successfully unloaded.");
            _flyingPlayers = new List<SteamPlayer>();
        }

        public void KeyDown(Player player, uint simulation, byte key, bool state)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
            FlyComponent comp = uPlayer.GetComponent<FlyComponent>();

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
                        comp.UpdateStance(EPlayerStance.SWIM);
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
                            comp.UpdateStance(EPlayerStance.SWIM);
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
                        player.GetComponent<FlyComponent>().UpdateStance(EPlayerStance.SWIM);
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