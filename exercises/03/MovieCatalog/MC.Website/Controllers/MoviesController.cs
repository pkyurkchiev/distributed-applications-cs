using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MC.Data.Entities;
using MC.Website.Data;
using X.PagedList;
using X.PagedList.EF;

namespace MC.Website.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieCatalogDbContext _context;

        public MoviesController(MovieCatalogDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string searchTitle, int? searchGenreId, string sortOrder, int? page)
        {
            int pageCurrent = page ?? 1; //page == null ? 1 : page
            int pageMaxSize = 3;

            var movies = _context.Movies.Include(m => m.Genre).Include(w => w.Writer).AsQueryable();
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Value");

            ViewBag.TitleSearch = searchTitle;
            if (!string.IsNullOrEmpty(searchTitle))
                movies = movies.Where(x => x.Title.Contains(searchTitle));

            ViewBag.GenreIdSearch = searchGenreId.ToString();
            if (searchGenreId.HasValue)
                movies = movies.Where(x => x.GenreId == searchGenreId);

            ViewBag.SortOrder = sortOrder;
            ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title-desc" : "";
            ViewBag.ReleaseDateSortParam = sortOrder == "rdate-desc" ? "rdate-asc" : "rdate-desc";

            movies = sortOrder switch
            {
                "title-desc" => movies.OrderByDescending(x => x.Title),
                "rdate-asc" => movies.OrderBy(x => x.ReleaseDate),
                "rdate-desc" => movies.OrderByDescending(x => x.ReleaseDate),
                _ => movies.OrderBy(x => x.Title),
            };
            return View(await movies.ToPagedListAsync(pageCurrent, pageMaxSize));
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Value");
            ViewData["WriterId"] = new SelectList(_context.Set<Writer>(), "Id", "UserName");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,GenreId,WriterId,Rating,Country")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Value");
            ViewData["WriterId"] = new SelectList(_context.Set<Writer>(), "Id", "UserName");

            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Value", movie.GenreId);
            ViewData["WriterId"] = new SelectList(_context.Set<Writer>(), "Id", "UserName", movie.WriterId);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,GenreId,WriterId,Rating,Country")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Value", movie.GenreId);
            ViewData["WriterId"] = new SelectList(_context.Set<Writer>(), "Id", "UserName", movie.WriterId);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'MovieCatalogDbContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return _context.Movies.Any(e => e.Id == id);
        }
    }
}
