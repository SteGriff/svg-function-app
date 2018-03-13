using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Svg;
using System;
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
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Convert")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // TODO: Error handling

            // Get request body (SVG text)
            Stream requestStream;
            try
            {
                requestStream = await req.Content.ReadAsStreamAsync();
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Couldn't understand the input stream. Please send a raw POST body.");
            }

            // Render the SVG into a PNG MemoryStream
            var memStream = new MemoryStream();
            try
            {
                var doc = SvgDocument.Open<SvgDocument>(requestStream);
                doc.Draw().Save(memStream, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Failed to render SVG - please check that your input is a well-formed SVG: " + ex.Message);
            }

            // Fiddle with the MemoryStream to get a Byte[]
            byte[] imageBytes;
            try
            {
                imageBytes = new byte[memStream.Length];
                memStream.Seek(0, SeekOrigin.Begin);
                int streamLength = System.Convert.ToInt32(memStream.Length);
                memStream.Read(imageBytes, 0, streamLength - 1);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to format output PNG (maybe the file was too big): " + ex.Message);
            }

            // Prepare the response
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(imageBytes);

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            { FileName = "image.png" };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return response;
        }
    }
}
