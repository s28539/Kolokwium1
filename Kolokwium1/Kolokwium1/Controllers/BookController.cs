using System.Transactions;
using Kolokwium1.Models;
using Kolokwium1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1.Controllers;

[Route("api/[controller]")]
[ApiController]

public class BookController : ControllerBase
{
    private IRepository _repository;
    public BookController(IRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet("{id}/authors")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await _repository.GetBook(id);
        return Ok(book);
    }
    
    [HttpPost("/books")]
    public async Task<IActionResult> editBook(Book book)
    {
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repository.AddNewBook(book);
            foreach (var author in book.authors)
            {
                await _repository.AddNewAuthor(author);
                await _repository.asociateBookAndAuthor(author, book);
            }
            

            
            scope.Complete();
        }
        return  Created(Request.Path.Value ?? "api/books", book);
    }
    
}