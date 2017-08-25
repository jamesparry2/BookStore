using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class BookStockViewModel
    {
        public BookStockViewModel()
        {
            book = new Book();
            stock = new Stock();
            author = new Author();
        }

        public virtual Book book { get; set; }

        public virtual Stock stock { get; set; }

        public virtual Author author { get; set; }

    }
}