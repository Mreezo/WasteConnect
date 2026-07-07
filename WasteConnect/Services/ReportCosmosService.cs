using Microsoft.Azure.Cosmos;
using WasteConnect.Models;

namespace WasteConnect.Services
{
    public class ReportCosmosService
    {
        private readonly Container _container;

        public ReportCosmosService(IConfiguration configuration)
        {
            string endpoint = configuration["CosmosDb:Endpoint"];
            string key = configuration["CosmosDb:Key"];
            string databaseName = configuration["CosmosDb:DatabaseName"];
            string containerName = configuration["CosmosDb:ContainerName"];

            CosmosClient client = new CosmosClient(endpoint, key);

            _container = client.GetContainer(databaseName, containerName);
        }


        public async Task AddReportAsync(DumpingReport report)
        {
            await _container.CreateItemAsync(
                report,
                new PartitionKey(report.UserId));
        }

        public async Task<List<DumpingReport>> GetReportsByUserIdAsync(string userId)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.UserId = @userId ORDER BY c.CreatedAt DESC")
                .WithParameter("@userId", userId);

            var iterator = _container.GetItemQueryIterator<DumpingReport>(query);

            var reports = new List<DumpingReport>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                reports.AddRange(response);
            }

            return reports;
        }

        public async Task<List<DumpingReport>> GetAllReportsAsync()
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.IsMasterReport = true ORDER BY c.CreatedAt DESC");

            var iterator =
                _container.GetItemQueryIterator<DumpingReport>(query);

            var reports = new List<DumpingReport>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                reports.AddRange(response);
            }

            return reports;
        }

        public async Task<DumpingReport?> GetReportByIdAsync(string id, string userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<DumpingReport>(
                    id,
                    new PartitionKey(userId));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task UpdateReportAsync(DumpingReport report)
        {
            await _container.ReplaceItemAsync(
                report,
                report.Id,
                new PartitionKey(report.UserId));
        }

        public async Task DeleteReportAsync(string id, string userId)
        {
            await _container.DeleteItemAsync<DumpingReport>(
                id,
                new PartitionKey(userId));
        }
        public async Task<List<DumpingReport>> GetReportsByAssignedCompanyAsync(string companyUserId)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.AssignedCompanyUserId = @companyUserId ORDER BY c.AssignedAt DESC")
                .WithParameter("@companyUserId", companyUserId);

            var iterator = _container.GetItemQueryIterator<DumpingReport>(query);

            var reports = new List<DumpingReport>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                reports.AddRange(response);
            }

            return reports;
        }
        
        public async Task<List<DumpingReport>> GetOpenMasterReportsAsync()
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.IsMasterReport = true AND c.Status != 'Cleaned'");

            var iterator = _container.GetItemQueryIterator<DumpingReport>(query);

            var reports = new List<DumpingReport>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                reports.AddRange(response);
            }

            return reports;
        }

        public async Task<List<DumpingReport>> GetLinkedReportsAsync(string masterReportId)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.MasterReportId = @masterReportId")
                .WithParameter("@masterReportId", masterReportId);

            var iterator =
                _container.GetItemQueryIterator<DumpingReport>(query);

            var reports = new List<DumpingReport>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                reports.AddRange(response);
            }

            return reports;
        }

        public async Task UpdateMasterAndLinkedReportsAsync(DumpingReport masterReport)
        {
            await UpdateReportAsync(masterReport);

            var linkedReports =
                await GetLinkedReportsAsync(masterReport.Id);

            foreach (var linkedReport in linkedReports)
            {
                linkedReport.Status = masterReport.Status;

                linkedReport.AssignedCompanyId = masterReport.AssignedCompanyId;
                linkedReport.AssignedCompanyUserId = masterReport.AssignedCompanyUserId;
                linkedReport.AssignedCompanyName = masterReport.AssignedCompanyName;
                linkedReport.AssignedAt = masterReport.AssignedAt;

                linkedReport.CompletedAt = masterReport.CompletedAt;
                linkedReport.AfterCleanupImageUrl = masterReport.AfterCleanupImageUrl;
                linkedReport.CompletionNotes = masterReport.CompletionNotes;

                await UpdateReportAsync(linkedReport);
            }
        }

        public async Task<bool> CompanyHasActiveJobAsync(string companyUserId)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.AssignedCompanyUserId = @companyUserId AND c.Status != 'Cleaned'")
                .WithParameter("@companyUserId", companyUserId);

            var iterator = _container.GetItemQueryIterator<DumpingReport>(query);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();

                if (response.Any())
                    return true;
            }

            return false;
        }
    }
}