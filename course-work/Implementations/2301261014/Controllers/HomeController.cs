using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceTrack.Data;
using RaceTrack.Models;

namespace RaceTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // ? ????: ???????? ?????? ?? ????? (???? 0) - ???? ??????
        public async Task<IActionResult> ViewBookings(int page = 1)
        {
            int pageSize = 20;

            var query = _context.Bookings
                .Include(b => b.User)
                .Where(b => b.Status == "Approved") // ???? ???????? ??????????
                .OrderByDescending(b => b.BookingDate);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var bookings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(bookings);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}