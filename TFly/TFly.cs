using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Text;
using Rocket.Unturned;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Logging;
using UnityEngine;

namespace Tavstal.TFly
{
    // ReSharper disable once InconsistentNaming
    public class TFly : PluginBase<FlyConfig>
    {
        public static TFly Instance { get; private set; } = null!;
        internal static readonly List<UnturnedPlayer> _flyingPlayers = new List<UnturnedPlayer>();

        public override void OnPreLoad()
        {
            Instance = this;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("████████╗███████╗██╗  ██╗   ██╗");
            sb.AppendLine("╚══██╔══╝██╔════╝██║  ╚██╗ ██╔╝");
            sb.AppendLine("   ██║   █████╗  ██║   ╚████╔╝ ");
            sb.AppendLine("   ██║   ██╔══╝  ██║    ╚██╔╝  ");
            sb.AppendLine("   ██║   ██║     ███████╗██║   ");
            sb.AppendLine("   ╚═╝   ╚═╝     ╚══════╝╚═╝   ");
            sb.AppendLine();
            sb.AppendLine("[ About ]");
            sb.AppendLine(" ▸ Developer : Tavstal");
            sb.AppendLine(" ▸ Discord   : @Tavstal");
            sb.AppendLine(" ▸ Website   : https://redstoneplugins.com");
            sb.AppendLine(" ▸ GitHub    : https://github.com/TavstalDev");
            sb.AppendLine();
            sb.AppendLine("[ Build ]");
            sb.AppendLine($" ▸ Version   : {Version}");
            sb.AppendLine($" ▸ Build Date: {BuildDate} UTC");
            sb.AppendLine($" ▸ TLibrary  : {LibraryVersion}");
            sb.AppendLine();
            sb.AppendLine("[ Support ]");
            sb.AppendLine(" ▸ Report issues or request features:");
            sb.AppendLine(" ▸ https://github.com/TavstalDev/TFly/issues");
            sb.AppendLine();
            sb.AppendLine("────────────────────────────────────────────────────────");
            Logger.Log(ELogLevel.COMMAND, sb.ToString(), includePrefixes: false, color:  ConsoleColor.Cyan);
        }
        
        public override void OnLoad()
        {
            try
            {
                if (Config.DefaultFlySpeed < 0)
                {
                    Config.DefaultFlySpeed = 5;
                    Config.Save();

                }

                if (Config.FlyUpSpeed > 1 || Config.FlyUpSpeed < 0)
                {
                    Config.FlyUpSpeed = 0.5f;
                    Config.Save();
                }

                U.Events.OnPlayerDisconnected += OnOnPlayerDisconnected;
                Logger.Info($"# {GetPluginName()} has been loaded.");
            }
            catch (Exception ex)
            {
                Logger.Error($"# Failed to load {GetPluginName()}...", ex);
            }
        }

        public override void OnUnLoad()
        {
            PlayerInput.onPluginKeyTick -= OnKeyDown;
            
            foreach (UnturnedPlayer player in _flyingPlayers)
            {
                try
                {
                    FlyComponent comp = player.GetComponent<FlyComponent>();
                    if (comp.IsFlying)
                        comp.SetFlightMode(false);
                }
                catch
                {
                    /* ignore */
                }
            }
            
            U.Events.OnPlayerDisconnected -= OnOnPlayerDisconnected;
            Logger.Info($"# {GetPluginName()} has been successfully unloaded.");
        }
        
        private void OnOnPlayerDisconnected(UnturnedPlayer player)
        {
            if (!_flyingPlayers.Contains(player))
                return;
            
            FlyComponent comp = player.GetComponent<FlyComponent>();
            comp.SetFlightMode(false);
        }

        private void OnKeyDown(Player player, uint simulation, byte key, bool state)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
            FlyComponent comp = uPlayer.GetComponent<FlyComponent>();

            if (!comp.IsFlying)
                return;

            if (!state)
                return;
            
            
            switch (key)
            {
                case 0:
                {
                    comp.SetFlySpeed(comp.FlySpeed + 1);
                    uPlayer.Player.movement.sendPluginGravityMultiplier(Config.Gravity);
                    uPlayer.Player.movement.sendPluginSpeedMultiplier(comp.FlySpeed);

                    if (Config.GodModeWhenFlyEnabled)
                        uPlayer.GodMode = true;

                    if (Config.FlyAnimationEnabled)
                        comp.UpdateStance(EPlayerStance.SWIM);
                    return;
                }
                case 1:
                {
                    if (comp.FlySpeed - 1 < 1)
                        return;

                    if (comp.FlySpeed <= 0)
                        comp.SetFlySpeed(Config.DefaultFlySpeed);
                    else
                        comp.SetFlySpeed(comp.FlySpeed - 1);

                    player.movement.sendPluginGravityMultiplier(Config.Gravity);
                    player.movement.sendPluginSpeedMultiplier(comp.FlySpeed);

                    if (Config.GodModeWhenFlyEnabled)
                        uPlayer.GodMode = true;

                    if (Config.FlyAnimationEnabled)
                        comp.UpdateStance(EPlayerStance.SWIM);

                    break;
                }
            }
        }

        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
             { "prefix", "&e[TFly] " },
             { "general_error_player_not_found", "&cThe specified player was not found."},
             { "commands_common_error_cooldown", "&cYou need to wait {0} second(s) to use this command." },
             { "commands_fly_start", "&aYour flight mode has been &2enabled&a." },
             { "commands_fly_start_other", "&aYou have &2enabled &a{0}'s flight mode." },
             { "commands_fly_stop", "&aYour flight mode has been &cdisabled&a." },
             { "commands_fly_stop_other", "&aYou have &cdisabled &a{0}'s flight mode." },
             { "commands_flyadmin_changed_all", "&aYou have changed everyone's flight mode." },
           };

        private void Update()
        {
            if (_flyingPlayers.Count == 0)
                return;
            
            foreach (var player in _flyingPlayers)
            {
                var comp = player.GetComponent<FlyComponent>();
                if (!comp.IsFlying)
                    continue;
                
                // The player might break their leg when landing
                if (player.Broken)
                    player.Broken = false;
                if (player.Bleeding)
                    player.Bleeding = false;

                if (player.Player.input.keys[0]) // Space
                {
                    player.Player.movement.sendPluginGravityMultiplier(-1f);
                    continue;
                }

                if (player.Player.input.keys[5]) // Left Shift
                {
                    player.Player.movement.sendPluginGravityMultiplier(1f);
                    continue;
                }

                if (Config.FlyAnimationEnabled && player.Stance != EPlayerStance.SWIM)
                    player.GetComponent<FlyComponent>().UpdateStance(EPlayerStance.SWIM);
                player.Player.movement.sendPluginGravityMultiplier(Config.Gravity);
            }
        }
    }
}