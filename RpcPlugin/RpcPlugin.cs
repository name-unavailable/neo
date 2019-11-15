using Neo;
using Neo.Plugins;
using Neo.Wallets;

namespace RpcPlugin
{
    public class RpcPlugin : Plugin, IServer
    {
        private Settings rpcSetting;
        private RpcServer rpcServer;

        public override void Configure()
        {
            this.rpcSetting = new Settings(GetConfiguration());
        }

        public void Start(NeoSystem neoSystem, Wallet wallet)
        {
            rpcServer = new RpcServer(neoSystem, wallet, rpcSetting.MaxGasInvoke);
            rpcServer.Start(rpcSetting.BindAddress, rpcSetting.Port, rpcSetting.SslCert, rpcSetting.SslCertPassword, null);
        }
    }
}
