using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ComicBookLibraryManagerWebApp.ViewModels;
using System.Net;
using System.Data.Entity.Infrastructure;
using ComicBookShared.Data;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Comic Books" section of the website.
    /// </summary>
    public class ComicBooksController : BaseController
    {

        public ActionResult Index()
        {
            // Include the "Series" navigation property.
            //var comicBooks = new List<ComicBook>();

            // TODO Get the comic books list.
            var comicBooks = Context.ComicBooks // Replaced all _context With Context Property
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();

            return View(comicBooks);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Include the "Series", "Artists.Artist", and "Artists.Role" navigation properties.
            // var comicBook = new ComicBook();

            // TODO Get the comic book.
            var comicBook = Context.ComicBooks
                    .Include(cb => cb.Series)
                    .Include(cb => cb.Artists.Select(a => a.Artist))
                    .Include(cb => cb.Artists.Select(a => a.Role))
                    .Where(cb => cb.Id == id)
                    .SingleOrDefault();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            // Sort the artists.
            comicBook.Artists = comicBook.Artists.OrderBy(a => a.Role.Name).ToList();

            return View(comicBook);
        }

        public ActionResult Add()
        {
            var viewModel = new ComicBooksAddViewModel();

            // TODO Pass the Context class to the view model "Init" method.
            viewModel.Init(Context);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(ComicBooksAddViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;
                comicBook.AddArtist(viewModel.ArtistId, viewModel.RoleId);

                // TODO Add the comic book to the DB.
                Context.ComicBooks.Add(comicBook);
                Context.SaveChanges();

                TempData["Message"] = "Your comic book was successfully added!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            // TODO Pass the Context class to the view model "Init" method.
            viewModel.Init(Context);

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get the comic book.
            var comicBook = Context.ComicBooks.Where( cb => cb.Id == id).SingleOrDefault();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            // if a comicbook is found
            var viewModel = new ComicBooksEditViewModel()
            {
                ComicBook = comicBook
            };
            viewModel.Init(Context);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ComicBooksEditViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;

                // TODO Update the comic book.
                Context.Entry(comicBook).State = EntityState.Modified; // Changing the state to modified will cause a sql update statement
                Context.SaveChanges();

                /* But will update all columns and sometimes you might not wwant to do that so in that case: 
                 * Let's look at a couple of approaches that you can use to avoid EF creating a SQL update statement 
                 * that updates every table column.
                 *
                 * First off, if you're just looking to prevent updating a single column (or handful of columns), 
                 * you can use an entity's context entry to change a property's IsModified property to false. 
                 * For example, this line of code would keep the IssueNumber column from being included in the SQL update statement:     
                 * 
                 * comicBookEntry.Property("IssueNumber").IsModified = false;
                 * 
                 * Another approach is to retrieve the entity from the database 
                 * (so that the context is tracking it) and then update that entity with the new values 
                 * from the user's post data. A convenient way of doing that is to use the Controller class' 
                 * UpdateModel or TryUpdateModel method to update the entity that's retrieved from the database.
                 * 
                 * // Creating an anonymous object here in order to keep the shape of the model
                 *   // the same as the incoming form post data.
                 *   var comicBookToUpdate = new
                 *   {
                 *       ComicBook = _comicBooksRepository.Get(comicBook.Id)
                 *   };
                 *   UpdateModel(comicBookToUpdate);
                 *   Context.SaveChanges();
                 *   
                 *   Securtity Flaw to check out
                 *   https://odetocode.com/blogs/scott/archive/2012/03/11/complete-guide-to-mass-assignment-in-asp-net-mvc.aspx
                 *   
                 * */

                TempData["Message"] = "Your comic book was successfully updated!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            viewModel.Init(Context);

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get the comic book.          
            // Include the "Series" navigation property.
            var comicBook = Context.ComicBooks
                .Include( cb => cb.Series )
                .Where( cb => cb.Id == id )
                .SingleOrDefault();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            return View(comicBook);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO Delete the comic book.
            var comicBook = new ComicBook() { Id = id };
            Context.Entry(comicBook).State = EntityState.Deleted;
            Context.SaveChanges();

            TempData["Message"] = "Your comic book was successfully deleted!";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Validates a comic book on the server
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="comicBook">The comic book to validate.</param>
        private void ValidateComicBook(ComicBook comicBook)
        {
            // If there aren't any "SeriesId" and "IssueNumber" field validation errors...
            if (ModelState.IsValidField("ComicBook.SeriesId") &&
                ModelState.IsValidField("ComicBook.IssueNumber"))
            {
                // Then make sure that the provided issue number is unique for the provided series.
                // TODO Call method to check if the issue number is available for this comic book.
                if (Context.ComicBooks.Any(cb => cb.Id  != comicBook.Id && 
                                            cb.SeriesId == comicBook.SeriesId &&
                                            cb.IssueNumber == comicBook.IssueNumber) ) // .Any checks and does not returns true | false
                {
                    ModelState.AddModelError("ComicBook.IssueNumber",
                        "The provided Issue Number has already been entered for the selected Series.");
                }
            }
        }
    }
}