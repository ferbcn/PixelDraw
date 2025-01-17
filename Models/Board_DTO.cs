﻿
namespace MyWebApplication.Models
{
    public class Board_DTO
    {

        private String[,] board;

        public Board_DTO(int size)
        {
            // 2 dimensional array of points to represent the board 
            board = new String[size, size];
            InitBoard(size);
            //Console.WriteLine("New Board with Size: " + size);
        }
        
        public Board_DTO(int size, string[,] color_data_str)
        {
            // 2 dimensional array of points to represent the board 
            board = new String[size, size];
            InitBoard(size, color_data_str);
            if (color_data_str.GetLength(0) >= size || color_data_str.GetLength(1) >= size)
            {
                //Console.WriteLine("Warning: Board size smaller than data size");
            }
        }

        private void InitBoard(int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    board[i, j] = "#FFFFFF";
                }
            }
        }

        private void InitBoard(int size, string[,] bw_data_str)
        {
            //Console.WriteLine("Length: " + bw_data_str.Length);
            for (int y = 0; y < GetSize(); y++)
            {
                for (int x = 0; x < GetSize(); x++)
                {
                    // var newColor = bw_data_str[y, x] == "1" ? "#DD3333" : "#FFFFFF";
                    SetCell(x, y, bw_data_str[y, x]);
                }
            }	
        }

        // get board
        public String[,]? GetBoardString()
        {
            return board;
        }

        // get board size
        public int GetSize()
        {
            return board.GetLength(0);
        }

        // set cell
        public void SetCell(int x, int y, String color)
        {
            if (x >= 0 && x < GetSize() && y >= 0 && y < GetSize())
            {
                board[y, x] = color;
            }
            else
            {
                //Console.Write("Cells outside of board: " + x + ", " + y + "\n");
            }
        }

        // get cell
        public String? GetCell(int x, int y)
        {
            return board[x, y];
        }
    }
}
