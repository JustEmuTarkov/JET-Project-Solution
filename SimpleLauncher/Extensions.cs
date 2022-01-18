using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SimpleLauncher
{
    public static class Extensions
    {
        public static byte[] Compress(this string text)
        {
            if (text == null)
                return null;

            using (Stream memOutput = new MemoryStream())
            {
                using (GZipOutputStream zipOut = new GZipOutputStream(memOutput))
                {
                    using (StreamWriter writer = new StreamWriter(zipOut))
                    {
                        writer.Write(text);

                        writer.Flush();
                        zipOut.Finish();

                        byte[] bytes = new byte[memOutput.Length];
                        memOutput.Seek(0, SeekOrigin.Begin);
                        memOutput.Read(bytes, 0, bytes.Length);

                        return bytes;
                    }
                }
            }
        }

        private static string Decompress(byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (Stream memInput = new MemoryStream(bytes))
            using (GZipInputStream zipInput = new GZipInputStream(memInput))
            using (StreamReader reader = new StreamReader(zipInput))
            {
                return reader.ReadToEnd();
            }
        }
        public static string Decompress(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (stream == null)
                    return "";
                stream.CopyTo(ms);
                return Decompress(ms.ToArray());
            }
        }
        public static byte[] ToBytes(this string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static T JsonCastTo<T>(this string json) 
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e) 
            {
                return default(T);
            }
        }
        public static string ToString<T>(this T data) 
        {
            return JsonSerializer.Serialize(data);
        }
    }
}
