using Microsoft.Azure.Cosmos;
using WasteConnect.Models;

namespace WasteConnect.Services
{
    public class CommunityAlertCosmosService
    {
        private readonly Container _container;

        public CommunityAlertCosmosService(
            CosmosClient cosmosClient,
            IConfiguration configuration)
        {
            var databaseName =
                configuration["CosmosDb:DatabaseName"];

            _container = cosmosClient.GetContainer(
                databaseName,
                "CommunityAlerts");
        }


        // ==========================================
        // CREATE ALERT
        // ==========================================

        public async Task<CommunityAlert> CreateAlertAsync(
            CommunityAlert alert)
        {
            alert.Id = Guid.NewGuid().ToString();
            alert.CreatedAt = DateTime.UtcNow;
            alert.Status = "Active";

            var response =
                await _container.CreateItemAsync(
                    alert,
                    new PartitionKey(alert.AlertType));

            return response.Resource;
        }


        // ==========================================
        // GET ALL ALERTS
        // ==========================================

        public async Task<List<CommunityAlert>> GetAllAlertsAsync()
        {
            var query = new QueryDefinition(
                "SELECT * FROM c ORDER BY c.CreatedAt DESC");

            var iterator =
                _container.GetItemQueryIterator<CommunityAlert>(
                    query);

            var alerts = new List<CommunityAlert>();

            while (iterator.HasMoreResults)
            {
                var response =
                    await iterator.ReadNextAsync();

                alerts.AddRange(response);
            }

            return alerts;
        }


        // ==========================================
        // GET ACTIVE ALERTS
        // ==========================================

        public async Task<List<CommunityAlert>>
            GetActiveAlertsAsync()
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.Status = @status")
                .WithParameter("@status", "Active");

            var iterator =
                _container.GetItemQueryIterator<CommunityAlert>(
                    query);

            var alerts = new List<CommunityAlert>();

            while (iterator.HasMoreResults)
            {
                var response =
                    await iterator.ReadNextAsync();

                alerts.AddRange(response);
            }

            return alerts
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }
    }
}