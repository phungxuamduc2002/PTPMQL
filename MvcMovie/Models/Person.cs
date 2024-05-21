using System.Reflection.Metadata.Ecma335;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMovie.Models

{
    [Table("Persons")]


    public class Persons
    {
        [Key]

        public string PersonId { get; set;}

        public string FullName { get; set;}

        public string Address { get; set;}
    }
}