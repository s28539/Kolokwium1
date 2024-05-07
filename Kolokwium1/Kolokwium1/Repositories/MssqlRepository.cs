using Kolokwium1.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace Kolokwium1.Repositories;

public class MssqlRepository : IRepository
{
    private  IConfiguration _configuration;

    public MssqlRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Book> GetBook(int bookID)
    {
        var query = @"select PK AS bookID,title AS bookTitle from books where PK = @ID";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", bookID);
	    
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        Book book = new Book();
        var bookIDOrdinal = reader.GetOrdinal("bookID");
        var bookTitleOrdinal = reader.GetOrdinal("bookTitle");
        while (await reader.ReadAsync())
        {
            book = new Book()
            {
                id = reader.GetInt32(bookIDOrdinal),
                title = reader.GetString(bookTitleOrdinal),
                authors = await GetAuthors(bookID)
            };
        }

        return book;
    }

    public async Task<List<Author>> GetAuthors(int bookID)
    {
        var query = @"select first_name,last_name from books
                        inner join books_authors on books_authors.FK_book = books.PK
                        inner join authors on authors.PK = books_authors.FK_author
                        where books.PK = @ID;";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", bookID);
	    
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        List<Author> authors = new List<Author>();
        
        var lastNameOrdinal = reader.GetOrdinal("last_name");
        var firstNameOrdinal = reader.GetOrdinal("first_name");
        while (await reader.ReadAsync())
        {
            authors.Add(new Author()
            {
                firstName = reader.GetString(firstNameOrdinal),
                lastName = reader.GetString(lastNameOrdinal)
            });
        }

        return authors;
    }
    
    public async Task<Book> AddNewBook(Book book)
    {
        var insert = @"INSERT INTO book  VALUES(@title);";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;
	    
        command.Parameters.AddWithValue("@title", book.title);

        await connection.OpenAsync();
	    
        var id = await command.ExecuteScalarAsync();
        
        return book;
    }

    public async Task<Author> AddNewAuthor(Author author)
    {
        var insert = @"INSERT INTO authors  VALUES(@firstName, @lastName);";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;
	    
        command.Parameters.AddWithValue("@firstName", author.firstName);
        command.Parameters.AddWithValue("@lastName", author.lastName);

        await connection.OpenAsync();
	    
        var id = await command.ExecuteScalarAsync();
        
        return author;
    }

    public async Task asociateBookAndAuthor(Author author, Book book)
    {
        var insert = @"INSERT INTO books_authors VALUES(@PK_BOOK, @FK_AUTHOR);";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = insert;

        command.Parameters.AddWithValue("@PK_BOOK", book.id);
        command.Parameters.AddWithValue("@FK_AUTHOR", author.id);

        await connection.OpenAsync();

        var id = await command.ExecuteScalarAsync();
        
    }
}