using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TFly
{
    /// <summary>
    /// Represents a component that controls the flying behavior of the player.
    /// This class allows enabling/disabling flying, adjusting fly speed, and managing flight-related cooldowns.
    /// </summary>
    public class FlyComponent : UnturnedPlayerComponent
    {
        /// <summary>
        /// Gets a value indicating whether the player is currently flying.
        /// </summary>
        public bool IsFlying { get; private set; }
        /// <summary>
        /// Gets a value indicating whether the player is currently flying.
        /// </summary>
        public float FlySpeed { get; private set; } = TFly.Instance.Config.DefaultFlySpeed;
        /// <summary>
        /// Gets or sets the cooldown time for the flight mode.
        /// </summary>
        public DateTime Cooldown { get; private set; } = DateTime.Now;

        /// <summary>
        /// Sets the flying state of the player.
        /// </summary>
        /// <param name="isFlying">Indicates whether the player is flying.</param>
        public void SetIsFlying(bool isFlying)
        {
            IsFlying = isFlying;
        }

        /// <summary>
        /// Sets the fly speed for the player.
        /// </summary>
        /// <param name="speed">The new fly speed to set.</param>
        public void SetFlySpeed(float speed)
        {
            FlySpeed = speed;
        }

        /// <summary>
        /// Sets the cooldown time for the flight mode.
        /// </summary>
        /// <param name="cooldown">The new cooldown time.</param>
        public void SetCooldown(DateTime cooldown)
        {
            Cooldown = cooldown;
        }

        
        /// <summary>
        /// Updates the cooldown time by adding the configured cooldown duration.
        /// </summary>
        public void UpdateCooldown()
        {
            Cooldown = DateTime.Now.AddSeconds(TFly.Instance.Config.CooldownInSeconds);
        }

        /// <summary>
        /// Enables or disables flight mode for the player.
        /// </summary>
        /// <param name="enable">Indicates whether to enable or disable flight mode.</param>
        public void SetFlightMode(bool enable)
        {
            float flySpeed = TFly.Instance.Config.DefaultFlySpeed;
            if (FlySpeed != 0)
                flySpeed = FlySpeed;
            
            if (enable)
            {
                if (IsFlying)
                    return;
                
                IsFlying = true;
                Player.Player.movement.sendPluginGravityMultiplier(TFly.Instance.Config.Gravity);
                Player.Player.movement.sendPluginSpeedMultiplier(flySpeed);
                TFly._flyingPlayers.Add(Player);

                if (TFly.Instance.Config.GodModeWhenFlyEnabled)
                    Player.GodMode = true;

                if (TFly.Instance.Config.FlyAnimationEnabled)
                    UpdateStance(EPlayerStance.SWIM);
                TFly.Instance.SendChatMessage(Player.SteamPlayer(), "commands_fly_start", TFly.Instance.Config.General.MessageIcon);
                return;
            }

            if (!IsFlying)
                return;
                
            IsFlying = false;
            Player.Player.movement.sendPluginGravityMultiplier(1f);
            Player.Player.movement.sendPluginSpeedMultiplier(1f);
            TFly._flyingPlayers.Remove(Player);
            
            if (TFly.Instance.Config.GodModeWhenFlyEnabled)
                Player.GodMode = false;

            if (TFly.Instance.Config.FlyAnimationEnabled)
                UpdateStance(EPlayerStance.STAND);
            TFly.Instance.SendChatMessage(Player.SteamPlayer(), "commands_fly_stop", TFly.Instance.Config.General.MessageIcon);
        }

        /// <summary>
        /// Updates the player's stance to the specified stance.
        /// </summary>
        /// <param name="newStance">The new stance to set for the player.</param>
        public void UpdateStance(EPlayerStance newStance)
        {
            Player.Player.stance.stance = newStance;
            Player.Player.stance.ReceiveStance(newStance);
        }
    }
}
