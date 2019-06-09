using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace dynamodb_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private const string TableName = "Movies";
        private readonly DynamoDBContext dynamoDbContext;

        public MoviesController(IAmazonDynamoDB dynamoDb)
        {
            this.dynamoDbContext = new DynamoDBContext(dynamoDb);
        }

        [HttpGet]
        public async Task<ActionResult<Movie>> Get(int year, string title)
        {
            var movie = new Movie()
            {
                Year = year,
                Title = title
            };

            var response = await dynamoDbContext.LoadAsync<Movie>(movie);

            if (response == null)
            {
                return NotFound($"No movie with year: {year} and title: {title}");
            }

            return response;
        }
    }

    [DynamoDBTable("Movies", LowerCamelCaseProperties = true)]
    public class Movie
    {
        [DynamoDBHashKey]
        public int Year
        {
            get; set;
        }

        [DynamoDBRangeKey]
        public string Title
        {
            get; set;
        }
    }
}
