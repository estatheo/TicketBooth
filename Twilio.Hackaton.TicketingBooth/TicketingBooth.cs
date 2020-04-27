using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Twilio.Rest.Api.V2010.Account;

namespace Twilio.Hackaton.TicketingBooth
{
    public static class TicketingBooth
    {
        [FunctionName("NewTicket")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("A new Ticket request has been submitted");

            var formData = await req.ReadFormAsync();

            var email = new Email
            {
                Dkim = formData["dkim"],
                To = formData["to"],
                Html = formData["html"],
                From = formData["from"],
                Text = formData["text"],
                SenderIp = formData["sender_ip"],
                Envelope = formData["envelope"],
                Attachments = int.Parse(formData["attachments"]),
                Subject = formData["subject"], 
                Charsets = formData["charsets"],
                Spf = formData["spf"],
            };

            String strorageconn = System.Environment.GetEnvironmentVariable("storageAccount");
            CloudStorageAccount storageacc = CloudStorageAccount.Parse(strorageconn);

            CloudBlobClient blobClient = storageacc.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(System.Environment.GetEnvironmentVariable("containerName"));

            await container.CreateIfNotExistsAsync();

            var uris = new List<Uri>();
            foreach (var file in req.Form.Files)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);
                
                using (var filestream = file.OpenReadStream())
                {


                    await blockBlob.UploadFromStreamAsync(filestream);

                }

                uris.Add(blockBlob.Uri);
                
            }

            string accountSid = System.Environment.GetEnvironmentVariable("accountSid");
            string authToken = System.Environment.GetEnvironmentVariable("accountSecret");

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                from: new Twilio.Types.PhoneNumber($"whatsapp:{System.Environment.GetEnvironmentVariable("fromNumber")}"),
                body: email.Text,
                to: new Twilio.Types.PhoneNumber($"whatsapp:{email.Subject}"),
                mediaUrl: uris
            );


            return new OkObjectResult($"Ticket delivered!");
        }
    }
}
