using System.ComponentModel.DataAnnotations.Schema;

namespace CMSDemoAPI.Models
{
    public class BookAuthor
    {
        public int BookId {  get; set; }
        [ForeignKey("BookId")]
        public Book? book { get; set; }
        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public Author? author { get; set; }
    }
}
