using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace HackerPlatform.Infrastructure.Extinitons
{
    //public class ECommerceDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    //{
    //    public AppDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

    //        // ضع هنا Connection String الخاص بك (نفس الموجود في appsettings.json)
    //        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=HackerPlatformDb;Trusted_Connection=True;");

    //        return new AppDbContext(optionsBuilder.Options);
    //    }
    //}
}
