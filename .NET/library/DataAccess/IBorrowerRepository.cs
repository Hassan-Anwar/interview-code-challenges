using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface IBorrowerRepository
    {
        public List<Borrower> GetBorrowers();

        public Guid AddBorrower(Borrower borrower);

        List<(Borrower borrower, List<(Book book, DateTime loanEndDate)>)> GetActiveLoans();

    }
}
