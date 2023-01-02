using Microsoft.EntityFrameworkCore;
using Shared.Data.Models;

namespace Shared.Data.Contexts
{
    public class ApplicationDbContext : dmsdevdbContext
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<dmsdevdbContext> options)
            : base(options)
        {
        }
    }
}
