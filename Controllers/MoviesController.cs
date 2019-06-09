using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace dynamodb_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private const string TableName = "Movies";
        private readonly IAmazonDynamoDB dynamoDb;

        public MoviesController(IAmazonDynamoDB dynamoDb)
        {
            this.dynamoDb = dynamoDb;
        }

        [HttpGet("{year}")]
        public async Task<ActionResult<string>> Get(int year)
        {
            // number of keys must match that created in the table
            var key = new Dictionary<string, AttributeValue>
            {
                { "year", new AttributeValue { N = year.ToString() } },
                { "title", new AttributeValue { S = "Turn It Down, Or Else!" } }
            };

            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = key
            };

            var response = await dynamoDb.GetItemAsync(request);

            if (!response.IsItemSet)
                return NotFound();

            return response.Item["info"].M["actors"].L[0].S;
        }
    }
}
