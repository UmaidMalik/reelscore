using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReelScore.Api.DataTransferObjects;
using ReelScore.Api.Models;
using ReelScore.Api.Services;

namespace ReelScore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: api/<MovieController>
        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            return Ok(await _movieService.GetAllMovies());
        }

        // GET api/<MovieController>/5
        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovie(long movieId)
        {
            return Ok(await _movieService.GetMovieById(movieId));
        }

        // POST api/<MovieController>
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(MovieDto movieDto)
        {
            Movie movie = new Movie
            {
                Title = movieDto.Title,
                Summary = movieDto.Summary,
                ReleaseYear = movieDto.ReleaseYear
            };
            return Ok(await _movieService.AddMovie(movie));

        }

        // PUT api/<MovieController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Movie>> PutMovie(long id, [FromBody] MovieDto movieDto)
        {
            Movie movie = new Movie
            {
                MovieId = id,
                Title = movieDto.Title,
                Summary = movieDto.Summary,
                ReleaseYear = movieDto.ReleaseYear
            };
            return Ok(await _movieService.UpdateMovie(id, movie));
        }

        // DELETE api/<MovieController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(long id)
        {
            return Ok(await _movieService.DeleteMovie(id));
        }

        private async Task<bool> MovieExists(long id)
        {
            return await _movieService.MovieExists(id);
        }
    }
}
