using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class BookRepository : IBookRepository
    {
        public BookRepository()
        {
        }
        public List<Book> GetBooks()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Books
                    .ToList();
                return list;
            }
        }

        public Guid AddBook(Book book)
        {
            using (var context = new LibraryContext())
            {
                context.Books.Add(book);
                context.SaveChanges();
                return book.Id;
            }
        }


        public bool ReturnBook(Guid bookStockId)
        {
            using (var context = new LibraryContext())
            {
                var bookStock = context.Catalogue
                    .Include(bs => bs.OnLoanTo)
                    .FirstOrDefault(bs => bs.Id == bookStockId);

                if (bookStock == null || bookStock.OnLoanTo == null)
                    return false;

                var now = DateTime.UtcNow;

                if (bookStock.LoanEndDate.HasValue && now > bookStock.LoanEndDate.Value)
                {
                    var daysLate = (now - bookStock.LoanEndDate.Value).Days;
                    var fine = new Fine
                    {
                        Id = Guid.NewGuid(),
                        Borrower = bookStock.OnLoanTo,
                        Amount = daysLate * 1.00m,
                        DateIssued = now,
                        Reason = $"Book returned {daysLate} day(s) late."
                    };
                    context.Fines.Add(fine);
                }

                bookStock.OnLoanTo = null;
                bookStock.LoanEndDate = null;

                context.SaveChanges();
                return true;
            }
        }

    }
}
