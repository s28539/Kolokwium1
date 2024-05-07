using Kolokwium1.Models;

namespace Kolokwium1.Repositories;

public interface IRepository
{
    Task<Book> GetBook(int bookID);
    Task<List<Author>> GetAuthors(int bookID);
    Task<Book> AddNewBook(Book book);
    Task<Author> AddNewAuthor(Author author);
    Task asociateBookAndAuthor(Author author, Book book);
    
}