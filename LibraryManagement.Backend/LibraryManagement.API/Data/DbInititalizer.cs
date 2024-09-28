using LibraryManagement.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.API.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();

            if (context.Books.Any() || context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Create roles
            await CreateRoles(roleManager);

            // Create users
            await CreateUsers(userManager);

            // Create books
            CreateBooks(context);

            await context.SaveChangesAsync();
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CreateUsers(UserManager<ApplicationUser> userManager)
        {
            var users = new List<(ApplicationUser User, string Password, string Role)>
            {
                (new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com", Name = "Admin User" }, "AdminPass123!", "Admin"),
                (new ApplicationUser { UserName = "user1@example.com", Email = "user1@example.com", Name = "User One" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user2@example.com", Email = "user2@example.com", Name = "User Two" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user3@example.com", Email = "user3@example.com", Name = "User Three" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user4@example.com", Email = "user4@example.com", Name = "User Four" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user5@example.com", Email = "user5@example.com", Name = "User Five" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user6@example.com", Email = "user6@example.com", Name = "User Six" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user7@example.com", Email = "user7@example.com", Name = "User Seven" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user8@example.com", Email = "user8@example.com", Name = "User Eight" }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user9@example.com", Email = "user9@example.com", Name = "User Nine" }, "UserPass123!", "User"),
            };

            foreach (var (user, password, role) in users)
            {
                if (await userManager.FindByEmailAsync(user.Email) == null)
                {
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private static void CreateBooks(ApplicationDbContext context)
        {
            var books = new List<Book>
            {
                new Book { Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "9780446310789", PublicationYear = 1960, Category = "Fiction", IsAvailable = true },
                new Book { Title = "1984", Author = "George Orwell", ISBN = "9780451524935", PublicationYear = 1949, Category = "Science Fiction", IsAvailable = true },
                new Book { Title = "Pride and Prejudice", Author = "Jane Austen", ISBN = "9780141439518", PublicationYear = 1813, Category = "Classic", IsAvailable = true },
                new Book { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "9780743273565", PublicationYear = 1925, Category = "Fiction", IsAvailable = true },
                new Book { Title = "The Catcher in the Rye", Author = "J.D. Salinger", ISBN = "9780316769174", PublicationYear = 1951, Category = "Fiction", IsAvailable = true },
                new Book { Title = "Moby-Dick", Author = "Herman Melville", ISBN = "9780142437247", PublicationYear = 1851, Category = "Classic", IsAvailable = true },
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", ISBN = "9780547928227", PublicationYear = 1937, Category = "Fantasy", IsAvailable = true },
                new Book { Title = "The Da Vinci Code", Author = "Dan Brown", ISBN = "9780307474278", PublicationYear = 2003, Category = "Thriller", IsAvailable = true },
                new Book { Title = "The Hunger Games", Author = "Suzanne Collins", ISBN = "9780439023481", PublicationYear = 2008, Category = "Young Adult", IsAvailable = true },
                new Book { Title = "The Alchemist", Author = "Paulo Coelho", ISBN = "9780062315007", PublicationYear = 1988, Category = "Fiction", IsAvailable = true },
                new Book { Title = "The Girl with the Dragon Tattoo", Author = "Stieg Larsson", ISBN = "9780307454546", PublicationYear = 2005, Category = "Mystery", IsAvailable = true },
                new Book { Title = "The Fault in Our Stars", Author = "John Green", ISBN = "9780142424179", PublicationYear = 2012, Category = "Young Adult", IsAvailable = true },
                new Book { Title = "The Martian", Author = "Andy Weir", ISBN = "9780553418026", PublicationYear = 2011, Category = "Science Fiction", IsAvailable = true },
                new Book { Title = "Gone Girl", Author = "Gillian Flynn", ISBN = "9780307588371", PublicationYear = 2012, Category = "Thriller", IsAvailable = true },
                new Book { Title = "The Help", Author = "Kathryn Stockett", ISBN = "9780425232200", PublicationYear = 2009, Category = "Historical Fiction", IsAvailable = true },
                new Book { Title = "The Kite Runner", Author = "Khaled Hosseini", ISBN = "9781594631931", PublicationYear = 2003, Category = "Fiction", IsAvailable = true },
                new Book { Title = "The Giver", Author = "Lois Lowry", ISBN = "9780544336261", PublicationYear = 1993, Category = "Young Adult", IsAvailable = true },
                new Book { Title = "The Road", Author = "Cormac McCarthy", ISBN = "9780307387899", PublicationYear = 2006, Category = "Post-apocalyptic", IsAvailable = true },
                new Book { Title = "The Curious Incident of the Dog in the Night-Time", Author = "Mark Haddon", ISBN = "9781400032716", PublicationYear = 2003, Category = "Mystery", IsAvailable = true },
                new Book { Title = "The Book Thief", Author = "Markus Zusak", ISBN = "9780375842207", PublicationYear = 2005, Category = "Historical Fiction", IsAvailable = true },
            };

            context.Books.AddRange(books);
        }
    }
}