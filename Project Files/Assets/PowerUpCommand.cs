using UnityEngine;

namespace PowerUpCommands
{
    public enum PowerUp
    {
        TimeRewind,
        TimeStop
    };
    public class PowerUpCommand
    {

        public PowerUp type;
        public float parameter;

        public PowerUpCommand(PowerUp p, float parameter)
        {
            this.type = p;
            this.parameter = parameter;
        }

        public PowerUpCommand(PowerUp p) : this(p, 0)
        {
        }
    }
}

