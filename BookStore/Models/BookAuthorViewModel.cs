using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class BookAuthorViewModel
    {
        public virtual Book Book { get; set; }

        public virtual Author Author { get; set; }

    }
}