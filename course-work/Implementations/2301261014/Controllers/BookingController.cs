using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceTrack.Data;
using RaceTrack.Models;

namespace RaceTrack.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BookingController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Booking/SelectPlan
        public IActionResult SelectPlan()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Моля влезте в системата за да направите резервация!";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // GET: Booking/BookingForm
        public IActionResult BookingForm(string plan)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(plan))
            {
                return RedirectToAction("SelectPlan");
            }

            ViewBag.Plan = plan;

            decimal price = plan switch
            {
                "Начинаещ" => 150.00m,
                "Напредващ" => 250.00m,
                "Професионален" => 400.00m,
                _ => 0
            };
            ViewBag.Price = price;

            return View();
        }

        // POST: Booking/BookingForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookingForm(Booking booking, string plan)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            booking.UserId = userId.Value;
            booking.Plan = plan;
            booking.BookingDate = DateTime.Now;
            booking.Status = "Pending";

            booking.Price = plan switch
            {
                "Начинаещ" => 150.00m,
                "Напредващ" => 250.00m,
                "Професионален" => 400.00m,
                _ => 0
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("UploadDocuments", new { id = booking.BookingId });
        }

        // GET: Booking/UploadDocuments
        public async Task<IActionResult> UploadDocuments(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            ViewBag.BookingId = id;
            return View();
        }

        // POST: Booking/UploadDocuments
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocuments(int bookingId, IFormFile contract, IFormFile idFront, IFormFile idBack)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsFolder);

            if (contract != null && contract.Length > 0)
            {
                var fileName = $"contract_{bookingId}_{Guid.NewGuid()}{Path.GetExtension(contract.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await contract.CopyToAsync(stream);
                }

                var doc = new Document
                {
                    BookingId = bookingId,
                    DocumentType = "Contract",
                    FilePath = $"/uploads/documents/{fileName}",
                    FileName = contract.FileName,
                    FileSize = contract.Length,
                    UploadedAt = DateTime.Now,
                    Status = "Pending"
                };
                _context.Documents.Add(doc);
            }

            if (idFront != null && idFront.Length > 0)
            {
                var fileName = $"id_front_{bookingId}_{Guid.NewGuid()}{Path.GetExtension(idFront.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await idFront.CopyToAsync(stream);
                }

                var doc = new Document
                {
                    BookingId = bookingId,
                    DocumentType = "IDFront",
                    FilePath = $"/uploads/documents/{fileName}",
                    FileName = idFront.FileName,
                    FileSize = idFront.Length,
                    UploadedAt = DateTime.Now,
                    Status = "Pending"
                };
                _context.Documents.Add(doc);
            }

            if (idBack != null && idBack.Length > 0)
            {
                var fileName = $"id_back_{bookingId}_{Guid.NewGuid()}{Path.GetExtension(idBack.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await idBack.CopyToAsync(stream);
                }

                var doc = new Document
                {
                    BookingId = bookingId,
                    DocumentType = "IDBack",
                    FilePath = $"/uploads/documents/{fileName}",
                    FileName = idBack.FileName,
                    FileSize = idBack.Length,
                    UploadedAt = DateTime.Now,
                    Status = "Pending"
                };
                _context.Documents.Add(doc);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Документите са качени успешно! Резервацията ви е направена.";
            return RedirectToAction("MyBookings");
        }

        // GET: Booking/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Documents)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Booking/DownloadContract
        public IActionResult DownloadContract()
        {
            var filePath = Path.Combine(_environment.WebRootPath, "documents", "contract_template.html");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Договорът не е намерен.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "text/html", "Договор_RaceTrack.html");
        }
    }
}