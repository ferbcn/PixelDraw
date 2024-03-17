using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Data;
using MyWebApplication.Models;
using System.Diagnostics;


namespace MyWebApplication.Controllers
{
    public class HomeController : Controller
    {
        Board myboard = new Board(50);
        
        private readonly ILogger<HomeController> _logger;

        private readonly MyWebApplicationContext _context;

        public HomeController(ILogger<HomeController> logger, MyWebApplicationContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Board"] = myboard.GetBoard();
            // read all Cells in Db and apply to board
            List<Cell> cells = _context.Cell.ToList();
            foreach (var cell in cells)
            {

                myboard.SetCell(cell.Y, cell.X, cell.Color);

            }
            ViewData["Size"] = myboard.GetSize();
            return View("Index");
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