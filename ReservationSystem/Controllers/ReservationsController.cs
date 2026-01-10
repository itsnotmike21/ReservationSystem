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
                .Include(r => r.User); // admin zobaczy dane usera

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
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FacilityId,StartTime,EndTime,Notes")] Reservation reservation)
        {
            reservation.UserId = HttpContext.Session.GetInt32("UserId").Value;

            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError(string.Empty, "Czas rozpoczęcia musi być wcześniejszy niż czas zakończenia.");
            }

            // Sprawdzenie konfliktu rezerwacji dla tego samego obiektu
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            // Sprawdzenie konfliktu rezerwacji przy edycji
            bool hasConflict = await _context.Reservations
                .AnyAsync(r =>
                    r.Id != reservation.Id &&                  // pomijamy tę samą rezerwację
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

            // jeśli są błędy, trzeba na nowo załadować dropdowny
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
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

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

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
