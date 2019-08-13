using BasicWebAPI.Models;
using BasicWebAPI.Models.Entity;
using BasicWebAPI.Models.Param;
using BasicWebAPI.Models.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class MoviesController : ControllerBase
    {
        private readonly BasicDbContext dbContext;

        public MoviesController(BasicDbContext dbContext) 
        {
            this.dbContext = dbContext;
            if (this.dbContext.Movies.Count() == 0)
            {
                this.dbContext.Movies.Add(new Movie
                {
                    Title = "Harry Potter",
                    DurationInHour = 2
                });
                this.dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieView>>> Get([FromQuery]GetMovieParam param)
        {
            var dataSource = await dbContext.Movies
                .Select(movie => new MovieView {
                    Title = movie.Title,
                    DurationInHour = movie.DurationInHour
                })
                .ToListAsync();
            if (string.IsNullOrWhiteSpace(param.Title))
            {
                return dataSource;
            }
            return dataSource.Where(item => item.Title.ToLower()
                .Contains(param.Title.ToLower()))
                .ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieView>> Get([FromRoute]int id)
        {
            var selectedMovie = await dbContext.Movies
                .Where(movie => movie.Id == id)
                .Select(movie => new MovieView {
                    Title = movie.Title,
                    DurationInHour = movie.DurationInHour
                })
                .SingleOrDefaultAsync();
            if (selectedMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return selectedMovie;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<MovieView>>> Post([FromBody]MovieView param)
        {
            var newMovie = new Movie {
                Title = param.Title,
                DurationInHour = param.DurationInHour
            };
            dbContext.Add(newMovie);
            await this.dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new{id = newMovie.Id}, param);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<MovieView>>> Put([FromRoute]int id, [FromBody] MovieView param)
        {
            var selectedMovie = await dbContext.Movies.SingleOrDefaultAsync(item => item.Id == id);
            if (selectedMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            selectedMovie.Title = param.Title;
            selectedMovie.DurationInHour = param.DurationInHour;
            await this.dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<MovieView>>> Delete([FromRoute]int id)
        {
            var selectedMovie = await dbContext.Movies.SingleOrDefaultAsync(item => item.Id == id);
            if (selectedMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            dbContext.Remove(selectedMovie);
            await this.dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
        
    }
}