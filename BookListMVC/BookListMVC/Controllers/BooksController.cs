using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext _db;

        [BindProperty]
        public Book Book { get; set; }
        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            Book = new Book();
            if (id == null)
            {
                return View(Book);
            }
            Book = await _db.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert()
        {
            if (ModelState.IsValid) 
            {
                if (Book.Id == 0)
                {
                    _db.Books.Add(Book);
                }
                else 
                {
                    _db.Books.Update(Book);
                }
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Book);
        }


        #region API calls
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int Id) 
        {
            var book = await _db.Books.FirstOrDefaultAsync(book => book.Id == Id);
            if (book == null) 
            {
                return Json(new { success = false, message = "error occurred" });
            }
            _db.Remove(book);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Successful remove book"});
        }
        #endregion
    }
}
