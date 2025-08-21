using System.IO.Compression;

namespace DataSocketService.Utils;

public class DecodingF
{
    public static byte[] Normalize(byte[] data)
    {
        if (IsGZipCompressed(data)) return DecompressGZip(data);
        if (IsDeflateCompressed(data)) return DecompressDeflate(data);
        return data;
    }

    private static bool IsGZipCompressed(byte[] data)
        => data.Length >= 2 && data[0] == 0x1F && data[1] == 0x8B;

    private static bool IsDeflateCompressed(byte[] data)
        => data.Length >= 2 && data[0] == 0x78;

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
