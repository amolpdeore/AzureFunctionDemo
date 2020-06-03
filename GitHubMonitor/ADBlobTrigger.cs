using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;

namespace GitHubMonitor
{
    public static class ADBlobTrigger
    {
        [FunctionName("ADBlobTrigger")]
        public static void Run([BlobTrigger("adblobcontainer/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=amolstorageaccount;AccountKey=4IY0jTiCkP++zCb2glRLumsgB1JlC8gEu5a+nVnhceMrFezai27xqFwg86LKGObok8yHfLwmzCGQmkuT034RnQ==;EndpointSuffix=core.windows.net");
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("tImage");
            table.CreateIfNotExistsAsync();
        }
    }
}
