using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ap07GalaxyNet.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "profile-pictures"; // Το όνομα του φακέλου που φτιάξαμε στο Azure

        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            // 1. Βρίσκουμε τον φάκελο (container) στο Azure
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // 2. Δημιουργούμε ένα μοναδικό όνομα για την εικόνα (π.χ. 3f8a..._toffee.jpg) για να μην σβηστεί κάποια παλιά με το ίδιο όνομα
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            // 3. Ανεβάζουμε την εικόνα
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            // 4. Επιστρέφουμε το URL (το link) της εικόνας!
            return blobClient.Uri.ToString();
        }
    }
}