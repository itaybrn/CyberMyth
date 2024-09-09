using ExitGames.Client.Photon;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace PowerUpCommands
{
    public enum PowerUp
    {
        TimeRewind,
        TimeStop,
        Swap,
        Superweapon,
        Clone
    };
    public class PowerUpCommand
    {

        public PowerUp type;
        public float parameter;
        public int playerIndex;

        public PowerUpCommand(PowerUp p, float parameter, int playerID)
        {
            this.type = p;
            this.parameter = parameter;
            this.playerIndex = playerID;
        }

        public PowerUpCommand(PowerUp p, int playerID) : this(p, 0, playerID) {}
    }

    public static class CommandSerializer
    {
        public static void Register()
        {
            PhotonPeer.RegisterType(typeof(PowerUpCommand), (byte)'P', SerializePowerUpCommand, DeserializePowerUpCommand);
        }

        private static byte[] SerializePowerUpCommand(object customType)
        {
            PowerUpCommand command = (PowerUpCommand)customType;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, command);

                return memoryStream.ToArray();
            }
        }

        private static object DeserializePowerUpCommand(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                return formatter.Deserialize(memoryStream);
            }
        }
    }
}

