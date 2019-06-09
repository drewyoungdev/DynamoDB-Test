using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
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

        [HttpGet]
        public async Task<ActionResult<string>> Get(int year, string title)
        {
            Table table = Table.LoadTable(dynamoDb, TableName);
            var document = await table.GetItemAsync(year, title);

            return document.ToJsonPretty();
        }

        [HttpPut]
        public async Task<ActionResult<string>> Put(int year, string title)
        {
            Table table = Table.LoadTable(dynamoDb, TableName);

            var movie = new Document();
            movie["year"] = year;
            movie["title"] = title;

            var movieInfo = new Document();
            movieInfo["rating"] = 1;

            // if you had a more nested structure,
            // how would you only update a single item in the nested object?
            movie["info"] = movieInfo;

            // Optional parameters.
            UpdateItemOperationConfig config = new UpdateItemOperationConfig
            {
                // Get updated item in response.
                ReturnValues = ReturnValues.AllNewAttributes
            };

            var document = await table.UpdateItemAsync(movie, config);

            return document.ToJsonPretty();
        }
    }
}
