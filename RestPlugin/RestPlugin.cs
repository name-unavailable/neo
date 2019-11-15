using Neo;
using Neo.Plugins;
using Neo.Wallets;

namespace RestPlugin
{
    public class RestPlugin : Plugin, IServer
    {
        private Settings restSetting;
        private RestServer restServer;

        public override void Configure()
        {
            this.restSetting = new Settings(GetConfiguration());
        }

        public void Start(NeoSystem neoSystem, Wallet wallet)
        {
            restServer = new RestServer(neoSystem, wallet, restSetting.MaxGasInvoke);
            restServer.Start(restSetting.BindAddress, restSetting.Port, restSetting.SslCert, restSetting.SslCertPassword, null);
        }
    }
}
