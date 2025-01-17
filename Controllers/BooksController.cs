﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMSDemoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CMSDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BooksController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BooksController(BookDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(x => x.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

			return book;
		}

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostBook([FromBody]Book book)
        {
            if (book == null)
            {
                return BadRequest("Invalid book data.");
            }

            if (!ModelState.IsValid) // Check for model state errors
            {
                return BadRequest(ModelState);
            }
            try
            {
                _context.Books.Add(book);
                _context.Entry(book).State = EntityState.Added;
                await _context.SaveChangesAsync();
                return NoContent(); // Return the created book
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error while saving book data");
                return StatusCode(500, "Internal server error");
            }
           
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
