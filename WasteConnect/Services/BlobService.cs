using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace WasteConnect.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(IConfiguration configuration)
        {
            string connectionString = configuration["BlobStorage:ConnectionString"];
            string containerName = configuration["BlobStorage:ContainerName"];

            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            return GenerateReadUrl(blobClient);
        }

        public async Task<string> UploadBase64ImageAsync(string base64Image)
        {
            string base64Data = base64Image.Split(',')[1];
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            string fileName = Guid.NewGuid() + ".png";

            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = new MemoryStream(imageBytes);

            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = "image/png"
            });

            return GenerateReadUrl(blobClient);
        }

        private string GenerateReadUrl(BlobClient blobClient)
        {
            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddYears(5)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }

            return blobClient.Uri.ToString();
        }

        //=======================//
        // COMPANY DOCUMENTATION //
        //=======================//

        public async Task<string> UploadPdfAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file selected.");

            if (file.ContentType != "application/pdf")
                throw new Exception("Only PDF documents are allowed.");

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension != ".pdf")
                throw new Exception("Only PDF documents are allowed.");

            string fileName = Guid.NewGuid().ToString() + ".pdf";

            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = "application/pdf"
            });

            return blobClient.Uri.ToString();
        }
    }
}