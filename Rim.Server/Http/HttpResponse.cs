using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Rim.Server
{
    public class HttpResponse
    {

        #region Properties

        public HttpStatusCode StatusCode { get; set; }

        public string ContentType { get; set; }

        public bool ApplyGzip { get; set; }
        
        internal Stream Stream { get; set; }

        public Dictionary<string, string> AdditionalHeaders { get; set; } = new Dictionary<string, string>();

        private StringBuilder Content { get; set; } = new StringBuilder();

        #endregion

        public void Write(string content)
        {
            Content.Append(content);
        }
        
        public void SetToHtml()
        {
            ContentType = "text/html";
            StatusCode = HttpStatusCode.OK;
        }

        public void SetToJson(object model)
        {
            ContentType = "application/json";
            StatusCode = HttpStatusCode.OK;
            Content.Append(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }
        
        public byte[] GetContent()
        {
            return Encoding.UTF8.GetBytes(Content.ToString());
        }

    }
}
