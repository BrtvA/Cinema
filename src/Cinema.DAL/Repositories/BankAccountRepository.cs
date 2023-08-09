using Cinema.DAL.Entities;
using Cinema.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DAL.Repositories;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly ApplicationContext _db;

    public BankAccountRepository(ApplicationContext applicationContext)
    {
        _db = applicationContext;
    }

    public async Task<BankAccount?> GetAsync(string cardNumber)
    {
        return await _db.BankAccounts.FirstOrDefaultAsync(acct => acct.CardNumber == cardNumber);
    }

    public void Update(BankAccount bankAccount)
    {
        _db.BankAccounts.Update(bankAccount);
    }
}
