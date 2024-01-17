using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TFly
{
    public class CommandFly : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "fly";
        public string Help => "Toggles flight mode";
        public string Syntax => "";
        public List<string> Aliases => new List<string>() { "flight" };
        public List<string> Permissions => new List<string> { TFly.Instance.Config.Permission, TFly.Instance.Config.PermissionAdmin };

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
