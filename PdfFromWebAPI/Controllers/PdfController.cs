using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace PdfFromWebAPI.Controllers
{
    public class PdfController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetPdf()
        {
            var path = @"C:\Users\ariel\Downloads\ApplicationSignature.pdf";

            if (!File.Exists(path))
            {
                return BadRequest("File not found");
            }

            //converting Pdf file into bytes array  
            var dataBytes = File.ReadAllBytes(path);

            //adding bytes to memory stream   
            var dataStream = new MemoryStream(dataBytes);

            return new eBookResult(dataStream, Request, "SamplePdfName.pdf");

        }

        public class eBookResult : IHttpActionResult
        {
            MemoryStream bookStuff;
            string PdfFileName;
            HttpRequestMessage httpRequestMessage;
            HttpResponseMessage httpResponseMessage;

            public eBookResult(MemoryStream data, HttpRequestMessage request, string filename)
            {
                bookStuff = data;
                httpRequestMessage = request;
                PdfFileName = filename;
            }

            public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(
                System.Threading.CancellationToken cancellationToken)
            {
                httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(bookStuff);
                //httpResponseMessage.Content = new ByteArrayContent(bookStuff.ToArray());  
                httpResponseMessage.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = PdfFileName;
                httpResponseMessage.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                return System.Threading.Tasks.Task.FromResult(httpResponseMessage);
            }

            public HttpResponseMessage GetPdf(int docid)
            {
                //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
                //var documents = reader.GetDocument(docid);
                //if (documents != null && documents.Length == 1)
                //{
                //    var document = documents[0];
                //    docid = document.docid;
                //    byte[] buffer = new byte[0];
                //    //generate pdf document
                //    MemoryStream memoryStream = new MemoryStream();
                //    MyPDFGenerator.New().PrintToStream(document, memoryStream);
                //    //get buffer
                //    buffer = memoryStream.ToArray();
                //    //content length for use in header
                //    var contentLength = buffer.Length;
                //    //200
                //    //successful
                //    var statuscode = HttpStatusCode.OK;
                //    response = Request.CreateResponse(statuscode);
                //    response.Content = new StreamContent(new MemoryStream(buffer));
                //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                //    response.Content.Headers.ContentLength = contentLength;
                //    ContentDispositionHeaderValue contentDisposition = null;
                //    if (ContentDispositionHeaderValue.TryParse("inline; filename=" + document.Name + ".pdf", out contentDisposition))
                //    {
                //        response.Content.Headers.ContentDisposition = contentDisposition;
                //    }
                //}
                //else
                //{
                //    var statuscode = HttpStatusCode.NotFound;
                //    var message = String.Format("Unable to find resource. Resource \"{0}\" may not exist.", docid);
                //    var responseData = responseDataFactory.CreateWithOnlyMetadata(statuscode, message);
                //    response = Request.CreateResponse((HttpStatusCode)responseData.meta.code, responseData);
                //}
                //return response;

                return null;
            }
        }
    }
}
