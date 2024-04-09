﻿using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Data;
using MyWebApplication.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;


namespace MyWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly MyWebApplicationContext _context;

        public HomeController(ILogger<HomeController> logger, MyWebApplicationContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var allBoards = await _context.Board.ToListAsync();
            Random random = new Random();
            int randomIndex = random.Next(0, allBoards.Count);
            var randomBoardId = allBoards[randomIndex].Id;

            return RedirectToAction("Load", "Board", new { id = randomBoardId });
        }
        
        public async Task<IActionResult> Automata(int? size=50)
        {
            ViewData["Size"] = size.Value;
            ViewData["Title"] = "Cellular Automata";
            return View();
        }
        
        public async Task<IActionResult> GetBoard(int? size=50)
        {
            Board_DTO myboard = new Board_DTO(size.Value);
            ViewData["Board"] = myboard.GetBoardString();
            ViewData["Size"] = myboard.GetSize();
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}