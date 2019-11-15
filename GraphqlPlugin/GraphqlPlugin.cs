using Neo;
using Neo.Plugins;
using Neo.Wallets;

namespace GraphqlPlugin
{
    public class GraphqlPlugin : Plugin, IServer
    {
        private Settings rpcSetting;
        private GraphQLServer graphQLServer;

        public override void Configure()
        {
            this.rpcSetting = new Settings(GetConfiguration());
        }

        public void Start(NeoSystem neoSystem, Wallet wallet)
        {
            graphQLServer = new GraphQLServer(neoSystem, wallet, rpcSetting.MaxGasInvoke);
            graphQLServer.Start(rpcSetting.BindAddress, rpcSetting.Port, rpcSetting.SslCert, rpcSetting.SslCertPassword, null);
        }
    }
}
