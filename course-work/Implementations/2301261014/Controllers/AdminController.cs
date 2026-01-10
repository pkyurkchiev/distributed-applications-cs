using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceTrack.Data;
using RaceTrack.Models;

namespace RaceTrack.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Проверка дали е администратор
        private bool IsAdmin()
        {
            var userLevel = HttpContext.Session.GetInt32("UserLevel");
            return userLevel == 2;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            // Статистики
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalBookings = await _context.Bookings.CountAsync();
            ViewBag.PendingBookings = await _context.Bookings.CountAsync(b => b.Status == "Pending");
            ViewBag.TotalRevenue = await _context.Bookings.SumAsync(b => (decimal?)b.Price) ?? 0;
            ViewBag.TotalDocuments = await _context.Documents.CountAsync();
            ViewBag.PendingDocuments = await _context.Documents.CountAsync(d => d.Status == "Pending");

            // Последни 5 регистрирани потребители
            var recentUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Последни 10 качени документи
            var recentDocuments = await _context.Documents
                .Include(d => d.Booking)
                .ThenInclude(b => b.User)
                .OrderByDescending(d => d.UploadDate)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentUsers = recentUsers;
            ViewBag.RecentDocuments = recentDocuments;

            // Последни 10 резервации
            var recentBookings = await _context.Bookings
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .Take(10)
                .ToListAsync();

            return View("~/Views/Booking/Admin/Dashboard.cshtml", recentBookings);
        }

        // GET: Admin/AllBookings - ДОБАВЕНО ДИНАМИЧНО СОРТИРАНЕ
        public async Task<IActionResult> AllBookings(string search, string status, string sortBy = "BookingDate", string sortOrder = "desc", int page = 1)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Bookings.Include(b => b.User).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.FullName.Contains(search) ||
                                        b.Email.Contains(search) ||
                                        b.Phone.Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.Status == status);
            }

            // ✅ ДОБАВЕНО: Динамично сортиране
            query = sortBy switch
            {
                "FullName" => sortOrder == "asc" ? query.OrderBy(b => b.FullName) : query.OrderByDescending(b => b.FullName),
                "Email" => sortOrder == "asc" ? query.OrderBy(b => b.Email) : query.OrderByDescending(b => b.Email),
                "Plan" => sortOrder == "asc" ? query.OrderBy(b => b.Plan) : query.OrderByDescending(b => b.Plan),
                "Status" => sortOrder == "asc" ? query.OrderBy(b => b.Status) : query.OrderByDescending(b => b.Status),
                "Price" => sortOrder == "asc" ? query.OrderBy(b => b.Price) : query.OrderByDescending(b => b.Price),
                _ => sortOrder == "asc" ? query.OrderBy(b => b.BookingDate) : query.OrderByDescending(b => b.BookingDate)
            };

            int pageSize = 20;
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var bookings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View("~/Views/Booking/Admin/AllBookings.cshtml", bookings);
        }

        // GET: Admin/BookingDetails
        public async Task<IActionResult> BookingDetails(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Documents)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View("~/Views/Booking/Admin/BookingDetails.cshtml", booking);
        }

        // POST: Admin/UpdateBookingStatus
        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, string status)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Статусът е обновен успешно!";
            return RedirectToAction("BookingDetails", new { id });
        }

        // POST: Admin/UpdateDocumentStatus
        [HttpPost]
        public async Task<IActionResult> UpdateDocumentStatus(int id, string status)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            document.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Статусът на документа е обновен!";
            return RedirectToAction("BookingDetails", new { id = document.BookingId });
        }

        // GET: Admin/AllUsers - ДОБАВЕНО ДИНАМИЧНО СОРТИРАНЕ
        public async Task<IActionResult> AllUsers(string search, string sortBy = "CreatedAt", string sortOrder = "desc", int page = 1)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Users.Include(u => u.Bookings).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.FullName.Contains(search) ||
                                        u.Email.Contains(search) ||
                                        u.Username.Contains(search));
            }

            // ✅ ДОБАВЕНО: Динамично сортиране
            query = sortBy switch
            {
                "FullName" => sortOrder == "asc" ? query.OrderBy(u => u.FullName) : query.OrderByDescending(u => u.FullName),
                "Username" => sortOrder == "asc" ? query.OrderBy(u => u.Username) : query.OrderByDescending(u => u.Username),
                "Email" => sortOrder == "asc" ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "UserLevel" => sortOrder == "asc" ? query.OrderBy(u => u.UserLevel) : query.OrderByDescending(u => u.UserLevel),
                _ => sortOrder == "asc" ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt)
            };

            int pageSize = 20;
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View("~/Views/Booking/Admin/AllUsers.cshtml", users);
        }

        // GET: Admin/UserDetails
        public async Task<IActionResult> UserDetails(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Documents)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View("~/Views/Booking/Admin/UserDetails.cshtml", user);
        }

        // GET: Admin/AllDocuments - ДОБАВЕНО ДИНАМИЧНО СОРТИРАНЕ
        public async Task<IActionResult> AllDocuments(string search, string status, string sortBy = "UploadDate", string sortOrder = "desc", int page = 1)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Documents
                .Include(d => d.Booking)
                .ThenInclude(b => b.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Booking.User.FullName.Contains(search) ||
                                        d.FileName.Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(d => d.Status == status);
            }

            // ✅ ДОБАВЕНО: Динамично сортиране
            query = sortBy switch
            {
                "FileName" => sortOrder == "asc" ? query.OrderBy(d => d.FileName) : query.OrderByDescending(d => d.FileName),
                "DocumentType" => sortOrder == "asc" ? query.OrderBy(d => d.DocumentType) : query.OrderByDescending(d => d.DocumentType),
                "Status" => sortOrder == "asc" ? query.OrderBy(d => d.Status) : query.OrderByDescending(d => d.Status),
                "FileSize" => sortOrder == "asc" ? query.OrderBy(d => d.FileSize) : query.OrderByDescending(d => d.FileSize),
                _ => sortOrder == "asc" ? query.OrderBy(d => d.UploadDate) : query.OrderByDescending(d => d.UploadDate)
            };

            int pageSize = 20;
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var documents = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            return View("~/Views/Booking/Admin/AllDocuments.cshtml", documents);
        }

        // GET: Admin/ViewDocument
        public async Task<IActionResult> ViewDocument(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var document = await _context.Documents
                .Include(d => d.Booking)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(d => d.DocumentId == id);

            if (document == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('~', '/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файлът не е намерен на сървъра.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(document.FileName);

            return File(fileBytes, contentType);
        }

        // POST: Admin/ApproveBooking
        [HttpPost]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Резервацията е одобрена!";
            }

            return RedirectToAction("BookingDetails", new { id });
        }

        // POST: Admin/RejectBooking
        [HttpPost]
        public async Task<IActionResult> RejectBooking(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Резервацията е отказана!";
            }

            return RedirectToAction("BookingDetails", new { id });
        }

        // POST: Admin/ApproveDocument
        [HttpPost]
        public async Task<IActionResult> ApproveDocument(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                document.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Документът е одобрен!";
            }

            return RedirectToAction("AllDocuments");
        }

        // POST: Admin/RejectDocument
        [HttpPost]
        public async Task<IActionResult> RejectDocument(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                document.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Документът е отказан!";
            }

            return RedirectToAction("AllDocuments");
        }

        // ✅ НОВО: POST: Admin/UpdateUserLevel - Промяна на нивото на потребител
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserLevel(int userId, int newLevel)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Потребителят не е намерен!";
                return RedirectToAction("AllUsers");
            }

            // Не позволявай на админа да промени собственото си ниво
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (userId == currentUserId)
            {
                TempData["Error"] = "Не можете да промените собственото си ниво!";
                return RedirectToAction("UserDetails", new { id = userId });
            }

            user.UserLevel = newLevel;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Нивото на {user.FullName} е променено на {(newLevel == 2 ? "Администратор" : "Потребител")}!";
            return RedirectToAction("UserDetails", new { id = userId });
        }

        // ✅ НОВО: POST: Admin/DeleteBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings
                .Include(b => b.Documents)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                TempData["Error"] = "Резервацията не е намерена!";
                return RedirectToAction("AllBookings");
            }

            // Изтрий всички документи свързани с резервацията
            foreach (var doc in booking.Documents)
            {
                var filePath = Path.Combine(_environment.WebRootPath, doc.FilePath.TrimStart('~', '/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Резервацията е изтрита успешно!";
            return RedirectToAction("AllBookings");
        }

        // ✅ НОВО: POST: Admin/DeleteDocument
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                TempData["Error"] = "Документът не е намерен!";
                return RedirectToAction("AllDocuments");
            }

            // Изтрий физическия файл
            var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('~', '/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var bookingId = document.BookingId;
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Документът е изтрит успешно!";
            return RedirectToAction("BookingDetails", new { id = bookingId });
        }

        // ✅ НОВО: POST: Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Documents)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                TempData["Error"] = "Потребителят не е намерен!";
                return RedirectToAction("AllUsers");
            }

            // Не позволявай изтриване на собствения акаунт
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (id == currentUserId)
            {
                TempData["Error"] = "Не можете да изтриете собствения си акаунт!";
                return RedirectToAction("UserDetails", new { id });
            }

            // Изтрий всички файлове на потребителя
            foreach (var booking in user.Bookings)
            {
                foreach (var doc in booking.Documents)
                {
                    var filePath = Path.Combine(_environment.WebRootPath, doc.FilePath.TrimStart('~', '/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Потребителят {user.FullName} е изтрит успешно!";
            return RedirectToAction("AllUsers");
        }

        // Helper method за определяне на content type
        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}