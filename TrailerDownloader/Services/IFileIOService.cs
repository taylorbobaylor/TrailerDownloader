using System;

namespace TrailerDownloader.Services
{
    public interface IFileIOService
    {
        bool Exists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
    }
}
