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
            var role = HttpContext.Session.GetString("UserRole");

            //// ZMIANA: Admin dostaje listę użytkowników
            if (role == "Admin")
            {
                ViewBag.UserId = new SelectList(_context.Users, "Id", "Email");
            }

            //// ZMIANA: Dodano listę obiektów dla usera i admina
            ViewBag.FacilityId = new SelectList(_context.Facilities, "Id", "Name", facilityId);

            //// ZMIANA: Pobieramy obiekt, aby wyświetlić nazwę i cenę
            ViewBag.Facility = _context.Facilities.FirstOrDefault(f => f.Id == facilityId);

            //// ZMIANA: Ustawiamy poprawne domyślne daty
            var start = startDate ?? DateTime.Now;

            return View(new Reservation
            {
                FacilityId = facilityId ?? 0,
                StartTime = start,
                EndTime = start.AddHours(1)   //// ZMIANA: EndTime nie jest 01/01/0001
            });
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FacilityId,StartTime,EndTime,Notes")] Reservation reservation)
        {
            var role = HttpContext.Session.GetString("UserRole");

            //// ZMIANA: UserId ustawiane automatycznie dla zwykłego usera
            if (role != "Admin")
            {
                reservation.UserId = HttpContext.Session.GetInt32("UserId").Value;
            }

            //// Walidacja czasu
            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError(string.Empty, "Czas rozpoczęcia musi być wcześniejszy niż czas zakończenia.");
            }

            //// Sprawdzenie konfliktu rezerwacji
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

            //// ZMIANA: ponowne załadowanie dropdownów po błędzie
            if (role == "Admin")
            {
                ViewBag.UserId = new SelectList(_context.Users, "Id", "Email", reservation.UserId);
            }

            ViewBag.FacilityId = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);

            //// ZMIANA: ponowne przekazanie obiektu do widoku
            ViewBag.Facility = _context.Facilities.FirstOrDefault(f => f.Id == reservation.FacilityId);

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,Notes")] Reservation edited)
        {
            if (id != edited.Id)
                return NotFound();

            // 1. Wczytujemy istniejącą rezerwację z bazy
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            // 2. Aktualizujemy TYLKO dozwolone pola
            reservation.StartTime = edited.StartTime;
            reservation.EndTime = edited.EndTime;
            reservation.Notes = edited.Notes;

            // 3. Walidacja czasu
            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError(string.Empty, "Czas rozpoczęcia musi być wcześniejszy niż czas zakończenia.");
            }

            // 4. Sprawdzenie konfliktu rezerwacji
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

            if (!ModelState.IsValid)
            {
                // jeśli wracasz do widoku, a on korzysta z dropdownów:
                var role = HttpContext.Session.GetString("UserRole");
                if (role == "Admin")
                    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", reservation.UserId);
                ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);

                return View(reservation);
            }

            // 5. Zapisujemy – FK nie zostały dotknięte, więc nie wybucha
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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