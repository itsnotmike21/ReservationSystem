using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            IQueryable<Reservation> reservations = _context.Reservations
                .Include(r => r.Facility)
                .Include(r => r.User);

            if (role != "Admin")
            {
                reservations = reservations.Where(r => r.UserId == userId);
            }

            return View(await reservations.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Facility)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(int? facilityId, DateTime? startDate)
        {
            if (startDate.HasValue)
            {
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-ddTHH:mm");
            }

            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", facilityId);
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FacilityId,StartTime,EndTime,Notes")] Reservation reservation)
        {
            reservation.UserId = HttpContext.Session.GetInt32("UserId").Value;

            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError(string.Empty, "Czas rozpoczęcia musi być wcześniejszy niż czas zakończenia.");
            }

            bool hasConflict = await _context.Reservations
                .AnyAsync(r =>
                    r.FacilityId == reservation.FacilityId &&
                    r.StartTime < reservation.EndTime &&
                    reservation.StartTime < r.EndTime);

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "Istnieje już rezerwacja dla tego obiektu w podanym przedziale czasu.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            var role = HttpContext.Session.GetString("UserRole");

            if (role == "Admin")
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", reservation.UserId);

            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,FacilityId,StartTime,EndTime,Notes")] Reservation reservation)
        {
            if (id != reservation.Id)
                return NotFound();

            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError(string.Empty, "Czas rozpoczęcia musi być wcześniejszy niż czas zakończenia.");
            }

            bool hasConflict = await _context.Reservations
                .AnyAsync(r =>
                    r.Id != reservation.Id &&
                    r.FacilityId == reservation.FacilityId &&
                    r.StartTime < reservation.EndTime &&
                    reservation.StartTime < r.EndTime);

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "Istnieje już rezerwacja dla tego obiektu w podanym przedziale czasu.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var role = HttpContext.Session.GetString("UserRole");

            if (role == "Admin")
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", reservation.UserId);

            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);

            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Facility)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // TIMELINE — lista rezerwacji pogrupowana po dniach
        public async Task<IActionResult> Timeline()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            IQueryable<Reservation> reservations = _context.Reservations
                .Include(r => r.Facility)
                .Include(r => r.User)
                .OrderByDescending(r => r.StartTime);

            if (role != "Admin")
            {
                reservations = reservations.Where(r => r.UserId == userId);
            }

            var grouped = await reservations
                .GroupBy(r => r.StartTime.Date)
                .ToListAsync();

            return View(grouped);
        }

        // KALENDARZ MIESIĘCZNY
        public async Task<IActionResult> MonthlyCalendar(int? year, int? month)
        {
            var now = DateTime.Now;

            int y = year ?? now.Year;
            int m = month ?? now.Month;

            var firstDay = new DateTime(y, m, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            IQueryable<Reservation> reservations = _context.Reservations
                .Where(r => r.StartTime.Date >= firstDay && r.StartTime.Date <= lastDay);

            if (role != "Admin")
                reservations = reservations.Where(r => r.UserId == userId);

            var grouped = await reservations
                .GroupBy(r => r.StartTime.Date)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            ViewBag.Year = y;
            ViewBag.Month = m;
            ViewBag.FirstDay = firstDay;
            ViewBag.LastDay = lastDay;
            ViewBag.Reservations = grouped;

            return View();
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}