using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Db;
using MovieApp.DTos;
using MovieApp.Model;

namespace MovieApp.Controllers
{
    [Authorize]
    [Route("api/[Controller]/[Action]")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// create movie
        /// </summary>
        /// <param name="moviedto"></param>
        /// <returns></returns>
        [HttpPost]
        
        public ActionResult Create([FromBody] MovieDTO moviedto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var movie = new Movie
            {
                Moviename = moviedto.Moviename,
                Director = moviedto.Director,
                MusicDirector = moviedto.MusicDirector,
                Rating = moviedto.Rating,
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            foreach (var item in moviedto.MovieCastName)
            {
                var MovieCast = new Cast
                {
                    MovieId = movie.Id,
                    cast = item
                };

                _context.Casts.Add(MovieCast);
            }

            _context.SaveChanges();


            return Ok();

        }



        [HttpGet]
        public ActionResult Getmovie()
        {
            var movie = _context.Movies.Include(u => u.CastList).Select(u =>
                new
                {
                    Moviename = u.Moviename,
                    Director = u.Director,
                    MusicDirector = u.MusicDirector,
                    Rating = u.Rating,
                    Cast = u.CastList.Select(x => x.cast)
                });
            return Json(movie);
        }


        [HttpGet]
        //filtercastbymovie
        public ActionResult getmoviebycast(string Cast)
        {
            var cast = _context.Casts.Where(x => x.cast == Cast).ToList();

            List<int> datavalue = new List<int>();
            foreach (var item in cast)
            {
                datavalue.Add(item.MovieId);
            }
            var data = _context.Movies.Include(u => u.CastList).Where(u => datavalue.Contains(u.Id)).Select(u =>
                new
                {
                    Moviename = u.Moviename,
                    Director = u.Director,
                    MusicDirector = u.MusicDirector,
                    Rating = u.Rating,
                    Cast = u.CastList.Select(x => x.cast)
                });
            return Json(data);


        }
        [HttpGet]
        //Filtermoviebycast
        public ActionResult Getmoviebydirector(string dirName)
        {
            var movie = _context.Movies.Where(x => x.Director.ToLower() == dirName).Select(u =>
                new
                {
                    Moviename = u.Moviename,
                    Director = u.Director,
                    MusicDirector = u.MusicDirector,
                    Rating = u.Rating,
                    Cast = u.CastList.Select(x => x.cast)
                });
            return Json(movie);

        }

        [HttpPut]
        public ActionResult updatemovie(int id, [FromBody] Movie movie)
        {
            var Movie = _context.Movies.FirstOrDefault(x => x.Id == id);
            Movie.Moviename = movie.Moviename;
            Movie.MusicDirector = movie.MusicDirector;
            Movie.Director = movie.Director;
            Movie.Rating = movie.Rating;
            _context.SaveChanges();
            return Json(movie);
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var movie = _context.Movies.SingleOrDefault(u => u.Id == id);
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok();

        }


    }
}
