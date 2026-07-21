using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
// ReSharper disable UnusedType.Global

namespace Tavstal.TFly.Commands
{
    public class CommandFlyAdmin : CustomCommandBase
    {
        public override IPlugin Plugin => TFly.Instance;
        public override bool UseBackgroundThread => false;

        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "flyadmin";
        public override string Help => "Moderates flight mode";
        public override string Syntax => "[player] <enable/disable/on/off> | all <enable/disable/on/off>";
        public override List<string> Aliases => new List<string> { "flya", "flighta", "flightadmin" };
        public override List<string> Permissions => new List<string> { "tfly.commands.flyadmin" };
        public override List<ISubcommand> SubCommands => new List<ISubcommand>
        {
            new SubCommand("all", "Changes everyone's flight mode,", "all <enable/disable/on/off>", new List<string>(), new List<string>(), 
                Plugin, AllowedCaller,
                (caller, args) =>
                {
                    bool? flyMode = null;
                    if (args.Length == 2)
                    {
                        switch (args[1].ToLower())
                        {
                            case "disable":
                            case "off":
                                flyMode = false;
                                break;
                            case "enable":
                            case "on":
                                flyMode = true;
                                break;
                        }
                    }

                    foreach (SteamPlayer steamPlayer in Provider.clients)
                    {
                        UnturnedPlayer target = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                        FlyComponent comp = target.GetComponent<FlyComponent>();
                        if (flyMode != null)
                        {
                            comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                            comp.SetIsFlying(flyMode.Value);
                            continue;
                        }

                        if (comp.IsFlying)
                        {
                            comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                            comp.SetIsFlying(false);
                            continue;
                        }

                        comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                        comp.SetIsFlying(true);
                    }
                    Plugin.SendCommandReply(caller, "commands_flyadmin_changed_all", TFly.Instance.Config.General.MessageIcon);
                    return Task.CompletedTask;
                })
        };


        protected override bool HandleExecute(IRocketPlayer caller, string[] args)
        {
            if (args.Length > 2 || args.Length == 0)
                return false;

            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
            if (targetPlayer == null)
            {
                Plugin.SendCommandReply(caller, "general_error_player_not_found", TFly.Instance.Config.General.MessageIcon, args[0]);
                return true;
            }

            FlyComponent comp = targetPlayer.GetComponent<FlyComponent>();
            bool flyMode = !comp.IsFlying;
            if (args.Length == 2)
            {
                switch (args[1].ToLower())
                {
                    case "disable":
                    case "off":
                        flyMode = false;
                        break;
                    case "enable":
                    case "on":
                        flyMode = true;
                        break;
                }
            }
            comp.SetFlightMode(flyMode);
            comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
            Plugin.SendCommandReply(caller, flyMode ? "commands_fly_start_other" : "commands_fly_stop_other", TFly.Instance.Config.General.MessageIcon, targetPlayer.CharacterName);
            return true;
        }
    }
}
