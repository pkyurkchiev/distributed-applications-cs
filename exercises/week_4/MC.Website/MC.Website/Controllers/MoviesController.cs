using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MC.Data.Context;
using MC.Data.Entities;
using PagedList;

namespace MC.Website.Controllers
{
    public class MoviesController : Controller
    {
        private MovieCatalogDbContext db = new MovieCatalogDbContext();

        // GET: Movies
        public ActionResult Index(int? page, string titleSearch, string sortOrder)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;
            IQueryable<Movie> movies = db.Movies.AsQueryable();

            ViewBag.TitleSearch = titleSearch;
            if (!String.IsNullOrEmpty(titleSearch))
            {
                movies = movies.Where(x => x.Title.Contains(titleSearch));
            }
            
            ViewBag.CurrentSortParm = sortOrder;
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.WriterSortParm = sortOrder == "writer_asc" ? "writer_desc" : "writer_asc";
            switch (sortOrder)
            {
                case "title_desc":
                    movies = movies.OrderByDescending(x => x.Title);
                    break;
                case "writer_asc":
                    movies = movies.OrderBy(x => x.Writer.UserName);
                    break;
                case "writer_desc":
                    movies = movies.OrderByDescending(x => x.Writer.UserName);
                    break;
                default:
                    movies = movies.OrderBy(x => x.Title);
                    break;
            }

            return View(movies.ToPagedList(pageNumber, pageSize));
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Ratings = new SelectList(db.Ratings, "Id", "RatingValue");
            ViewBag.Writers = new SelectList(db.Writers, "Id", "UserName");

            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ReleaseDate,Country,RatingId,WriterId,Description")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Ratings = new SelectList(db.Ratings, "Id", "RatingValue");
            ViewBag.Writers = new SelectList(db.Writers, "Id", "UserName");

            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            ViewBag.Ratings = new SelectList(db.Ratings, "Id", "RatingValue");
            ViewBag.Writers = new SelectList(db.Writers, "Id", "UserName");

            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ReleaseDate,Country,RatingId,WriterId,Description")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.Ratings = new SelectList(db.Ratings, "Id", "RatingValue");
            ViewBag.Writers = new SelectList(db.Writers, "Id", "UserName");

            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
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
