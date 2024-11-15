using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;


namespace CoalMineApi.Entities
{
    [Table("Emissions", Schema = "CoalMine")]
    public class Emission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public float? Ch4 { get; set; }
        public float? C2H6 { get; set; }
        [Required]
        public Point Point { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
    }

    [Table("Coverages", Schema = "CoalMine")]
    public class Coverage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Polygon Geometry { get; set; }
    }
}