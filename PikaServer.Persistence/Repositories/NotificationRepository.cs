
using Microsoft.EntityFrameworkCore;
using PikaServer.Domain.Entities;
using PikaServer.Persistence.Context;
using PikaServer.Persistence.Internal.Abstracts;
using PikaServer.Persistence.Internal.Data;

namespace PikaServer.Persistence.Repositories;

public interface INotificationRepository : IRepositoryBase<Notification>
{
    Task<List<Notification>> GetNotificationsByUserIdAsync(string accountNo);
}

public class NotificationRepository : RepositoryBase<Notification, AppDbContext> , INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Notification>> GetNotificationsByUserIdAsync(string accountNo)
    {
        var notifications = await DbContext.Notifications.Where(x => x.AccountId == accountNo).OrderByDescending(x => x.CreatedAt).ToListAsync();
        return notifications;
    }
}
