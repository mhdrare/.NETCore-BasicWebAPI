using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BasicWebAPI.Models.Param;
using BasicWebAPI.Models.View;
using System.Linq;
using System;
using BasicWebAPI.Models;
using BasicWebAPI.Models.Entity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BasicWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class BooksController : ControllerBase
    {
        private readonly BasicDbContext dbContext;

        public BooksController(BasicDbContext dbContext)
        {
            this.dbContext = dbContext;
            if (this.dbContext.Books.Count() == 0)
            {
                this.dbContext.Books.Add(new Book
                {
                    Id = 1,
                    Date = DateTime.UtcNow,
                    Title = "Hello There!"
                });
                this.dbContext.Books.Add(new Book
                {
                    Id = 2,
                    Date = DateTime.UtcNow,
                    Title = "Hello You!"
                });
                this.dbContext.Books.Add(new Book
                {
                    Id = 3,
                    Date = DateTime.UtcNow,
                    Title = "11:11"
                });
                this.dbContext.Books.Add(new Book
                {
                    Id = 4,
                    Date = DateTime.UtcNow,
                    Title = "Konspirasi Alam Semesta"
                });
                this.dbContext.SaveChanges();
            }
        }

        public BookView[] BookList {get; set;}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookView>>> Get([FromQuery]GetBookParam param)
        {
            var dataSource = await dbContext.Books
                .Select(book => new BookView {
                    Title = book.Title,
                    Date = book.Date
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

        [HttpPost]
        public async Task<ActionResult<IEnumerable<BookView>>> Post([FromBody] BookView param)
        {
            var newId = await dbContext.Books.CountAsync() + 1;
            dbContext.Add(new Book {
                Id = newId,
                Title = param.Title,
                Date = param.Date
            });
            await this.dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<BookView>>> Put([FromRoute]int id, [FromBody] BookView param)
        {
            var selectedBook = await dbContext.Books.SingleOrDefaultAsync(item => item.Id == id);
            if (selectedBook == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            selectedBook.Title = param.Title;
            selectedBook.Date = param.Date;
            await this.dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookView>> Get([FromRoute]int id)
        {
            var selectedBook = await dbContext.Books
                .Where(book => book.Id == id)
                .Select(book => new BookView {
                    Title = book.Title,
                    Date = book.Date
                })
                .SingleOrDefaultAsync();
            if (selectedBook == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return selectedBook;
        }
    }
}