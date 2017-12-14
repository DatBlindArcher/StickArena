using Steamworks;

namespace ArcherNetwork
{
    public partial class Packet
    {
        public void Write(CSteamID id)
        {
            Write((ulong)id);
        }

        public CSteamID ReadSteamID()
        {
            return (CSteamID)ReadULong();
        }
    }
}
