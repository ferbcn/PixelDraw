namespace MyWebApplication.Models;

public class BoardImageCellsViewModel
{
    public IEnumerable<Board> Boards { get; set; }
    public IEnumerable<Cell> Cells { get; set; }
    
    public IEnumerable<string> b64Images { get; set; }
    
}
