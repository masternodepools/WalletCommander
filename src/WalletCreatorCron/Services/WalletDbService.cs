using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletCreatorCron.Models;

namespace WalletCreatorCron.Services
{
    public class WalletDbService
    {
        private Table _walletsTable;

        public WalletDbService(AwsSettings settings)
        {
            var region = RegionEndpoint.GetBySystemName(settings.Region);
            var client = new AmazonDynamoDBClient(
                settings.AccessId,
                settings.AccessSecret,
                region);

            _walletsTable = Table.LoadTable(client, "wallets");
        }

        public async Task<IList<UserWallet>> GetUncreatedWallets(string coin)
        {
            var filter = new ScanFilter();
            filter.AddCondition("coin", ScanOperator.Equal, coin);
            filter.AddCondition("address", ScanOperator.Equal, "UNCREATED");

            var wallets = await _walletsTable
                .Scan(filter)
                .GetRemainingAsync();

            var returnValue = new List<UserWallet>();
            foreach (var wallet in wallets)
            {
                returnValue.Add(JsonConvert.DeserializeObject<UserWallet>(wallet.ToJson()));
            }

            return returnValue;
        }

        public async Task UpdateWallet(UserWallet wallet)
        {
            var doc = Document.FromJson(JsonConvert.SerializeObject(wallet));
            await _walletsTable.UpdateItemAsync(doc);
        }
    }
}
