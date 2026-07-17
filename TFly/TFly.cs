using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Text;
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
            sb.AppendLine(" ▸ https://github.com/TavstalDev/TShop2/issues");
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
                    Config.DefaultFlySpeed = 10;
                    Config.Save();

                }

                if (Config.FlyUpSpeed > 1 || Config.FlyUpSpeed < 0)
                {
                    Config.FlyUpSpeed = 0.5f;
                    Config.Save();
                }

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
            
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                try
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    FlyComponent comp = player.GetComponent<FlyComponent>();
                    if (comp.IsFlying)
                        comp.SetFlightMode(false);
                }
                catch
                {
                    /* ignore */
                }
            }
            
            Logger.Info($"# {GetPluginName()} has been successfully unloaded.");
        }

        public void OnKeyDown(Player player, uint simulation, byte key, bool state)
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
                    player.Player.movement.transform.position = new Vector3(player.Position.x, player.Position.y + Config.FlyUpSpeed, player.Position.z);
                else if (player.Player.input.keys[5])
                    player.Player.movement.sendPluginGravityMultiplier(1f);
                else
                {
                    if (Config.FlyAnimationEnabled)
                        player.GetComponent<FlyComponent>().UpdateStance(EPlayerStance.SWIM);
                    player.Player.movement.sendPluginGravityMultiplier(Config.Gravity);
                }

                // The player might break their leg when landing
                if (player.Broken)
                    player.Heal(10, null, false);
            }
        }
    }
}