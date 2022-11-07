using MC.Data;
using MC.Data.Entities;
using PagedList;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MC.Website.Controllers
{
    public class MoviesController : Controller
    {
        private MovieCatalogDbContext db = new MovieCatalogDbContext();

        // GET: Movies
        public ActionResult Index(string searchTitle, int? searchGenreId, int? page, string sortOrder)
        {
            int pageCurrent = page ?? 1; //page == null ? 1 : page
            int pageMaxSize = 3;

            var movies = db.Movies.Include(m => m.Genre).AsQueryable();
            ViewBag.Genres = new SelectList(db.Genres, "Id", "Value");
            ViewBag.TitleSearch = searchTitle;

            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(searchTitle))
                    movies = movies.Where(x => x.Title.Contains(searchTitle));

                if (searchGenreId.HasValue)
                    movies = movies.Where(x => x.GenreId == searchGenreId);
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title-desc" : "";
            ViewBag.ReleaseDateSortParam = sortOrder == "rdate-desc" ? "rdate-asc" : "rdate-desc";

            switch (sortOrder)
            {
                case "title-desc":
                    movies = movies.OrderByDescending(x => x.Title);
                    break;
                case "rdate-asc":
                    movies = movies.OrderBy(x => x.ReleaseDate);
                    break;
                case "rdate-desc":
                    movies = movies.OrderByDescending(x => x.ReleaseDate);
                    break;
                default:
                    movies = movies.OrderBy(x => x.Title);
                    break;
            }

            return View(movies.ToPagedList(pageCurrent, pageMaxSize));
        }

        // GET: Movies/Details/5
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        // GET: Movies/Create
        public ActionResult Create()
        {
            ViewBag.Genres = new SelectList(db.Genres, "Id", "Value");
            ViewBag.Writers = new SelectList(db.Writers, "Id", "UserName");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ReleaseDate,GenreId,WriterId,Rating,Country")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Genres = new SelectList(db.Genres, "Id", "Value", movie.GenreId);
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
            ViewBag.Genres = new SelectList(db.Genres, "Id", "Value", movie.GenreId);
            ViewBag.Writers = new SelectList(db.Writers, "Id", "UserName");
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ReleaseDate,GenreId,WriterId,Rating,Country")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Genres = new SelectList(db.Genres, "Id", "Value", movie.GenreId);
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
