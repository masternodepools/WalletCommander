using Microsoft.Extensions.Configuration;
using QTWalletClient.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using WalletCommander.Models;
using WalletCommander.Services;

namespace WalletCommander
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = GetConfiguration();

            var awsSettings = config
                .GetSection("AwsSettings")
                .Get<AwsSettings>();

            var walletSettings = config
                .GetSection("WalletSettings")
                .Get<WalletSettings>();

            var walletCreator = new WalletCreator(walletSettings, awsSettings);

            Task.Run(walletCreator.Run);

            Console.ReadKey();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
