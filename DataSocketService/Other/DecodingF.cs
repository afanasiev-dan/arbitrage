using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSocketService.Other
{
    public class DecodingF
    {
        public static byte[] Normalize(byte[] data)
        {
            if (IsGZipCompressed(data))
            {
                return DecompressGZip(data);
            }
            else if (IsDeflateCompressed(data))
            {
                return DecompressDeflate(data);
            }
            else
            {
                return data; // уже обычный текст в UTF-8
            }
        }

        private static bool IsGZipCompressed(byte[] data)
        {
            return data.Length >= 2 && data[0] == 0x1F && data[1] == 0x8B;
        }

        private static bool IsDeflateCompressed(byte[] data)
        {
            return data.Length >= 2 && data[0] == 0x78; // deflate header
        }

        private static byte[] DecompressGZip(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzipStream.CopyTo(output);
            return output.ToArray();
        }

        private static byte[] DecompressDeflate(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress);
            using var output = new MemoryStream();
            deflateStream.CopyTo(output);
            return output.ToArray();
        }
    }
}
