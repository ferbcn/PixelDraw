using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Data;
using MyWebApplication.Models;
using System;
using System.Drawing;

namespace MyWebApplication.Controllers
{
    public class ClickController : Controller
    {
        
        private String[] allColors = { "#cc2222", "#006400", "#00008b" };  // red, green, blue

        private Random random = new();

        private readonly MyWebApplicationContext _context;

        public ClickController(MyWebApplicationContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Cell(int i, int j)
        {
            string newColor = allColors[random.Next(0, 3)];
            Cell? currentCell = await _context.Cell.FirstOrDefaultAsync(c => c.X == j && c.Y == i);

            if (currentCell != null)
            {
                // Update Cell
                System.Diagnostics.Debug.WriteLine($"Cell ({j},{i}) currently has color color: {currentCell.Color}");
                currentCell.Color = newColor;
                _context.Update(currentCell);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"Cell ({j},{i}) UPDATED with color: {newColor}");

            }
            else
            {
                // Create Cell
                Cell newCell = new() { X = j, Y = i, Color = newColor };
                _context.Add(newCell);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"Cell ({j},{i}) ADDED with color: {newColor}");
            }

            ViewData["i"] = i;
            ViewData["j"] = j;
            ViewData["color"] = newColor;
            return View("Cell");
        }

        public IActionResult Reset()
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM Cell");
            return RedirectToAction("Index", "Home");
        }

    }
}
