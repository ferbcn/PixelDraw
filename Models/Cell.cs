using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;

namespace MyWebApplication.Models
{
    public class Cell
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; }
    }
    
}
