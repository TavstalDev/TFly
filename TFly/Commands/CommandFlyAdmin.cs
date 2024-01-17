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

namespace Tavstal.TFly
{
    public class CommandFlyAdmin : CommandBase
    {
        public override IPlugin Plugin => TFly.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "flyadmin";
        public override string Help => "Moderates flight mode";
        public override string Syntax => "[player] <enable/disable> | all <enable/disable/on/off>";
        public override List<string> Aliases => new List<string>() { "flya", "flighta", "flightadmin" };
        public override List<string> Permissions => new List<string> { TFly.Instance.Config.PermissionAdmin };
        public override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("all", "Changes everyone's flight mode,", "all <enable/disable/on/off>", new List<string>(), new List<string>(), 
                (IRocketPlayer player, string[] args) =>
                {

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
            comp.SetFlightMode(!comp.IsFlying);
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
                UnturnedChat.Say(caller, TFly.Instance.Localize("error_command_cooldown", Convert.ToInt32((comp.Cooldown - DateTime.Now).TotalSeconds).ToString()));
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

            if (args.Length == 1)
            {
                if (args[0].ToLower() == "all" && player.HasPermission(TFly.Instance.Config.PermissionAdmin))
                {
                    foreach (SteamPlayer sp in Provider.clients)
                    {
                        UnturnedPlayer target = UnturnedPlayer.FromSteamPlayer(sp);
                        TFlyComponent cp = target.GetComponent<TFlyComponent>();
                        if (cp.isFlying)
                        {
                            cp.FlySpeed = config.DefaultSpeedInFly;
                            main.FlyMode(target, false);
                        }
                        else if (!cp.isFlying)
                        {
                            cp.FlySpeed = config.DefaultSpeedInFly;
                            main.FlyMode(target, true);
                        }
                    }
                    UnturnedChat.Say(caller, main.Translate("Fly_changed_all"));
                }
                else if (args[0].ToLower() != "all" && player.HasPermission(config.PermissionAdmin))
                {
                    UnturnedPlayer target = UnturnedPlayer.FromName(args[0]);
                    if (target == null)
                    {
                        UnturnedChat.Say(caller, main.Translate("Player_Not_Found", args[0].ToString()));
                        return;
                    }
                    else
                    {
                        TFlyComponent cp = target.GetComponent<TFlyComponent>();
                        if (cp.isFlying)
                        {
                            cp.FlySpeed = config.DefaultSpeedInFly;
                            main.FlyMode(target, false);
                            main.Cooldowns.Add(player.CSteamID, DateTime.Now.AddSeconds(config.CooldownInSeconds));
                        }
                        else if (!cp.isFlying)
                        {
                            cp.FlySpeed = config.DefaultSpeedInFly;
                            main.FlyMode(target, true);
                            main.Cooldowns.Add(player.CSteamID, DateTime.Now.AddSeconds(config.CooldownInSeconds));
                        }
                    }
                }
            }
        }
    }
}
