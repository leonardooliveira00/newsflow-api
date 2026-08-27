using Microsoft.EntityFrameworkCore;

namespace NewsflowApi.Data;

public class NewsflowDbContext : DbContext
{
    public NewsflowDbContext(DbContextOptions<NewsflowDbContext> options) : base(options)
    { }
}