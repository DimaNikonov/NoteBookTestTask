using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Core
{
    public class StreamProvider
    {
        public Stream GetZipStream(string textNote)
        {
            var arrayByte = Encoding.UTF8.GetBytes(textNote);
            var outStream = new MemoryStream();
            using (var zipStream = new GZipStream(outStream, CompressionMode.Compress))
            {
                using (var memoryStream = new MemoryStream(arrayByte))
                {
                    memoryStream.CopyTo(zipStream);
                }
            }

            return outStream;
        }

        public Stream GetZipStream(Stream stream)
        {
            var outStream = new MemoryStream();
            using (var zipStream = new GZipStream(outStream, CompressionMode.Compress))
            {
                stream.CopyTo(zipStream);
            }

            return outStream;
        }

        public Stream GetUnZipStream(byte[] array)
        {
            var outStream = new MemoryStream();
            using (MemoryStream compressStream = new MemoryStream(array))
            {
                using (GZipStream zipStream = new GZipStream(compressStream, CompressionMode.Decompress))
                {
                    zipStream.CopyTo(outStream);
                }
            }

            return outStream;
        }
    }
}
