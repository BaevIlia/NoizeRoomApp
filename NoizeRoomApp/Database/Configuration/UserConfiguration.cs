using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Database.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.Bookings)
                .WithOne(b => b.User);

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            builder.HasOne(u => u.Notify)
            .WithMany(n => n.Users)
            .HasForeignKey(u => u.NotifyTypeId);
        }
    }
}
