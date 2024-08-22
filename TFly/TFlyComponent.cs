using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TFly
{
    public class TFlyComponent : UnturnedPlayerComponent
    {
        public bool IsFlying { get; private set; }
        public float FlySpeed { get; private set; }
        public DateTime Cooldown = DateTime.Now;

        public void SetIsFlying(bool isFlying)
        {
            IsFlying = isFlying;
        }

        public void SetFlySpeed(float speed)
        {
            FlySpeed = speed;
        }

        public void SetFlightMode(bool enable)
        {
            float flyspeed = TFly.Instance.Config.DefaultFlySpeed;
            if (FlySpeed != 0)
            {
                flyspeed = FlySpeed;
            }

            if (IsFlying && !enable)
            {
                IsFlying = false;
                Player.Player.movement.sendPluginGravityMultiplier(1f);
                Player.Player.movement.sendPluginSpeedMultiplier(1f);

                if (TFly.Instance.Config.GodModeWhenFlyEnabled)
                    Player.GodMode = false;

                if (TFly.Instance.Config.FlyAnimationEnabled)
                {
                    UpdateStance(EPlayerStance.STAND);
                }
                UChatHelper.ServerSendChatMessage("success_fly_stop", toPlayer: Player.SteamPlayer());
            }
            else if (!IsFlying && enable)
            {
                IsFlying = true;
                Player.Player.movement.sendPluginGravityMultiplier(TFly.Instance.Config.Gravity);
                Player.Player.movement.sendPluginSpeedMultiplier(flyspeed);

                if (TFly.Instance.Config.GodModeWhenFlyEnabled)
                    Player.GodMode = true;

                if (TFly.Instance.Config.FlyAnimationEnabled)
                {
                    UpdateStance(EPlayerStance.SWIM);
                }
                UChatHelper.ServerSendChatMessage("success_fly_start", toPlayer: Player.SteamPlayer());
            }
        }

        public void UpdateStance(EPlayerStance newStance)
        {
            /*Player.Player.stance.channel.send("tellStance", ESteamCall.OWNER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[]
            {
               (byte)newStance
            });*/
            //Player.Player.stance.stance = newStance;
            Player.Player.stance.ReceiveStance(newStance);
        }
    }
}
