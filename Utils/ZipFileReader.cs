using System.IO.Compression;

namespace atx2img.Utils;

public class ZipFileReader : IDisposable
{
    private readonly ZipArchive _zipArchive;

    public ZipFileReader(string zipFilePath)
    {
        if (!File.Exists(zipFilePath))
        {
            throw new FileNotFoundException($"Zip file not found at '{zipFilePath}'");
        }
        _zipArchive = ZipFile.OpenRead(zipFilePath);
    }

    public Stream? GetEntryStream(string entryName)
    {
        var entry = _zipArchive.GetEntry(entryName);
        return entry?.Open();
    }

    public void Dispose()
    {
        _zipArchive.Dispose();
    }
}