using Pinnacle.IServices;
using RestSharp;

namespace Pinnacle.Services
{
    public class WhatsappService : IWhatsappService
    {
        public async Task SendMessageAsync(string PhoneNumber, string Message)
        {
            var url = "https://api.ultramsg.com/instance130895/messages/chat";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "xmo1132rjx2c2dth");
            request.AddParameter("to", PhoneNumber);
            request.AddParameter("body", Message);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            Console.WriteLine(output);
        }
        public async Task SendImageAsync(string PhoneNumber, string Caption, string ImagePath)
        {
            var url = "https://api.ultramsg.com/instance130895/messages/image";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "xmo1132rjx2c2dth");
            request.AddParameter("to", PhoneNumber);
            request.AddParameter("image", ImagePath);
            request.AddParameter("caption", Caption);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            Console.WriteLine(output);
        }
        public async Task SendDocumentAsync(string PhoneNumber, string FileName, string Caption, MemoryStream stream)
        {
            var url = "https://api.ultramsg.com/instance130895/messages/document";
            var client = new RestClient(url);
            string base64String = Convert.ToBase64String(stream.ToArray());
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "xmo1132rjx2c2dth");
            request.AddParameter("to", PhoneNumber);
            request.AddParameter("filename", FileName);
            request.AddParameter("document", base64String);
            request.AddParameter("caption", Caption);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            Console.WriteLine(output);
        }
    }
}
