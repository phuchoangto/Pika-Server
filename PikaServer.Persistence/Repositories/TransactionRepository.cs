using Microsoft.EntityFrameworkCore;
using PikaServer.Domain.Entities;
using PikaServer.Persistence.Context;
using PikaServer.Persistence.Internal.Abstracts;
using PikaServer.Persistence.Internal.Data;

namespace PikaServer.Persistence.Repositories;

public interface ITransactionRepository : IRepositoryBase<Transaction>
{
    public Task<List<Transaction>> GetByAccountNoAsync(string accountNo);
}

public class TransactionRepository : RepositoryBase<Transaction, AppDbContext>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Transaction>> GetByAccountNoAsync(string accountNo)
    {
        var transactions = await DbContext.Transactions.Where(x => x.FromAccountNo == accountNo || x.ToAccountNo == accountNo).OrderByDescending(x => x.CreatedAt).ToListAsync();
        return transactions;
    }
}