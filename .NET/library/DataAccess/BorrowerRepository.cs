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

    }
}
