﻿using System.ComponentModel.DataAnnotations;

namespace CMSDemoAPI.Models
{
    public class Author
    {
        [Key]
        public int authId { get; set; }
        public string? authName {  get; set; }

        //Navigation Property
        public ICollection<BookAuthor> bookAuthors { get; set; }

    }
}
