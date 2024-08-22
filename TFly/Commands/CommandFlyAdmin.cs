using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TFly.Commands
{
    public class CommandFlyAdmin : CommandBase
    {
        protected override IPlugin Plugin => TFly.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "flyadmin";
        public override string Help => "Moderates flight mode";
        public override string Syntax => "[player] <enable/disable/on/off> | all <enable/disable/on/off>";
        public override List<string> Aliases => new List<string> { "flya", "flighta", "flightadmin" };
        public override List<string> Permissions => new List<string> { "tfly.commands.flyadmin" };
        protected override List<SubCommand> SubCommands => new List<SubCommand>
        {
            new SubCommand("all", "Changes everyone's flight mode,", "all <enable/disable/on/off>", new List<string>(), new List<string>(), 
                (caller, args) =>
                {
                    bool? flyMode = null;
                    if (args.Length == 2)
                    {
                        if (args[1].ToLower() == "disable" || args[1].ToLower() == "off")
                            flyMode = false;
                        else if (args[1].ToLower() == "enable" || args[1].ToLower() == "on")
                            flyMode = true;
                    }

                    foreach (SteamPlayer steamPlayer in Provider.clients)
                    {
                        UnturnedPlayer target = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                        TFlyComponent comp = target.GetComponent<TFlyComponent>();
                        if (flyMode != null)
                        {
                            comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                            comp.SetIsFlying(flyMode.Value);
                        }
                        else
                        {
                            if (comp.IsFlying)
                            {
                                comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                                comp.SetIsFlying(false);
                            }
                            else
                            {
                                comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                                comp.SetIsFlying(true);
                            }
                        }
                    }
                    Plugin.SendCommandReply(caller, "success_fly_changed_all");
                    return Task.CompletedTask;
                })
        };
        protected override Task<bool> ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            if (args.Length > 2 || args.Length == 0)
                return Task.FromResult(false);

            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
            if (targetPlayer == null)
            {
                Plugin.SendCommandReply(caller, "error_player_not_found", args[0]);
                return Task.FromResult(false);
            }

            TFlyComponent comp = targetPlayer.GetComponent<TFlyComponent>();
            bool flyMode = !comp.IsFlying;
            if (args.Length == 2)
            {
                if (args[1].ToLower() == "disable" || args[1].ToLower() == "off")
                    flyMode = false;
                else if (args[1].ToLower() == "enable" || args[1].ToLower() == "on")
                    flyMode = true;
            }
            comp.SetFlightMode(flyMode);
            comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
            Plugin.SendCommandReply(caller, "success_fly_changed_all");
            return Task.FromResult(true);
        }
    }
}
