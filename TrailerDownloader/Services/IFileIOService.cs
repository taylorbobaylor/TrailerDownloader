using System;

namespace TrailerDownloader.Services
{
    public interface IFileIOService
    {
        string ReadAllText(string path);
    }
}
