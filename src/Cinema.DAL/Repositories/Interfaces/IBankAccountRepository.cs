using Cinema.DAL.Entities;

namespace Cinema.DAL.Repositories.Interfaces;

public interface IBankAccountRepository
{
    public Task<BankAccount?> GetAsync(string cardNumber);
    public void Update(BankAccount bankAccount);
}
