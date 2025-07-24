using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelCrud.Models;

namespace HotelCrud.Controllers
{
    public class PersonaRolsController : Controller
    {
        private readonly HotelCaliforniaDbContext _context;

        public PersonaRolsController(HotelCaliforniaDbContext context)
        {
            _context = context;
        }

        // GET: PersonaRols
        public async Task<IActionResult> Index()
        {
            var hotelCaliforniaDbContext = _context.PersonaRols
                .Include(p => p.IdPersonaNavigation)
                .Include(p => p.IdRolNavigation);
            return View(await hotelCaliforniaDbContext.ToListAsync());
        }

        // GET: PersonaRols/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var personaRol = await _context.PersonaRols
                .Include(p => p.IdPersonaNavigation)
                .Include(p => p.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personaRol == null) return NotFound();

            return View(personaRol);
        }

        // GET: PersonaRols/Create
        public IActionResult Create()
        {
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre");
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Nombre");
            return View();
        }

        // POST: PersonaRols/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdPersona,IdRol")] PersonaRol personaRol)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personaRol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", personaRol.IdPersona);
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Nombre", personaRol.IdRol);
            return View(personaRol);
        }

        // GET: PersonaRols/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var personaRol = await _context.PersonaRols.FindAsync(id);
            if (personaRol == null) return NotFound();

            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", personaRol.IdPersona);
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Nombre", personaRol.IdRol);
            return View(personaRol);
        }

        // POST: PersonaRols/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPersona,IdRol")] PersonaRol personaRol)
        {
            if (id != personaRol.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personaRol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaRolExists(personaRol.Id))
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
            ViewData["IdPersona"] = new SelectList(_context.Personas, "Id", "Nombre", personaRol.IdPersona);
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Nombre", personaRol.IdRol);
            return View(personaRol);
        }

        // GET: PersonaRols/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var personaRol = await _context.PersonaRols
                .Include(p => p.IdPersonaNavigation)
                .Include(p => p.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personaRol == null) return NotFound();

            return View(personaRol);
        }

        // POST: PersonaRols/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personaRol = await _context.PersonaRols.FindAsync(id);
            if (personaRol != null)
            {
                _context.PersonaRols.Remove(personaRol);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaRolExists(int id)
        {
            return _context.PersonaRols.Any(e => e.Id == id);
        }
    }
}
