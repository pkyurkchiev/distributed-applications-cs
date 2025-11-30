using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;

namespace PlantShop.Controllers
{
    public class FertilizersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FertilizersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fertilizers
        public async Task<IActionResult> Index(
        string searchName,
        string searchNPK,
        string sortOrder,
        int pageNumber = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["NPKSort"] = sortOrder == "NPK" ? "npk_desc" : "NPK";
            ViewData["PriceSort"] = sortOrder == "Price" ? "price_desc" : "Price";

            ViewData["SearchName"] = searchName;
            ViewData["SearchNPK"] = searchNPK;

            var fert = from f in _context.Fertilizers
                       select f;

            // търсене
            if (!string.IsNullOrEmpty(searchName))
                fert = fert.Where(f => f.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(searchNPK))
                fert = fert.Where(f => f.NPKRatio.Contains(searchNPK));

            // сортиране
            fert = sortOrder switch
            {
                "name_desc" => fert.OrderByDescending(f => f.Name),
                "NPK" => fert.OrderBy(f => f.NPKRatio),
                "npk_desc" => fert.OrderByDescending(f => f.NPKRatio),
                "Price" => fert.OrderBy(f => f.PricePerKg),
                "price_desc" => fert.OrderByDescending(f => f.PricePerKg),
                _ => fert.OrderBy(f => f.Name)
            };

            // странициране
            int pageSize = 5;
            return View(await PaginatedList<Fertilizer>.CreateAsync(fert.AsNoTracking(), pageNumber, pageSize));
        }


        // GET: Fertilizers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fertilizer = await _context.Fertilizers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fertilizer == null)
            {
                return NotFound();
            }

            return View(fertilizer);
        }

        // GET: Fertilizers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fertilizers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,NPKRatio,PricePerKg,InStockQuantity,IsLiquid,CreatedOn, Description")] Fertilizer fertilizer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fertilizer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fertilizer);
        }

        // GET: Fertilizers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fertilizer = await _context.Fertilizers.FindAsync(id);
            if (fertilizer == null)
            {
                return NotFound();
            }
            return View(fertilizer);
        }

        // POST: Fertilizers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NPKRatio,PricePerKg,InStockQuantity,IsLiquid,CreatedOn, Description")] Fertilizer fertilizer)
        {
            if (id != fertilizer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fertilizer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FertilizerExists(fertilizer.Id))
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
            return View(fertilizer);
        }

        // GET: Fertilizers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fertilizer = await _context.Fertilizers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fertilizer == null)
            {
                return NotFound();
            }

            return View(fertilizer);
        }

        // POST: Fertilizers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fertilizer = await _context.Fertilizers.FindAsync(id);
            if (fertilizer != null)
            {
                _context.Fertilizers.Remove(fertilizer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FertilizerExists(int id)
        {
            return _context.Fertilizers.Any(e => e.Id == id);
        }
    }
}
