using System;
using System.Collections.Generic;

namespace Rim.Server.Http
{
    public class RequestReader
    {
        public HttpRequest Read(string data)
        {
            HttpRequest request = new HttpRequest();
            request.Content = "";
            request.Headers = new Dictionary<string, string>();

            string[] lines = data.Split('\n');
            bool head = true;

            //read first line
            string[] headline = lines[0].Split(' ');
            
            request.Method = headline[0];
            request.Path = headline[1];
            
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (head && string.IsNullOrEmpty(line))
                {
                    head = false;
                    continue;
                }

                if (head)
                {
                    int index = line.IndexOf(':');
                    if (index < 0)
                        continue;

                    string key = line.Substring(0, index);
                    string value = line.Substring(index + 1);
                    AddHeader(request, key, value);
                }
                else
                    request.Content += line + Environment.NewLine;
            }
            
            return request;
        }

        private void AddHeader(HttpRequest request, string key, string value)
        {
            string lcase = key.Trim().ToLower();
            string trimmed_value = value.Trim();

            switch(lcase)
            {
                case "host":
                    request.Host = trimmed_value;
                    break;

                case "sec-websocket-key":
                    request.WebSocketKey = trimmed_value;
                    request.IsWebSocket = true;
                    break;

                case "accept-encoding":
                    request.AcceptEncoding = trimmed_value;
                    break;

                default:
                    request.Headers.Add(key, trimmed_value);
                    break;
            }
        }

    }
}
