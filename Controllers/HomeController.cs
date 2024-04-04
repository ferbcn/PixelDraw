using Microsoft.AspNetCore.Mvc;
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
            // int totalBoards = _context.Board.Count();
            var allBoards = await _context.Board.ToListAsync();
            Random random = new Random();
            int randomIndex = random.Next(0, allBoards.Count);
            var randomBoardId = allBoards[randomIndex].Id;
            // Redirect to Board/Load?id=@randomBoardId 
            return RedirectToAction("Load", "Board", new { id = randomBoardId });
        }
        
        public async Task<IActionResult> Auto()
        {
            Board_DTO myboard = new Board_DTO(50);
            var col = "#DD3333";var cellsToSet = new List<(int, int)>
            {
                (22,22), (22,23), (22,24), (25,22), (25,23), (25,24), (23,21), (24,21), (23,25), (24,25), 
                (24,20), (24,21), (24,22), (27,20), (27,21), (27,22), (25,19), (26,19), (25,23), (26,23) };

            foreach (var (x, y) in cellsToSet)
            {
                myboard.SetCell(x, y, col);
            }

            ViewData["Board"] = myboard.GetBoardString();
            ViewData["Size"] = myboard.GetSize();
            ViewData["Title"] = "Automata";
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