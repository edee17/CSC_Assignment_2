using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheLifeTimeTalents.Services.DynamoDBServices
{
    public interface IPutItem
    {
        Task AddNewEntry(string priceId, int subBy, string subId);
    }
    public class PutItem
    {
        private readonly IAmazonDynamoDB _dynamoClient;

        public PutItem(IAmazonDynamoDB dynamoClient)
        {
            _dynamoClient = dynamoClient;
        }

        private async Task PutItemAsync(PutItemRequest request)
        {
            await _dynamoClient.PutItemAsync(request);
        }

        public async Task AddNewEntry(string priceId, int subBy, string subId)
        {
            var queryRequest = RequestBuilder( priceId, subBy, subId);

            await PutItemAsync(queryRequest);
        }

        private PutItemRequest RequestBuilder( string priceId, int subBy, string subId)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                {"PriceId", new AttributeValue {N = priceId}},
                {"SubBy", new AttributeValue {N = subBy.ToString()}},
                {"SubId", new AttributeValue {N = subId}}
            };

            return new PutItemRequest
            {
                TableName = "SubscriptionTable",
                Item = item
            };
        }
    }
}
