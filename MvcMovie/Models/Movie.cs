using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models;

public class MvcMovie
{

public interface Id {Get; set; }

public string? Title { get; set; }
[DataType(DataType.Date)]

public DateTime ReleassDate { get; set; }

public string? Genre { get; set; }

public decimal Price { get; set; }

}