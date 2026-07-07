using Microsoft.Azure.Cosmos;
using WasteConnect.Models;

namespace WasteConnect.Services
{
    public class CompanyCosmosService
    {
        private readonly Container _container;

        public CompanyCosmosService(IConfiguration configuration)
        {
            string endpoint = configuration["CompanyCosmosDb:Endpoint"];
            string key = configuration["CompanyCosmosDb:Key"];
            string databaseName = configuration["CompanyCosmosDb:DatabaseName"];
            string containerName = configuration["CompanyCosmosDb:ContainerName"];

            var client = new CosmosClient(endpoint, key);

            _container = client.GetContainer(databaseName, containerName);
        }

        public async Task AddCompanyAsync(Company company)
        {
            await _container.CreateItemAsync(
                company,
                new PartitionKey(company.UserId));
        }

        public async Task<Company?> GetCompanyByUserIdAsync(string userId)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.UserId = @userId")
                .WithParameter("@userId", userId);

            var iterator = _container.GetItemQueryIterator<Company>(query);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();

                return response.FirstOrDefault();
            }

            return null;
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            var query = new QueryDefinition(
                "SELECT * FROM c ORDER BY c.CreatedAt DESC");

            var iterator = _container.GetItemQueryIterator<Company>(query);

            var companies = new List<Company>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                companies.AddRange(response);
            }

            return companies;
        }
        public async Task<Company?> GetCompanyByIdAsync(string id, string userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<Company>(
                    id,
                    new PartitionKey(userId));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            await _container.ReplaceItemAsync(
                company,
                company.Id,
                new PartitionKey(company.UserId));
        }


        public async Task<List<Company>> GetApprovedCompaniesAsync()
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.Status = 'Approved'");

            var iterator =
                _container.GetItemQueryIterator<Company>(query);

            var companies = new List<Company>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                companies.AddRange(response);
            }

            return companies;
        }
    }
}