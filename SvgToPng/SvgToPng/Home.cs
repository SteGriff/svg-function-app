using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SvgToPng
{
    public static class Home
    {
        [FunctionName("Home")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Home")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Index");
            var response = req.CreateResponse(HttpStatusCode.OK);

            var stream = new FileStream(@"d:\home\site\wwwroot\index.htm", FileMode.Open);
            response.Content = new StreamContent(stream);

            //response.Content = new StringContent("Hello world");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
            //return req.CreateResponse(HttpStatusCode.OK, "Hello world");
        }
    }
}
