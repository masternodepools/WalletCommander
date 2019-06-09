using QTWalletClient;
using QTWalletClient.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletCreatorCron.Models;

namespace WalletCreatorCron.Services
{
    public class WalletCreator
    {
        private WalletDbService _walletDbService;
        private WalletClient _walletClient;
        private string _coinName;

        public WalletCreator(WalletSettings walletSettings,
            AwsSettings awsSettings)
        {
            _walletDbService = new WalletDbService(awsSettings);
            _walletClient = new WalletClient(walletSettings);
            _coinName = walletSettings.CoinName;
        }

        public async void Run()
        {
            try
            {
                var uncreatedWallets = await _walletDbService.GetUncreatedWallets(_coinName);
                foreach (var uncreatedWallet in uncreatedWallets)
                {
                    var wallet = await GetOrCreateWallet(_coinName, uncreatedWallet.UserId);
                    await _walletDbService.UpdateWallet(wallet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(1000);
            }

            Thread.Sleep(3000);

            Run();
        }

        private async Task<UserWallet> GetOrCreateWallet(string coinName, string userId)
        {
            Console.WriteLine($"Creating {coinName} wallet for user: {userId}");

            var address = await _walletClient.SendCommandAsync<string>(WalletCommands.GetAccountAddress, userId);

            var now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            var wallet = new UserWallet
            {
                UserId = userId,
                Coin = coinName,
                Address = address.Result,
                CreatedAt = now
            };

            Console.WriteLine($"Created successfully: {wallet.Address}");

            return wallet;
        }
    }
}
