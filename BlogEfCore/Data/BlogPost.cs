using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogEfCore.Data
{
    public class BlogPost
    {
        public Author Author { get; set; }

        public int AuthorId { get; set; }

        public string Content { get; set; }

        public int Id { get; set; }

        public DateTime? PublishDate { get; set; }

        public string Title { get; set; }
    }
}