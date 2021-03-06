﻿using System;
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
    public class BooksController : Controller
    {
        private BookStoreEntities db = new BookStoreEntities();

        public PartialViewResult GetStock(int bookId)
        {
            BookStockViewModel objReturnStock = GetStockCount(bookId);

            return PartialView("~/Views/Shared/_StockForBooks.cshtml", objReturnStock);

        }

        /*@Param Integer value of book id
         *@return An object view model for Book and Stock
         * A method which will pass back the correct stock 
         * for the desired book and a BookStockViewModel
         */ 
        private BookStockViewModel GetStockCount(int bookId)
        {
            var Query = (from p in db.Stocks
                         where p.BookID == bookId
                         select p.StockCount).FirstOrDefault();
            BookStockViewModel objReturn = new BookStockViewModel();
            objReturn.stock.StockCount = Query;

            return objReturn;
        }

        // GET: Books
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Author);
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            BookStockViewModel books = new BookStockViewModel();
            books.book = book;
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorFirstName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "BookName,ReleaseYear,AuthorID")] Book book)
        {
            if (ModelState.IsValid)
            {
                Stock newStock = new Stock();

                int currentLeargetStockId = 0;
                int bookId = 0;
                try
                {
                    currentLeargetStockId = db.Stocks.Max(x => x.StockID);
                } catch (Exception)
                {
                    //DO NOTHING
                }

                try
                {
                    bookId = db.Books.Max(x => x.BookID);
                } catch (Exception)
                {
                    //DO Nothing
                }
                book.BookID = ++bookId;

                newStock.StockID = currentLeargetStockId + 1;
                newStock.StockCount = 0;
                newStock.BookID = book.BookID;
                newStock.LoanLength = 5;
                db.Stocks.Add(newStock);

                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorFirstName", book.AuthorID);
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorFirstName", book.AuthorID);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "BookID,BookName,ReleaseYear,AuthorID")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorFirstName", book.AuthorID);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            int BookId = id;
            Stock stock = (from p in db.Stocks
                          where p.BookID == id
                          select p).FirstOrDefault();
            if(stock != null)
            {
                db.Stocks.Remove(stock);
            }
            db.Books.Remove(book);
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
    }
}
