using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TFly.Utils;

namespace Tavstal.TFly.Handlers
{
    public static class UnturnedPlayerHandler
    {
        public static void Attach()
        {
            PlayerInput.onPluginKeyTick += OnKeyDown;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
        }

        public static void Detach()
        {
            PlayerInput.onPluginKeyTick -= OnKeyDown;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
        }
        
        private static void OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (TFly._flyingPlayers.Contains(player))
            {
                FlyComponent? comp = ComponentManager.Get(player);
                comp?.SetFlightMode(false);
            }
            ComponentManager.Invalidate(player.Id);
        }
        
        private static void OnKeyDown(Player player, uint simulation, byte key, bool state)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
            FlyComponent? comp = ComponentManager.Get(uPlayer);
            if (comp == null)
                return;

            if (!comp.IsFlying)
                return;

            if (!state)
                return;
            
            
            switch (key)
            {
                case 0:
                {
                    comp.SetFlySpeed(comp.FlySpeed + 1);
                    uPlayer.Player.movement.sendPluginGravityMultiplier(TFly.Instance.Config.Gravity);
                    uPlayer.Player.movement.sendPluginSpeedMultiplier(comp.FlySpeed);

                    if (TFly.Instance.Config.GodModeWhenFlyEnabled)
                        uPlayer.GodMode = true;

                    if (TFly.Instance.Config.FlyAnimationEnabled)
                        comp.UpdateStance(EPlayerStance.SWIM);
                    return;
                }
                case 1:
                {
                    if (comp.FlySpeed - 1 < 1)
                        return;

                    if (comp.FlySpeed <= 0)
                        comp.SetFlySpeed(TFly.Instance.Config.DefaultFlySpeed);
                    else
                        comp.SetFlySpeed(comp.FlySpeed - 1);

                    player.movement.sendPluginGravityMultiplier(TFly.Instance.Config.Gravity);
                    player.movement.sendPluginSpeedMultiplier(comp.FlySpeed);

                    if (TFly.Instance.Config.GodModeWhenFlyEnabled)
                        uPlayer.GodMode = true;

                    if (TFly.Instance.Config.FlyAnimationEnabled)
                        comp.UpdateStance(EPlayerStance.SWIM);

                    break;
                }
            }
        }
    }
}