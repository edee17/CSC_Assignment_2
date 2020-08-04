using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLifeTimeTalents.Models;

namespace TheLifeTimeTalents.Services.DynamoDBServices
{
    public interface IGetItem
    {
        Task<DynamoTableItems> GetItems(int? id);

    }
    public class GetItem : IGetItem
    {
        private readonly IAmazonDynamoDB _dynamoClient;

        public GetItem(IAmazonDynamoDB dynamoClient)
        {
            _dynamoClient = dynamoClient;
        }

        public async Task<DynamoTableItems> GetItems(int? id)
        {
            var queryRequest = RequestBuilder(id);

            var result = await ScanAsync(queryRequest);

            return new DynamoTableItems
            {
                Items = result.Items.Select(Map).ToList()
            };
        }

        private ScanRequest RequestBuilder(int? id)
        {
            if (id.HasValue == false)
            {
                return new ScanRequest
                {
                    TableName = "Subscription",
                };
            }

            return new ScanRequest
            {
                TableName = "Subscription",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
            {
                ":v_Id", new AttributeValue { N = id.ToString() }}
        },
                FilterExpression = "SubId = :v_Id",
                ProjectionExpression = "PriceId, SubBy, SubId"
            };
        }

        private async Task<ScanResponse> ScanAsync(ScanRequest request)
        {
            var response = await _dynamoClient.ScanAsync(request);

            return response;
        }

        private Item Map(Dictionary<string, AttributeValue> result)
        {
            return new Item
            {
                PriceId = result["PriceId"].N,
                SubBy = int.Parse(result["SubBy"].N),
                SubId = result["SubId"].N
            };
        }
    }
}
