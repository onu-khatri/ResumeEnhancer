using Microsoft.EntityFrameworkCore;

namespace Persistence;

public interface IAppDbContextModelConfiguration
{
    void Configure(ModelBuilder modelBuilder);
}
