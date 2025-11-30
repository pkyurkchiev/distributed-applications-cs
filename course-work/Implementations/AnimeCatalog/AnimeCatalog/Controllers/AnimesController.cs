using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnimeCatalog.Data;
using AnimeCatalog.Models;
using Microsoft.AspNetCore.Authorization;

namespace AnimeCatalog.Controllers
{
    public class AnimesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Animes
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            // Ако цъкнеш "Рейтинг", сортира низходящо
            ViewData["RatingSortParm"] = sortOrder == "Rating" ? "rating_asc" : "Rating";

            if (searchString != null) { pageNumber = 1; }
            else { searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;

            var animes = _context.Animes.Include(a => a.Genre).Include(a => a.Writer).AsQueryable();

            if (!String.IsNullOrEmpty(searchString) && (User.IsInRole("User") || User.IsInRole("Admin")))
            {
                animes = animes.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));
            }

            if (User.IsInRole("User") || User.IsInRole("Admin"))
            {
                switch (sortOrder)
                {
                    case "title_desc":
                        animes = animes.OrderByDescending(s => s.Title);
                        break;
                    case "Rating": // Изискване: Най-висок рейтинг първо
                        animes = animes.OrderByDescending(s => s.IMDBRating);
                        break;
                    case "rating_asc":
                        animes = animes.OrderBy(s => s.IMDBRating);
                        break;
                    default: // По подразбиране: Азбучен ред
                        animes = animes.OrderBy(s => s.Title);
                        break;
                }
            }

            return View(await animes.ToListAsync());
        }

        // GET: Animes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var anime = await _context.Animes
                .Include(a => a.Genre)
                .Include(a => a.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anime == null) return NotFound();

            return View(anime);
        }

        // --- ADMIN AREA ---

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            ViewData["WriterId"] = new SelectList(_context.Writers, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,ImageUrl,Description,ReleaseDate,IMDBRating,Episodes,IsFinished,WriterId,GenreId")] Anime anime)
        {
            ModelState.Remove("Genre"); // Валидация фикс
            ModelState.Remove("Writer"); // Валидация фикс

            if (ModelState.IsValid)
            {
                _context.Add(anime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", anime.GenreId);
            ViewData["WriterId"] = new SelectList(_context.Writers, "Id", "FullName", anime.WriterId);
            return View(anime);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var anime = await _context.Animes.FindAsync(id);
            if (anime == null) return NotFound();
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", anime.GenreId);
            ViewData["WriterId"] = new SelectList(_context.Writers, "Id", "FullName", anime.WriterId);
            return View(anime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ImageUrl,Description,ReleaseDate,IMDBRating,Episodes,IsFinished,WriterId,GenreId")] Anime anime)
        {
            if (id != anime.Id) return NotFound();
            ModelState.Remove("Genre"); // Валидация фикс
            ModelState.Remove("Writer"); // Валидация фикс

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimeExists(anime.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", anime.GenreId);
            ViewData["WriterId"] = new SelectList(_context.Writers, "Id", "FullName", anime.WriterId);
            return View(anime);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var anime = await _context.Animes
                .Include(a => a.Genre)
                .Include(a => a.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anime == null) return NotFound();

            return View(anime);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime != null) _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimeExists(int id)
        {
            return _context.Animes.Any(e => e.Id == id);
        }
    }
}