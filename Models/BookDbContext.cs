using Microsoft.EntityFrameworkCore;

namespace CMSDemoAPI.Models
{
    public class BookDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Author> authors { get; set; }
        public DbSet<BookAuthor> bookAuthors { get; set; }
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba=>new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.book)
                .WithMany(a => a.bookAuthors)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<BookAuthor>()
               .HasOne(ba => ba.author)
               .WithMany(a => a.bookAuthors)
               .HasForeignKey(ba => ba.AuthorId);
        }
    }
}
