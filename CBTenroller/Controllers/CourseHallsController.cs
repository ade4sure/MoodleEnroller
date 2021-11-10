using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CBTenroller.Data;
using CBTenroller.Models;
using Microsoft.AspNetCore.Authorization;

namespace CBTenroller.Controllers
{
    [Authorize]
    public class CourseHallsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseHallsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseHalls
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CourseHalls
                .Include(c => c.Course)
                .Include(c => c.Hall)
                .OrderBy(o=>o.Id);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CourseHalls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseHall = await _context.CourseHalls
                .Include(c => c.Course)
                .Include(c => c.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseHall == null)
            {
                return NotFound();
            }

            return View(courseHall);
        }

        // GET: CourseHalls/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code");
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name");
            return View();
        }

        // POST: CourseHalls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,HallId,StartTime,StopTime,EnforceBatch,StartNumber,LastNumber")] CourseHall courseHall)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseHall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", courseHall.CourseId);
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", courseHall.HallId);
            return View(courseHall);
        }

        // GET: CourseHalls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseHall = await _context.CourseHalls.FindAsync(id);
            if (courseHall == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", courseHall.CourseId);
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", courseHall.HallId);
            return View(courseHall);
        }

        // POST: CourseHalls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,HallId,StartTime,StopTime,EnforceBatch,StartNumber,LastNumber")] CourseHall courseHall)
        {
            if (id != courseHall.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseHall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseHallExists(courseHall.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", courseHall.CourseId);
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", courseHall.HallId);
            return View(courseHall);
        }

        // GET: CourseHalls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseHall = await _context.CourseHalls
                .Include(c => c.Course)
                .Include(c => c.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseHall == null)
            {
                return NotFound();
            }

            return View(courseHall);
        }

        // POST: CourseHalls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseHall = await _context.CourseHalls.FindAsync(id);
            _context.CourseHalls.Remove(courseHall);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseHallExists(int id)
        {
            return _context.CourseHalls.Any(e => e.Id == id);
        }
    }
}
