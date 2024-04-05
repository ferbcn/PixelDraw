using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Data;
using MyWebApplication.Models;
using ImageConverter = MyWebApplication.Models.ImageConverter;

namespace MyWebApplication.Controllers
{
    public class BoardController : Controller
    {
        const int DEFAULT_BOARD_SIZE = 50;
        
        private readonly MyWebApplicationContext _context;
        
        public BoardController(MyWebApplicationContext context)
        {
            _context = context;
        }

        // GET: Boards
        public async Task<IActionResult> Index()
        {
            if (_context.Board == null)
            {
                return Problem("Entity set 'MyWebApplicationContext.Board'  is null.");
            }
            else
            {
                
                List<Board> boards = await _context.Board.OrderBy(b => b.Id).ToListAsync(); // Order by ID here
                boards.Reverse();
                // Fetch all cells for each board and include in View
                //var boardCellList = new List<List<Cell>>();
                var boardIds = boards.Select(b => b.Id).ToList();
                
                // Get cells for all board IDs
                var cellList = _context.Cell
                    .Where(c => boardIds.Contains(c.BoardId))
                    .OrderBy(c => c.BoardId)
                    .ToList();

                // Dictionary of boardId and corresponding cells
                var cellDictionary = cellList
                    .GroupBy(c => c.BoardId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Ensure there is a list (even if it's empty)
                var boardCellList = boardIds
                    .Select(id => cellDictionary.TryGetValue(id, out var cells) ? cells : new List<Cell>())
                    .ToList();
                
                // convert cells tom images
                List<string> b64ImageList = new List<string>();
                //foreach (var boardCells in boardCellList)
                //{ 
                for(int i = 0; i < boardCellList.Count; i++)
                {
                    int size = (int) boards[i].Size;
                    b64ImageList.Add(ImageConverter.ConvertCellsToBase64Image(boardCellList[i], size));
                }
                // Include the base64 image strings in the ViewData
                ViewData["BoardImages"] = b64ImageList;
                
                BoardImageCellsViewModel boardImageCellsViewModel = new BoardImageCellsViewModel();
                boardImageCellsViewModel.Boards = boards;
                boardImageCellsViewModel.Cells = _context.Cell.ToList();
                boardImageCellsViewModel.b64Images = b64ImageList;
                
                ViewData["Title"] = "Saved Boards";
                return View(boardImageCellsViewModel);
            }
        }

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context == null)
            {
                return NotFound();
            }

            var Board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Board == null)
            {
                return NotFound();
            }

            return View(Board);
        }
        
        // GET: Board/New
        public async Task<IActionResult>New()
        {
            // Create new board in DB
            Board newBoard = new Board();
            newBoard.Size = DEFAULT_BOARD_SIZE;
            newBoard.Name = "NewBoard";
            _context.Add(newBoard);
            await _context.SaveChangesAsync();
            // Redirect to Load View
            return RedirectToAction("Load", "Board", new { id = newBoard.Id });
        }

        // GET: Boards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Size")] Board Board)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(Board);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            return View(Board);
        }

        // GET: Boards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var Board = await _context.Board.FindAsync(id);
            if (Board == null)
            {
                return NotFound();
            }
            return View(Board);
        }

        // GET: Boards/Load/5
        public async Task<IActionResult> Load(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var Board = await _context.Board.FindAsync(id);
            int board_size = 50;
            if (Board.Size != null)
            {
                board_size = (int) Board.Size;
            }
            if (Board == null)
            {
                return NotFound();
            }
            
            // read only Cells from provided BoardId in Db and apply to board
            // List<Cell> cells = _context.Cell.ToList();
            List<Cell> cells = _context.Cell.Where(c => c.BoardId == Board.Id).ToList();
            Board_DTO myboard = new Board_DTO(board_size);
            
            foreach (var cell in cells)
            {
                myboard.SetCell(cell.X, cell.Y, cell.Color);
            }
            
            ViewData["Board"] = myboard.GetBoardString();
            ViewData["Size"] = myboard.GetSize();
            ViewData["BoardId"] = Board.Id;
            ViewData["BoardName"] = Board.Name;
            ViewData["Title"] = Board.Name;
            
            return View();
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name")] Board Board)
        {
            if (id != Board.Id)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
                Console.WriteLine("INVALID MODEL STATE !!!");
                Console.WriteLine(Board.Id);
                //Console.WriteLine(errors);
            }

            try
            {
                _context.Update(Board);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(Board.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return RedirectToAction(nameof(Index));
            //return View(Board);
        }

        // GET: Boards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Board == null)
            {
                return NotFound();
            }

            var Board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Board == null)
            {
                return NotFound();
            }

            return View(Board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Board == null)
            {
                return Problem("Entity set 'MyWebApplicationContext.Board'  is null.");
            }
            var Board = await _context.Board.FindAsync(id);
            if (Board != null)
            {
                _context.Board.Remove(Board);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
      
        //[HttpGet, ActionName("Reset")]
        public async Task<IActionResult> Reset(int id)
        {
            List<Cell> cells = _context.Cell.Where(c => c.BoardId == id).ToList();

            _context.Cell.RemoveRange(cells);
            await _context.SaveChangesAsync();
            // return RedirectToAction("Index", "Home");
            return RedirectToAction("Load", "Board", new { id = id });
        }

        private bool BoardExists(int id)
        {
          return (_context.Board?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
