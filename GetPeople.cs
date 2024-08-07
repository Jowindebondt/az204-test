using az204_test;
using az204_test.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyCompany
{
    public class GetPeople
    {
        private readonly ILogger<GetPeople> _logger;

        public GetPeople(ILogger<GetPeople> logger)
        {
            _logger = logger;
        }

        [Function("GetPeople")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            var peopleTable = TableClientFactory.GetClient();
            var result = peopleTable.Query<TableEntity>();
            return new OkObjectResult(result.Select(u => new PersonDTO{
                Id = (Guid)u.GetGuid(nameof(Guid)),
                FirstName = u.RowKey,
                LastName = u.PartitionKey
            }));
        }
    }
}
