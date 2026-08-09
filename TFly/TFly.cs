using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Text;
using Tavstal.TFly.Handlers;
using Tavstal.TFly.Utils;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Logging;

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

                UnturnedPlayerHandler.Attach();
                Logger.Info($"# {GetPluginName()} has been loaded.");
            }
            catch (Exception ex)
            {
                Logger.Error($"# Failed to load {GetPluginName()}...", ex);
            }
        }

        public override void OnUnLoad()
        {
            foreach (UnturnedPlayer player in _flyingPlayers)
            {
                try
                {
                    FlyComponent? comp = ComponentManager.Get(player);
                    if (comp  == null)
                        continue;
                    
                    if (comp.IsFlying)
                        comp.SetFlightMode(false);
                }
                catch
                {
                    /* ignore */
                }
            }

            UnturnedPlayerHandler.Detach();
            Logger.Info($"# {GetPluginName()} has been successfully unloaded.");
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
                var comp = ComponentManager.Get(player);
                if (!comp)
                    continue;
                
                if (!comp!.IsFlying)
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
                    comp.UpdateStance(EPlayerStance.SWIM);
                player.Player.movement.sendPluginGravityMultiplier(Config.Gravity);
            }
        }
    }
}