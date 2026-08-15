using Microsoft.EntityFrameworkCore;

namespace ResumeEnhancer.Infrastructure.Persistence;

public interface IAppDbContextModelConfiguration
{
    void Configure(ModelBuilder modelBuilder);
}

