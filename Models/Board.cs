using System.Runtime.CompilerServices;

namespace MyWebApplication.Models
{
    public class Board
    {

        private String[,] board;

        public Board(int size)
        {
            // 2 dimensional array of points to represent the board 
            board = new String[size, size];
            initBoard(size);
        }

        public void initBoard(int size)
        {
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = "white";
                }
            }
        }

        // get board
        public String[,]? GetBoard()
        {
            return board;
        }

        // get board size
        public int? GetSize()
        {
            return board.GetLength(0);
        }

        // set cell
        public void SetCell(int x, int y, String color)
        {
            board[x, y] = color;
        }

        // get cell
        public String? GetCell(int x, int y)
        {
            return board[x, y];
        }
    }
}
