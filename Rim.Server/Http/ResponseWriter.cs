using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Rim.Server.Http
{
    internal class ResponseWriter
    {
        internal void Write(HttpResponse response)
        {
            Stream stream = response.Stream;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] result;
                byte[] content = response.GetContent();
                if (content != null && content.Length > 0)
                {
                    if (response.ApplyGzip)
                    {
                        using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
                            gzip.Write(content, 0, content.Length);

                        result = ms.ToArray();
                    }
                    else
                        result = content;
                }
                else
                    result = new byte[0];

                StringBuilder responseHeader = new StringBuilder();
                responseHeader.Append("HTTP/1.1 " + Convert.ToInt32(response.StatusCode) + " " + response.StatusCode + Environment.NewLine);
                responseHeader.Append("Server: rim" + Environment.NewLine);
                responseHeader.Append("Content-Type: " + response.ContentType + "; charset=utf-8" + Environment.NewLine);

                if (response.ApplyGzip)
                    responseHeader.Append("Content-Encoding: gzip" + Environment.NewLine);

                responseHeader.Append("Content-Length: " + result.Length + Environment.NewLine);

                foreach (var header in response.AdditionalHeaders)
                    responseHeader.AppendLine(header.Key + ": " + header.Value + Environment.NewLine);

                responseHeader.Append(Environment.NewLine);
                
                byte[] headerBytes = Encoding.UTF8.GetBytes(responseHeader.ToString());
                
                stream.Write(headerBytes, 0, headerBytes.Length);
                stream.Write(result, 0, result.Length);
            }

        }

    }
}
