using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Database.Configuration
{
    public class NotifyConfiguration : IEntityTypeConfiguration<NotifyEntity>
    {
        public void Configure(EntityTypeBuilder<NotifyEntity> builder)
        {
            builder.HasMany(n => n.Users)
                .WithOne(u => u.Notify);

            builder.HasData(
                new NotifyEntity {Id=1, Name = "noNotify" },
                new NotifyEntity {Id =2, Name = "emailNotify" },
                new NotifyEntity {Id = 3, Name = "vkNotify" }
                );
        }
    }
}
