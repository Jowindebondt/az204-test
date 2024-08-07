using System.Text.Json;
using az204_test;
using az204_test.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyCompany
{
    public class AddPerson
    {
        private readonly ILogger<AddPerson> _logger;

        public AddPerson(ILogger<AddPerson> logger)
        {
            _logger = logger;
        }

        [Function("AddPerson")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("AddPerson Function starts processing request");

            var content = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogError("AddPerson Function could not process request");
                return new BadRequestObjectResult("Could not process request");
            }

            var addPersonDTO = JsonSerializer.Deserialize<AddPersonDTO>(content);
            if (addPersonDTO == null || string.IsNullOrEmpty(addPersonDTO.FirstName) || string.IsNullOrEmpty(addPersonDTO.LastName))
            {
                _logger.LogError("AddPerson Function could not process request");
                return new BadRequestObjectResult("Could not process request");
            }

            _logger.LogInformation("AddPerson Function is processing request");

            var peopleTable = TableClientFactory.GetClient();

            var hasEntity = await peopleTable.GetEntityIfExistsAsync<TableEntity>(addPersonDTO.LastName, addPersonDTO.FirstName);
            if (hasEntity.HasValue)
            {
                _logger.LogInformation("AddPerson Function has processed request");
                return new OkObjectResult(hasEntity.Value.GetGuid(nameof(Guid)));
            }

            var newPerson = new TableEntity(addPersonDTO.LastName, addPersonDTO.FirstName)
            {
                { nameof(Guid), Guid.NewGuid() },
            };

            var _ = peopleTable.AddEntity(newPerson);

            _logger.LogInformation("AddPerson Function has processed request");
            return new OkObjectResult(newPerson.GetGuid(nameof(Guid)));
        }
    }
}
