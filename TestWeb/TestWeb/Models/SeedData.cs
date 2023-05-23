using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestWeb.Data;
using System;
using System.Linq;

namespace TestWeb.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TestWebContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<TestWebContext>>()))
            {
                // Look for any movies.
                if (context.User.Any())
                {
                    return;   // DB has been seeded
                }

                context.User.AddRange(
                    new Login
                    {
                        USER_ID = "test1",
                        USER_PWD = "test1"
                    },

                    new Login
                    {
                        USER_ID = "test2",
                        USER_PWD = "tes21"
                    },

                    new Login
                    {
                        USER_ID = "test3",
                        USER_PWD = "test3"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}