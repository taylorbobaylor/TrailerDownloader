namespace TrailerDownloader.Models
{
    public class Movie
    {
        public string PosterPath { get; set; } = string.Empty;
        public string TrailerURL { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public bool TrailerExists { get; set; }
    }
}
