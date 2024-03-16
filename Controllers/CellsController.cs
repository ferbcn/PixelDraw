﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Data;
using MyWebApplication.Models;

namespace MyWebApplication.Controllers
{
    public class CellsController : Controller
    {
        private readonly MyWebApplicationContext _context;

        public CellsController(MyWebApplicationContext context)
        {
            _context = context;
        }

        // GET: Cells
        public async Task<IActionResult> Index()
        {
              return _context.Cell != null ? 
                          View(await _context.Cell.ToListAsync()) :
                          Problem("Entity set 'MyWebApplicationContext.Cell'  is null.");
        }

        // GET: Cells/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cell == null)
            {
                return NotFound();
            }

            var cell = await _context.Cell
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cell == null)
            {
                return NotFound();
            }

            return View(cell);
        }

        // GET: Cells/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cells/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,X,Y,Color")] Cell cell)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cell);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cell);
        }

        // GET: Cells/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cell == null)
            {
                return NotFound();
            }

            var cell = await _context.Cell.FindAsync(id);
            if (cell == null)
            {
                return NotFound();
            }
            return View(cell);
        }

        // POST: Cells/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,X,Y,Color")] Cell cell)
        {
            if (id != cell.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cell);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CellExists(cell.Id))
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
            return View(cell);
        }

        // GET: Cells/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cell == null)
            {
                return NotFound();
            }

            var cell = await _context.Cell
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cell == null)
            {
                return NotFound();
            }

            return View(cell);
        }

        // POST: Cells/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cell == null)
            {
                return Problem("Entity set 'MyWebApplicationContext.Cell'  is null.");
            }
            var cell = await _context.Cell.FindAsync(id);
            if (cell != null)
            {
                _context.Cell.Remove(cell);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CellExists(int id)
        {
          return (_context.Cell?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}