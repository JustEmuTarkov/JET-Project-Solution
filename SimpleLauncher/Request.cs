using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;

namespace SimpleLauncher
{
    
    class Request
    {
        string BackendUrl;
        string Session;
        internal Request(string backendUrl = "https://127.0.0.1/", string Session = "SimpleLauncher") {
            this.BackendUrl = backendUrl;
            this.Session = Session;
        }
        private void DisableSSL() 
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
        public Uri GenerateBackendUri(string Backend, string Path)
        {
            if (!Backend.EndsWith("/")) 
            {
                Backend += "/";
            }
            return new Uri(Backend + Path);
        }
        private Stream Send(string data = "", string backendPath = "", string method = "POST", bool compress = false) {
            if (backendPath == "")
                return null;
            // disable SSL encryption
            DisableSSL();

            Uri fullUri = GenerateBackendUri(BackendUrl, backendPath);
            WebRequest request = WebRequest.Create(fullUri);

            if (!string.IsNullOrEmpty(Session))
            {
                request.Headers.Add("Cookie", $"PHPSESSID={Session}");
                request.Headers.Add("SessionId", Session);
            }

            request.Headers.Add("Accept-Encoding", "deflate");

            request.Method = method;

            if (method != "GET" && !string.IsNullOrEmpty(data))
            {
                // set request body
                byte[] bytes = (compress) ? data.Compress() : data.ToBytes();

                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;

                if (compress)
                {
                    request.Headers.Add("Content-Encoding", "deflate");
                }

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            // get response stream
            try
            {
                return request.GetResponse().GetResponseStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + e.StackTrace);
            }

            return null;
        }

        internal string SendJson(string data = "", string backendPath = "") 
        {
            return Send(data, backendPath, "POST", true).Decompress();
        }

    }
}
/*
public string Session;
public string RemoteEndPoint;
public bool isUnity;
public Request(string session, string remoteEndPoint, bool isUnity = true)
{
    Session = session;
    RemoteEndPoint = remoteEndPoint;
}

private Stream Send(string url, string method = "GET", string data = null, bool compress = true)
{
    // disable SSL encryption
    ServicePointManager.Expect100Continue = true;
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

    var fullUri = url;
    if (!Uri.IsWellFormedUriString(fullUri, UriKind.Absolute))
        fullUri = RemoteEndPoint + fullUri;
    WebRequest request = WebRequest.Create(new Uri(fullUri));

    if (!string.IsNullOrEmpty(Session))
    {
        request.Headers.Add("Cookie", $"PHPSESSID={Session}");
        request.Headers.Add("SessionId", Session);
    }

    request.Headers.Add("Accept-Encoding", "deflate");

    request.Method = method;

    if (method != "GET" && !string.IsNullOrEmpty(data))
    {
        // set request body
        byte[] bytes = (compress) ? SimpleZlib.CompressToBytes(data, zlibConst.Z_BEST_COMPRESSION) : Encoding.UTF8.GetBytes(data);

        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;

        if (compress)
        {
            request.Headers.Add("Content-Encoding", "deflate");
        }

        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    // get response stream
    try
    {
        WebResponse response = request.GetResponse();
        return response.GetResponseStream();
    }
    catch (Exception e)
    {
        if (isUnity)
            Debug.LogError(e);
    }

    return null;
}

public void PutJson(string url, string data, bool compress = true)
{
    using (Stream stream = Send(url, "PUT", data, compress)) { }
}

public string GetJson(string url, bool compress = true)
{
    using (Stream stream = Send(url, "GET", null, compress))
    {
        using (MemoryStream ms = new MemoryStream())
        {
            if (stream == null)
                return "";
            stream.CopyTo(ms);
            return SimpleZlib.Decompress(ms.ToArray(), null);
        }
    }
}

public string PostJson(string url, string data, bool compress = true)
{
    using (Stream stream = Send(url, "POST", data, compress))
    {
        using (MemoryStream ms = new MemoryStream())
        {
            if (stream == null)
                return "";
            stream.CopyTo(ms);
            return SimpleZlib.Decompress(ms.ToArray(), null);
        }
    }
}

public Texture2D GetImage(string url, bool compress = true)
{
    using (Stream stream = Send(url, "GET", null, compress))
    {
        using (MemoryStream ms = new MemoryStream())
        {
            if (stream == null)
                return null;
            Texture2D texture = new Texture2D(8, 8);

            stream.CopyTo(ms);
#if B11661
                    texture.LoadRawTextureData(ms.ToArray());
#else
            texture.LoadImage(ms.ToArray());
#endif
            return texture;
        }
    }
}*/
