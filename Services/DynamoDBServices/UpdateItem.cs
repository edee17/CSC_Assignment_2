using Amazon.DynamoDBv2.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLifeTimeTalents.Models;

namespace TheLifeTimeTalents.Services.DynamoDBServices
{
    public interface IUpdateItem
    {

    }
    public class UpdateItem : IUpdateItem
    {
        private readonly IGetItem _getItem;

        public UpdateItem(IGetItem getItem)
        {
            _getItem = getItem;
        }

        //public async Task<Item> Update(int id, double price)
        //{
        //    var response = await _getItem.GetItems(id);

        //    var currentPrice = response.Items.Select(p => p.Price);

        //    var replyDateTime = response.Items.Select(p => p.ReplyDatetime);


        //}

        //private UpdateItemRequest RequestBuilder(int id, double price, string replyDateTime, double currentPrice)
        //{
        //    var request = new UpdateItemRequest {
        //        Key = new Dictionary<string, AttributeValue>
        //        {
        //            {"Id", new AttributeValue
        //            {
        //                N = id.ToString()
        //            } },
        //            {"ReplyDateTime", new AttributeValue
        //            {
        //                N = replyDateTime
        //            } },
        //        },
        //        ExpressionAttributeNames = new Dictionary<string, string>
        //        {
        //            {"#P","Price" }
        //        },
        //        ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        //        {
        //            {":newprice", new AttributeValue {

        //                N = price.ToString()
        //            } },
        //            {":currprice", new AttributeValue {

        //                N = currentPrice.ToString()
        //            } }
        //        },

        //        UpdateExpression = "SET #P = :newprice"
        //    };
        //}
    }
}
