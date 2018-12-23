using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Rim.Server
{
    public class HttpResponse
    {

        #region Properties

        /// <summary>
        /// Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Content type such as (text/plain, application/json) can include charset information with ";" seperator
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// If true, it's response of the request which has gzip accept encoding.
        /// However, if you want to response without gzip, this property can be set to false manually, and vice/versa.
        /// </summary>
        public bool ApplyGzip { get; set; }
        
        /// <summary>
        /// Network stream of the Requester (if connection is using SSL, this stream is SslStream. otherwise NetworkStream)
        /// </summary>
        internal Stream Stream { get; set; }

        /// <summary>
        /// Additional headers for the response.
        /// </summary>
        public Dictionary<string, string> AdditionalHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Response content. The response byte array is created just be sending the data to the client.
        /// Until this operation, data wil be appended to the content string builder.
        /// </summary>
        private StringBuilder Content { get; set; } = new StringBuilder();

        #endregion

        /// <summary>
        /// Writes a string to the response
        /// </summary>
        public void Write(string content)
        {
            Content.Append(content);
        }
        
        /// <summary>
        /// Sets response content type to html and status to 200
        /// </summary>
        public void SetToHtml()
        {
            ContentType = "text/html";
            StatusCode = HttpStatusCode.OK;
        }

        /// <summary>
        /// Sets response content type to json and status to 200
        /// </summary>
        public void SetToJson(object model)
        {
            ContentType = "application/json";
            StatusCode = HttpStatusCode.OK;
            Content.Append(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }
        
        /// <summary>
        /// Reads the content and creates byte array for shipping
        /// </summary>
        /// <returns></returns>
        public byte[] GetContent()
        {
            return Encoding.UTF8.GetBytes(Content.ToString());
        }

    }
}
