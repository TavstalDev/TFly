using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using static UnityEngine.GraphicsBuffer;
using Tavstal.TLibrary.Helpers.Unturned;
using Pathfinding.Ionic.Zlib;

namespace Tavstal.TFly
{
    public class CommandFlyAdmin : CommandBase
    {
        public override IPlugin Plugin => TFly.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "flyadmin";
        public override string Help => "Moderates flight mode";
        public override string Syntax => "[player] <enable/disable/on/off> | all <enable/disable/on/off>";
        public override List<string> Aliases => new List<string>() { "flya", "flighta", "flightadmin" };
        public override List<string> Permissions => new List<string> { TFly.Instance.Config.PermissionAdmin };
        public override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("all", "Changes everyone's flight mode,", "all <enable/disable/on/off>", new List<string>(), new List<string>(), 
                (IRocketPlayer caller, string[] args) =>
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
                    UChatHelper.SendCommandReply(Plugin, caller, "success_fly_changed_all");
                })
        };
        public override bool ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            if (args.Length > 2 || args.Length == 0)
                return false;

            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
            if (targetPlayer == null)
            {
                UChatHelper.SendCommandReply(Plugin, caller, "error_player_not_found", args[0].ToString());
                return false;
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
            UChatHelper.SendCommandReply(Plugin, caller, "success_fly_changed_all");
            return true;
        }

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            TFlyComponent comp = player.GetComponent<TFlyComponent>();

            if (DateTime.Now < comp.Cooldown && !player.HasPermission(TFly.Instance.Config.PermissionAdmin))
            {
                UChatHelper.SendCommandReply(TFly.Instance, caller, "error_command_cooldown", Convert.ToInt32((comp.Cooldown - DateTime.Now).TotalSeconds).ToString());
                return;
            }

            TFlyComponent cp = player.GetComponent<TFlyComponent>();

            if (cp.IsFlying)
            {
                comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                comp.SetFlightMode(false);
                comp.Cooldown = DateTime.Now.AddSeconds(TFly.Instance.Config.CooldownInSeconds);
            }
            else
            {
                comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                comp.SetFlightMode(true);
                comp.Cooldown = DateTime.Now.AddSeconds(TFly.Instance.Config.CooldownInSeconds);
            }

        }
    }
}
