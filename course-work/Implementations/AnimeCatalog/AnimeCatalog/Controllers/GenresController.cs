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
    public class GenresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            // Параметри за линковете за сортиране
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            // Ако цъкнеш "Популярност", ще сортира низходящо (10 -> 0)
            ViewData["PopSortParm"] = sortOrder == "Popularity" ? "pop_asc" : "Popularity";

            ViewData["CurrentFilter"] = searchString;

            var genres = _context.Genres.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                genres = genres.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    genres = genres.OrderByDescending(s => s.Name);
                    break;
                case "Popularity": // Изискване: Най-популярните най-отгоре
                    genres = genres.OrderByDescending(s => s.PopularityRating);
                    break;
                case "pop_asc":
                    genres = genres.OrderBy(s => s.PopularityRating);
                    break;
                default: // По подразбиране: Азбучен ред (A-Z)
                    genres = genres.OrderBy(s => s.Name);
                    break;
            }

            return View(await genres.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres.FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null) return NotFound();

            return View(genre);
        }

        // --- ADMIN AREA ---

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsAgeRestricted,PopularityRating")] Genre genre)
        {
            genre.CreatedDate = DateTime.Now;
            genre.AnimeCount = 0;
            ModelState.Remove("Animes"); // Валидация фикс

            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedDate,IsAgeRestricted,PopularityRating,AnimeCount")] Genre genre)
        {
            if (id != genre.Id) return NotFound();
            ModelState.Remove("Animes"); // Валидация фикс

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres.FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null) return NotFound();

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null) _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}