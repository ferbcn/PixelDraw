// using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApplication.Models
{
    public class Cell
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; }
        
        [ForeignKey("Board")]
        public int BoardId { get; set; }  // Nullable foreign key
    
        public Board Board { get; set; }
    }
    
}
