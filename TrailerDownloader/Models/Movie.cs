namespace TrailerDownloader.Models
{
    public class Movie
    {
        public string PosterPath { get; set; }
        public string TrailerURL { get; set; }
        public int? Id { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public bool TrailerExists { get; set; }
    }
}
