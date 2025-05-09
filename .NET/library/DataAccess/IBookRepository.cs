﻿using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface IBookRepository
    {
        public List<Book> GetBooks();

        public Guid AddBook(Book book);

        bool ReturnBook(Guid bookStockId);

        bool ReturnBook(Guid bookStockId);

        bool LoanBook(Guid bookStockId, Guid borrowerId, DateTime loanEndDate);

    }
}
