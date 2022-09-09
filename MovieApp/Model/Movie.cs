using System.ComponentModel.DataAnnotations;

namespace MovieApp.Model
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Moviename { get; set; }
        [Required]
        public string Director { get; set; }
        [Required]
        public string MusicDirector { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please enter rating from 1 to 5")]
        public int Rating { get; set; }

        public virtual List<Cast> CastList { get; set; }
    }
}
