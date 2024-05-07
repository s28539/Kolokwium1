namespace Kolokwium1.Models;

public class Book
{
     public int id { get; set; }
     public string title { get; set; } 
     public List<Author> authors{ get; set; }
}