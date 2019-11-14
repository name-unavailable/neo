using Neo.Wallets;

namespace Neo.Plugins
{
    public interface IServer
    {
        void Start(NeoSystem neoSystem, Wallet wallet);
    }
}
