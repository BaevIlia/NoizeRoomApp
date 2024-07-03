using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Configuration
{
    public class NotifyConfiguration : IEntityTypeConfiguration<NotifyEntity>
    {
        public void Configure(EntityTypeBuilder<NotifyEntity> builder)
        {
            builder.HasMany(n => n.Users)
                .WithOne(u => u.Notify);
                
        }
    }
}
