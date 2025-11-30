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
    public class WritersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WritersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Writers
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            // Ако цъкнеш "Дата", сортира от най-стария (OldestFirst)
            ViewData["DateSortParm"] = sortOrder == "DateOldest" ? "date_newest" : "DateOldest";

            ViewData["CurrentFilter"] = searchString;

            var writers = _context.Writers.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                writers = writers.Where(s => s.FullName.Contains(searchString) || s.Biography.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    writers = writers.OrderByDescending(s => s.FullName);
                    break;
                case "DateOldest": // Изискване: Най-възрастните (най-стара дата) първо
                    writers = writers.OrderBy(s => s.BirthDate);
                    break;
                case "date_newest":
                    writers = writers.OrderByDescending(s => s.BirthDate);
                    break;
                default: // По подразбиране: Азбучен ред
                    writers = writers.OrderBy(s => s.FullName);
                    break;
            }

            return View(await writers.ToListAsync());
        }

        // GET: Writers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var writer = await _context.Writers.FirstOrDefaultAsync(m => m.Id == id);
            if (writer == null) return NotFound();

            return View(writer);
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
        public async Task<IActionResult> Create([Bind("Id,FullName,ImageUrl,BirthDate,YearsActive,NetWorthMillions,IsRetired,Biography")] Writer writer)
        {
            ModelState.Remove("Animes"); // Валидация фикс

            if (ModelState.IsValid)
            {
                _context.Add(writer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(writer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var writer = await _context.Writers.FindAsync(id);
            if (writer == null) return NotFound();
            return View(writer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ImageUrl,BirthDate,YearsActive,NetWorthMillions,IsRetired,Biography")] Writer writer)
        {
            if (id != writer.Id) return NotFound();
            ModelState.Remove("Animes"); // Валидация фикс

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(writer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WriterExists(writer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(writer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var writer = await _context.Writers.FirstOrDefaultAsync(m => m.Id == id);
            if (writer == null) return NotFound();

            return View(writer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var writer = await _context.Writers.FindAsync(id);
            if (writer != null) _context.Writers.Remove(writer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WriterExists(int id)
        {
            return _context.Writers.Any(e => e.Id == id);
        }
    }
}