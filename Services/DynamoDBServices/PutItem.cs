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
        Task AddNewEntry(int id, string replyDateTime);
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

        public async Task AddNewEntry(int id, string replyDateTime, double price)
        {
            var queryRequest = RequestBuilder(id, replyDateTime, price);

            await PutItemAsync(queryRequest);
        }

        private PutItemRequest RequestBuilder(int id, string replyDateTime, double price)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue {N = id.ToString()}},
                {"ReplyDateTime", new AttributeValue {N = replyDateTime}},
                {"Price", new AttributeValue {N = price.ToString()}}
            };

            return new PutItemRequest
            {
                TableName = "TempDynamoDbTable",
                Item = item
            };
        }
    }
}
