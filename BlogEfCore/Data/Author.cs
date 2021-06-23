using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogEfCore.Data
{
    public class Author
    {
        public string Bio { get; set; }

        public IList<BlogPost> BlogPosts { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Photo { get; set; }
    }
}