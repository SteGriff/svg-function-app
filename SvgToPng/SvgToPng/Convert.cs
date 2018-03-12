using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Svg;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SvgToPng
{
    public static class Convert
    {
        [FunctionName("Convert")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "/Convert")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            dynamic data = await req.Content.ReadAsAsync<object>();
            var requestStream = await req.Content.ReadAsStreamAsync();
            log.Info(data);

            var memStream = new MemoryStream();
            var doc = SvgDocument.Open<SvgDocument>(requestStream);
            doc.Draw().Save(memStream, ImageFormat.Png);

            memStream.Seek(0, SeekOrigin.Begin);
            byte[] imageBytes = new byte[2 ^ 32];
            int streamLength = System.Convert.ToInt32(memStream.Length);
            memStream.Read(imageBytes, 0, streamLength);

            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(imageBytes);

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            { FileName = "image.png" };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            response.Headers.Add("content-disposition", "attachment; filename=\"image.png\"");
            return response;
        }
    }
}
