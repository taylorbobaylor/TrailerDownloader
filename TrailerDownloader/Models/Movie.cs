using System.ComponentModel.DataAnnotations;

namespace TrailerDownloader.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        public string PosterPath { get; set; }
        public string TrailerURL { get; set; }
        public int TMDBId { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public bool TrailerExists { get; set; }
    }
}
