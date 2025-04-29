using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class BorrowerRepository : IBorrowerRepository
    {
        public BorrowerRepository()
        {
        }
        public List<Borrower> GetBorrowers()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Borrowers
                    .ToList();
                return list;
            }
        }

        public Guid AddBorrower(Borrower borrower)
        {
            using (var context = new LibraryContext())
            {
                context.Borrowers.Add(borrower);
                context.SaveChanges();
                return borrower.Id;
            }
        }

        public List<(Borrower borrower, List<(Book book, DateTime loanEndDate)>)> GetActiveLoans()
        {
            using (var context = new LibraryContext())
            {
                var groupedLoans = context.Catalogue
                    .Where(bs => bs.OnLoanTo != null && bs.LoanEndDate.HasValue && bs.LoanEndDate > DateTime.UtcNow)
                    .Include(bs => bs.Book)
                    .Include(bs => bs.OnLoanTo)
                    .ToList()
                    .GroupBy(bs => bs.OnLoanTo)
                    .Select(group => (
                        borrower: group.Key,
                        books: group.Select(bs => (bs.Book, bs.LoanEndDate.Value)).ToList()
                    )).ToList();

                return groupedLoans;
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
