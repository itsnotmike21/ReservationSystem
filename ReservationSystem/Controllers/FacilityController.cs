using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    public class FacilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Facilities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Facilities.ToListAsync());
        }

        // GET: Facilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var facility = await _context.Facilities.FirstOrDefaultAsync(m => m.Id == id);
            if (facility == null)
                return NotFound();

            return View(facility);
        }

        // GET: Facilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Facilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Facility facility)
        {
            if (facility.ImageFile != null)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(facility.ImageFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await facility.ImageFile.CopyToAsync(stream);
                }

                facility.ImageUrl = "/images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(facility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(facility);
        }

        // GET: Facilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null)
                return NotFound();

            return View(facility);
        }

        // POST: Facilities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Facility facility)
        {
            if (id != facility.Id)
                return NotFound();

            var existing = await _context.Facilities.FindAsync(id);
            if (existing == null)
                return NotFound();

            // aktualizacja pól tekstowych
            existing.Name = facility.Name;
            existing.Description = facility.Description;
            existing.Price = facility.Price;
            existing.Size = facility.Size;
            existing.Occupancy = facility.Occupancy;

            // upload nowego zdjęcia
            if (facility.ImageFile != null)
            {
                string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(facility.ImageFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await facility.ImageFile.CopyToAsync(stream);
                }

                existing.ImageUrl = "/images/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Facilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var facility = await _context.Facilities.FirstOrDefaultAsync(m => m.Id == id);
            if (facility == null)
                return NotFound();

            return View(facility);
        }

        // POST: Facilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility != null)
                _context.Facilities.Remove(facility);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
            return _context.Facilities.Any(e => e.Id == id);
        }
    }
}