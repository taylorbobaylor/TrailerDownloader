using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrailerDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TrailerDownloader.SignalRHubs
{
    public class MovieHub : Hub
    {
        public async Task<bool> DownloadAllTrailers(IEnumerable<Movie> movieList)
        {
            //Parallel.ForEach(movieList, async movie =>
            //{
            //    if (movie.TrailerExists == false)
            //    {
            //        await DownloadTrailerAsync(movie);
            //        movie.TrailerExists = true;
            //        await Clients.All.SendAsync("downloadAllTrailers", movieList);
            //    }
            //});

            foreach (Movie movie in movieList)
            {
                if (movie.TrailerExists == false)
                {
                    await DownloadTrailerAsync(movie);
                    movie.TrailerExists = true;
                    await Clients.All.SendAsync("downloadAllTrailers", movieList);
                }
            }

            return true;
        }

        public async Task<bool> DeleteAllTrailers(IEnumerable<Movie> movieList)
        {
            foreach (Movie movie in movieList)
            {
                if (movie.TrailerExists)
                {
                    string filePath = Directory.GetFiles(movie.FilePath).Where(name => name.Contains("-Trailer")).FirstOrDefault();
                    File.Delete(filePath);
                    movie.TrailerExists = false;
                    await Clients.All.SendAsync("deleteAllTrailers", movieList);
                }
            }

            return true;
        }

        private async Task<bool> DownloadTrailerAsync(Movie movie)
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(movie.TrailerURL);

            // Get highest quality muxed stream
            var streamInfo = streamManifest.GetMuxed().WithHighestVideoQuality();

            if (streamInfo != null)
            {
                // Get the actual stream
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                // Download the stream to file
                await youtube.Videos.Streams.DownloadAsync(streamInfo, $@"{movie.FilePath}\{movie.Title}-Trailer.{streamInfo.Container}");

                return true;
            }

            return false;
        }
    }
}
