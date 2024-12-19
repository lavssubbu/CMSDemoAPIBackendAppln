using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMSDemoAPI.Models;

namespace CMSDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookAuthorsController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BookAuthorsController(BookDbContext context)
        {
            _context = context;
        }

        // GET: api/BookAuthors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookAuthor>>> GetbookAuthors()
        {
            return await _context.bookAuthors
                .Include(b=>b.book)
                .Include(a=>a.author)
                .ToListAsync();
        }

        // GET: api/BookAuthors/5
        [HttpGet("{bookId}/{authorId}")]
        public async Task<ActionResult<BookAuthor>> GetBookAuthor(int bookId,int authorId)
        {
            var bookAuthor = await _context.bookAuthors.Include(ba=>ba.book)
                .Include(a=>a.author)
                .FirstOrDefaultAsync(b=>b.BookId==bookId && b.AuthorId==authorId);

            if (bookAuthor == null)
            {
                return NotFound();
            }

            return bookAuthor;
        }
        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookAuthor>> GetBookAuthor(int bookId)
        {
            var bookAuthor = await _context.bookAuthors.Include(b=>b.book)
                .Include(a=>a.author)
                .Where(ba => ba.BookId == bookId)
                .ToListAsync();

            if (!bookAuthor.Any())
            {
                return NotFound();
            }

            return Ok(bookAuthor);
        }

        [HttpGet("GetBooks/{AuthorId}")]
        public async Task<IEnumerable<Book>> GetBooksbyAuthor(int AuthorId)
        {
            var books = await _context.Books
                .Where(b=>b.bookAuthors.Any(a=>a.AuthorId==AuthorId))     
                .Include(ba=>ba.bookAuthors)
                .ThenInclude(a=>a.author)
                .ToListAsync();
            
           return books;
        }

        [HttpGet("GetAuthors/{bukid}")]
        public async Task<IEnumerable<Author>> GetAuthorssbyBook(int bukid)
        {
            var authors = await _context.authors
                .Where(b => b.bookAuthors.Any(a => a.BookId == bukid))
                .ToListAsync();

            return authors;
        }

        // PUT: api/BookAuthors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookAuthor(int id, BookAuthor bookAuthor)
        {
            if (id != bookAuthor.BookId)
            {
                return BadRequest();
            }

            _context.Entry(bookAuthor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookAuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BookAuthors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookAuthor>> PostBookAuthor(BookAuthor bookAuthor)
        {
            _context.bookAuthors.Add(bookAuthor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookAuthorExists(bookAuthor.BookId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookAuthor", new { id = bookAuthor.BookId }, bookAuthor);
        }

        // DELETE: api/BookAuthors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookAuthor(int id)
        {
            var bookAuthor = await _context.bookAuthors.FindAsync(id);
            if (bookAuthor == null)
            {
                return NotFound();
            }

            _context.bookAuthors.Remove(bookAuthor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookAuthorExists(int id)
        {
            return _context.bookAuthors.Any(e => e.BookId == id);
        }
    }
}
