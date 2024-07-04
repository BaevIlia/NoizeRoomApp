using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoizeRoomApp.Database.Models;

namespace NoizeRoomApp.Database.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.HasMany(r => r.Users)
                .WithOne(u => u.Role);


        }
    }
}
