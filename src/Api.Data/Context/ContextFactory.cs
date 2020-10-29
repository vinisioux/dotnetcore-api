using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Data.Context
{
  public class ContextFactory : IDesignTimeDbContextFactory<MyContext>
  {
    public MyContext CreateDbContext(string[] args)
    {
      // This class is used to create migrations
      var connectionString = "Server=localhost;Port=5432;Database=dbapi;Uid=postgres;Pwd=docker";
      var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
      optionsBuilder.UseNpgsql(connectionString);
      return new MyContext(optionsBuilder.Options);
    }
  }
}
