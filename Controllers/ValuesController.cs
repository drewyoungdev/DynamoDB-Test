using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace dynamodb_test.Controllers
{
    // Do `docker run dynamodb-local` (after downloading latest dynamodb/local dockerhub image)
    // Create table with GET http://localhost:5000/api/values/init
    // Add item to table with POST http://localhost:5000/api/values/ with body '{ "Id": 1, "Title": "test" }
    // Retrieve item from table with GET http://localhost:5000/api/values/1 and it should return "test"

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private const string TableName = "SampleData";

        private readonly IAmazonDynamoDB dynamoDb;

        public ValuesController(IAmazonDynamoDB dynamoDb)
        {
            this.dynamoDb = dynamoDb;
        }

        [HttpGet("init")]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var request = new ListTablesRequest
            {
                Limit = 10
            };

            var response = await dynamoDb.ListTablesAsync(request);

            var results = response.TableNames;

            if (!results.Contains(TableName))
            {
                var createRequest = new CreateTableRequest
                {
                    TableName = TableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "N"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"  //Partition key
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 2,
                        WriteCapacityUnits = 2
                    }
                };

                await dynamoDb.CreateTableAsync(createRequest);
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { N = id.ToString() } } }
            };

            var response = await dynamoDb.GetItemAsync(request);

            if (!response.IsItemSet)
                return NotFound();

            return response.Item["Title"].S;
        }

        [HttpPost]
        public async Task Post([FromBody] PostInput input)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = input.Id.ToString() }},
                    { "Title", new AttributeValue { S = input.Title }}
                }
            };

            await dynamoDb.PutItemAsync(request);
        }
    }

    public class PostInput
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
