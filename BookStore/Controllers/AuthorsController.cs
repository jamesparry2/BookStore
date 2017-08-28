using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class AuthorsController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        // GET: Authors
        public ActionResult Index()
        {
            return View(db.Authors.ToList());
        }

        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Authors/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "AuthorFirstName,AuthorLastName,AuthorBirthYear")] Author author)
        {
            if (ModelState.IsValid)
            {
                int AuthorId = 0;
                try
                {
                    AuthorId = db.Authors.Max(x => x.AuthorID);
                    author.AuthorID = ++AuthorId;
                } catch (Exception e)
                {
                    author.AuthorID = ++AuthorId;
                }

                db.Authors.Add(author);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "AuthorID,AuthorFirstName,AuthorLastName,AuthorBirthYear")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Author author = db.Authors.Find(id);

            //Find all the books associated with the author
            List<Book> book = (from p in db.Books
                              where p.AuthorID == id
                              select p).ToList();

            //Check if there are any books exisist  
            if (book != null)
            {
                //Loop through each book and find the stock assoicated with that book and removed it 
                foreach (var item in book)
                {
                    Stock stock = (from p in db.Stocks
                                   where p.BookID == item.BookID
                                   select p).First();
                    db.Stocks.Remove(stock);

                }

                //Loop through and remove the book
                foreach (var item in book)
                {
                    db.Books.Remove(item);
                }
            }

            //Finally remove the author
            db.Authors.Remove(author);
            db.SaveChanges();
            return RedirectToAction("Index");
        } 

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //For the partail view, returns to the model which books relate
        //to the corresponding author
        [HttpGet]
        public ActionResult _BookForAuthor(int AuthorId)
        {
            var Query = (from p in db.Books
                         where p.AuthorID == AuthorId
                         select p.BookName).ToList();

            return PartialView("_BooksForAuthor", Query.AsEnumerable());
        }
    }
}
