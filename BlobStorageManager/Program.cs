using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace BlobStorageManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while(true)
            {
                string operation = "1", fileName = "", blobName = "", msg = "";

                Console.WriteLine("Select operation from below list\n1. Upload\n2. Download\n3. Delete");
                operation = Console.ReadLine();

                Console.WriteLine("\n\nEnter file name with extenstion: ");
                fileName = Console.ReadLine();

                Console.WriteLine("\n\nEnter blob name(small case only): ");
                blobName = Console.ReadLine();

                if (operation == "1")
                {
                    msg = await FileHandler.UploadFile(fileName, blobName);
                }
                else if (operation == "2")
                {
                    msg = FileHandler.DownloadFile(fileName, blobName);
                }
                else
                {
                    msg = await FileHandler.DeleteFile(fileName, blobName);
                }

                Console.WriteLine(msg);
                Console.WriteLine("\n\nPress ctrl + C to exit\n\n");
            }
        }
    }

    public static class FileHandler
    {
        public async static Task<string> UploadFile(string fileName, string containerName)
        {
            try
            {
                var blobStorageConnectionString = GetStorageConnectionString();

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

                //create a container if it is not already exists
                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                string filePath = @"C:\DevNDocs\_Upload\" + fileName;
                Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(Path.GetFileName(filePath));
                cloudBlockBlob.Properties.ContentType = GetMimeType(Path.GetFileName(filePath));
                await cloudBlockBlob.UploadFromStreamAsync(stream);

                return "File uploaded successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string DownloadFile(string fileName, string containerName)
        {
            try
            {
                var blobStorageConnectionString = GetStorageConnectionString();
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

                blockBlob.DownloadToFile(@"C:\DevNDocs\_Download\" + fileName, FileMode.Create);
                
                return "File downloaded successfully";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async static Task<string> DeleteFile(string fileName, string containerName)
        {
            try
            {
                var blobStorageConnectionString = GetStorageConnectionString();

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(fileName);
                await blob.DeleteIfExistsAsync();

                return "File removed from blob " + containerName;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        public static string GetStorageConnectionString()
        {
            return "DefaultEndpointsProtocol=https;AccountName=amolstorageaccount;AccountKey=4IY0jTiCkP++zCb2glRLumsgB1JlC8gEu5a+nVnhceMrFezai27xqFwg86LKGObok8yHfLwmzCGQmkuT034RnQ==;EndpointSuffix=core.windows.net";
        }
    }
}
