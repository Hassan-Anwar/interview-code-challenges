using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using System.Collections;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private readonly IBookRepository _bookRepository;

        public BookController(ILogger<BookController> logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;   
        }

        [HttpGet]
        [Route("GetBooks")]
        public IList<Book> Get()
        {
            return _bookRepository.GetBooks();
        }

        [HttpPost]
        [Route("AddBook")]
        public Guid Post(Book book)
        {
            return _bookRepository.AddBook(book);
        }

        [HttpPost]
        [Route("ReturnBook/{bookStockId}")]
        public ActionResult ReturnBook(Guid bookStockId)
        {
            var success = _bookRepository.ReturnBook(bookStockId);
            if (!success)
                return NotFound("Book not found or not on loan.");

            return Ok("Book returned successfully.");
        }


        [HttpPost]
        [Route("ReturnBook")]
        public ActionResult ReturnBook([FromQuery] Guid bookStockId)
        {
            var result = _bookRepository.ReturnBook(bookStockId);
            if (!result)
                return BadRequest("Could not return book (not found or already returned).");

            return Ok("Book returned successfully.");
        }

        [HttpPost]
        [Route("LoanBook")]
        public ActionResult LoanBook([FromQuery] Guid bookStockId, [FromQuery] Guid borrowerId, [FromQuery] DateTime loanEndDate)
        {
            var result = _bookRepository.LoanBook(bookStockId, borrowerId, loanEndDate);
            if (!result)
                return BadRequest("Could not loan book (may already be on loan or borrower/book not found).");

            return Ok("Book loaned successfully.");
        }

    }
}