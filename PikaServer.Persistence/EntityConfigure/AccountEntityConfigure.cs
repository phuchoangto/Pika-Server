using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PikaServer.Domain.Entities;

namespace PikaServer.Persistence.EntityConfigure;

public class AccountEntityConfigure : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasIndex(q => q.Username)
            .IsUnique();

        builder.HasIndex(q => q.Email)
            .IsUnique();
        
        // default for passbook balance is 0
        builder.Property(q => q.PassbookBalance)
            .HasDefaultValue(0);
    }
}