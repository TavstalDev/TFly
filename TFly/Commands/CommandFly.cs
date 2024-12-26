using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TFly.Commands
{
    public class CommandFly : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "fly";
        public string Help => "Toggles flight mode";
        public string Syntax => "";
        public List<string> Aliases => new List<string>() { "flight" };
        public List<string> Permissions => new List<string> { "tfly.commands.fly", "tfly.commands.fly.admin" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            FlyComponent comp = player.GetComponent<FlyComponent>();

            if (DateTime.Now < comp.Cooldown && !player.HasPermission("tfly.commands.fly.admin"))
            {
                TFly.Instance.SendCommandReply(caller, "error_command_cooldown", Convert.ToInt32((comp.Cooldown - DateTime.Now).TotalSeconds).ToString());
                return;
            }

            FlyComponent cp = player.GetComponent<FlyComponent>();

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
