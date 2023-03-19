using Microsoft.EntityFrameworkCore;
using PikaServer.Domain.Entities;
using PikaServer.Persistence.Context;
using PikaServer.Persistence.Internal.Abstracts;
using PikaServer.Persistence.Internal.Data;

namespace PikaServer.Persistence.Repositories;

public interface IContactRepository : IRepositoryBase<Contact>
{
    public Task<List<Contact>> GetContactsByAccountIdAsync(int id);
}

public class ContactRepository : RepositoryBase<Contact, AppDbContext>, IContactRepository 
{
    public ContactRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Contact>> GetContactsByAccountIdAsync(int id)
    {
        var contacts = await DbContext.Contacts.Where(c => c.AccountId == id).ToListAsync();
        return contacts;
    }
}