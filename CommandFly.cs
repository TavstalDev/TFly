using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Rocket.API;
using Rocket.Unturned.Chat;

namespace TPlugins.Fly
{
    public class CommandFly : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "Fly";
        public string Help => "Enables/Disables flying mode";
        public string Syntax => "nothing || <player>/all";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { TFly.Instance.Configuration.Instance.Permission };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            var main = TFly.Instance;
            var config = main.Configuration.Instance;

            if (args.Length == 1)
            {
                if (args[0].ToLower() == "all" && player.IsAdmin)
                {
                    foreach (SteamPlayer sp in Provider.clients)
                    {
                        UnturnedPlayer target = UnturnedPlayer.FromSteamPlayer(sp);
                        TFlyComponent cp = target.GetComponent<TFlyComponent>();
                        if (cp.isFlying)
                        {
                            main.FlyMode(target, false);
                        }
                        else if (!cp.isFlying)
                        {
                            main.FlyMode(target, true);
                        }
                    }
                    UnturnedChat.Say(caller, main.Translate("Fly_changed_all"));
                }
                else if (args[0].ToLower() != "all" && player.IsAdmin)
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
                            main.FlyMode(target, false);
                        }
                        else if (!cp.isFlying)
                        {
                            main.FlyMode(target, true);
                        }
                    }
                }
            }
            else if (args.Length == 0)
            {
                TFlyComponent cp = player.GetComponent<TFlyComponent>();

                if (cp.isFlying)
                {
                    main.FlyMode(player, false);
                }
                else if (!cp.isFlying)
                {
                    main.FlyMode(player, true);
                }
            }
            else
            {
                UnturnedChat.Say(caller, main.Translate("Fly_Usage" + Syntax));
                return;
            }
        }
    }
}
