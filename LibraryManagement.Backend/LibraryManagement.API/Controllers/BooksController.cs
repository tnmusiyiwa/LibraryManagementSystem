using LibraryManagement.API.Attributes;
using LibraryManagement.API.Models;
using LibraryManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    // [ApiVersion("1.0")]
    // [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("paginated")]
        [AllowAnonymous]
        [SwaggerOperation("GetAllPaginatedBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(PaginatedList<Book>), description: "Get all paginated books filtered according to search string")]

        public async Task<IActionResult> GetAllPaginatedBooks([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.GetPaginatedBooksAsync(pageIndex, pageSize);
            return Ok(books);
        }

        [HttpGet]
        [AllowAnonymous]
        [ValidateModelState]
        [SwaggerOperation("GetBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<Book>), description: "Get paginated books filtered according to search string")]

        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = "")
        {
            var books = await _bookService.GetAllBooksAsync(page, pageSize, searchQuery);
            var totalBooks = await _bookService.GetTotalBooksCountAsync(searchQuery);

            return Ok(new
            {
                Books = books,
                TotalBooks = totalBooks,
                CurrentPage = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ValidateModelState]
        [SwaggerOperation("GetBook")]
        [SwaggerResponse(statusCode: 200, type: typeof(Book), description: "Get book by id")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateModelState]
        [SwaggerOperation("CreateBook")]
        [SwaggerResponse(statusCode: 200, type: typeof(Book), description: "Create a new book (admin only)")]

        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            var createdBook = await _bookService.AddBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ValidateModelState]
        [SwaggerOperation("UpdateBook")]
        [SwaggerResponse(statusCode: 200, type: typeof(void), description: "Update Book Details (admin only)")]

        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            try
            {
                await _bookService.UpdateBookAsync(book);
            }
            catch (Exception)
            {
                if (await _bookService.GetBookByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ValidateModelState]
        [SwaggerOperation("DeleteBook")]
        [SwaggerResponse(statusCode: 200, type: typeof(void), description: "Delete Book (admin only)")]

        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            await _bookService.DeleteBookAsync(id);

            return NoContent();
        }

        [HttpGet("search")]
        [AllowAnonymous]
        [SwaggerOperation("SearchBooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(IEnumerable<Book>), description: "Search book by author or title")]
        public async Task<IActionResult> SearchBooks([FromQuery] string searchTerm)
        {
            var books = await _bookService.SearchBooksAsync(searchTerm);
            return Ok(books);
        }
    }
}