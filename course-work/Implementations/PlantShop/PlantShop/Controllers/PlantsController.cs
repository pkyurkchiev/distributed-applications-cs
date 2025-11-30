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
    public class PlantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Plants
        public async Task<IActionResult> Index(
        string searchName,
        string searchSpecies,
        string sortOrder,
        int pageNumber = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSort"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["StockSort"] = sortOrder == "Stock" ? "stock_desc" : "Stock";

            ViewData["SearchName"] = searchName;
            ViewData["SearchSpecies"] = searchSpecies;

            //var plants = from p in _context.Plants
            //             select p;
            var plants = _context.Plants
                        .Include(p => p.Soil)
                        .Include(p => p.Fertilizer)
                        .AsQueryable();


            // търсене по две полета
            if (!string.IsNullOrEmpty(searchName))
                plants = plants.Where(p => p.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(searchSpecies))
                plants = plants.Where(p => p.Species.Contains(searchSpecies));

            // сортиране
            plants = sortOrder switch
            {
                "name_desc" => plants.OrderByDescending(p => p.Name),
                "Price" => plants.OrderBy(p => p.Price),
                "price_desc" => plants.OrderByDescending(p => p.Price),
                "Stock" => plants.OrderBy(p => p.StockQuantity),
                "stock_desc" => plants.OrderByDescending(p => p.StockQuantity),
                _ => plants.OrderBy(p => p.Name)
            };

            // странициране
            int pageSize = 5;
            return View(await PaginatedList<Plant>.CreateAsync(plants.AsNoTracking(), pageNumber, pageSize));
        }


        // GET: Plants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants
                .Include(p => p.Fertilizer)
                .Include(p => p.Soil)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plant == null)
            {
                return NotFound();
            }

            return View(plant);
        }

        // GET: Plants/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FertilizerId"] = new SelectList(_context.Fertilizers, "Id", "Name");
            ViewData["SoilId"] = new SelectList(_context.Soils, "Id", "Name");
            return View();
        }

        // POST: Plants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Species,Price,StockQuantity,IsIndoor,PlantedOn,SoilId,FertilizerId, Description")] Plant plant)
        {
            Console.WriteLine(">>> POST ENTERED");
            ViewBag.DebugMessage = "POST entered";
            

            if (ModelState.IsValid)
            {
                _context.Add(plant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                foreach (var e in ModelState)
                {
                    Console.WriteLine($"KEY = {e.Key}");
                    foreach (var err in e.Value.Errors)
                    {
                        Console.WriteLine($"ERROR = {err.ErrorMessage}");
                    }
                }
            }
            Console.WriteLine("Didn't");
            ViewData["FertilizerId"] = new SelectList(_context.Fertilizers, "Id", "Name", plant.FertilizerId);
            ViewData["SoilId"] = new SelectList(_context.Soils, "Id", "Name", plant.SoilId);
            return View(plant);
        }

        // GET: Plants/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants.FindAsync(id);
            if (plant == null)
            {
                return NotFound();
            }
            ViewData["FertilizerId"] = new SelectList(_context.Fertilizers, "Id", "Name", plant.FertilizerId);
            ViewData["SoilId"] = new SelectList(_context.Soils, "Id", "Name", plant.SoilId);
            return View(plant);
        }

        // POST: Plants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,Price,StockQuantity,IsIndoor,PlantedOn,SoilId,FertilizerId, Description")] Plant plant)
        {
            if (id != plant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlantExists(plant.Id))
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
            ViewData["FertilizerId"] = new SelectList(_context.Fertilizers, "Id", "Name", plant.FertilizerId);
            ViewData["SoilId"] = new SelectList(_context.Soils, "Id", "Name", plant.SoilId);
            return View(plant);
        }

        // GET: Plants/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants
                .Include(p => p.Fertilizer)
                .Include(p => p.Soil)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plant == null)
            {
                return NotFound();
            }

            return View(plant);
        }

        // POST: Plants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant != null)
            {
                _context.Plants.Remove(plant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlantExists(int id)
        {
            return _context.Plants.Any(e => e.Id == id);
        }
    }
}
