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
    public class SoilsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoilsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Soils
        public async Task<IActionResult> Index(
         string searchName,
         string searchPH,
         string sortOrder,
         int pageNumber = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PHSort"] = sortOrder == "PH" ? "ph_desc" : "PH";
            ViewData["PriceSort"] = sortOrder == "Price" ? "price_desc" : "Price";

            ViewData["SearchName"] = searchName;
            ViewData["SearchPH"] = searchPH;

            var soils = from s in _context.Soils
                        select s;

            // търсене по 2 критерия
            if (!string.IsNullOrEmpty(searchName))
                soils = soils.Where(s => s.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPH))
                soils = soils.Where(s => s.PH.ToString().Contains(searchPH));

            // сортиране
            soils = sortOrder switch
            {
                "name_desc" => soils.OrderByDescending(s => s.Name),
                "PH" => soils.OrderBy(s => s.PH),
                "ph_desc" => soils.OrderByDescending(s => s.PH),
                "Price" => soils.OrderBy(s => s.PricePerBag),
                "price_desc" => soils.OrderByDescending(s => s.PricePerBag),
                _ => soils.OrderBy(s => s.Name)
            };

            // странициране
            int pageSize = 5;
            return View(await PaginatedList<Soil>.CreateAsync(soils.AsNoTracking(), pageNumber, pageSize));
        }


        // GET: Soils/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var soil = await _context.Soils
                .FirstOrDefaultAsync(m => m.Id == id);
            if (soil == null)
            {
                return NotFound();
            }

            return View(soil);
        }

        // GET: Soils/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Soils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,PH,BagWeightKg,PricePerBag,InStockQuantity,IsOrganic,CreatedOn, Description")] Soil soil)
        {
            if (ModelState.IsValid)
            {
                _context.Add(soil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(soil);
        }

        // GET: Soils/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var soil = await _context.Soils.FindAsync(id);
            if (soil == null)
            {
                return NotFound();
            }
            return View(soil);
        }

        // POST: Soils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PH,BagWeightKg,PricePerBag,InStockQuantity,IsOrganic,CreatedOn, Description")] Soil soil)
        {
            if (id != soil.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(soil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoilExists(soil.Id))
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
            return View(soil);
        }

        // GET: Soils/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var soil = await _context.Soils
                .FirstOrDefaultAsync(m => m.Id == id);
            if (soil == null)
            {
                return NotFound();
            }

            return View(soil);
        }

        // POST: Soils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var soil = await _context.Soils.FindAsync(id);
            if (soil != null)
            {
                _context.Soils.Remove(soil);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SoilExists(int id)
        {
            return _context.Soils.Any(e => e.Id == id);
        }
    }
}
