using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace demo.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        using var dbContext = new NutshellContext();
        IEnumerable<Customer> query = dbContext.Customers;

        List<Customer> customers = [];

        foreach (var customer in query)
        {
            customers.Add(customer);
        }

        ViewData["customers"] = customers;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string? delete = Request.Form["delete"];
        string? insert = Request.Form["insert"];
        string? inserting = Request.Form["inserting"];

        if (insert != null)
        {
            ViewData["insert"] = true;
            return Page();
        }

        if (inserting != null)
        {
            using var dbContext = new NutshellContext();
            IEnumerable<Customer> customers = dbContext.Customers;

            Customer customer = new() {
                ID = customers.Count() + 1,
                Name = Request.Form["name"],
                Email = Request.Form["email"]
            };

            dbContext.Customers.Add(customer);
            dbContext.SaveChanges();
        }

        if (delete != null)
        {
            int id = Int32.Parse(delete);
            using var dbContext = new NutshellContext();
            var customer = dbContext.Customers.Where(c => c.ID == id).Single();
            dbContext.Customers.Remove(customer);
            dbContext.SaveChanges();
        }

        return RedirectToPage();
    }
}

public class Customer
{    
    public int ID {get; set;}
    public string? Name {get; set;}

    public string? Email {get; set;}
}

public class NutshellContext : DbContext
{
    public virtual DbSet<Customer> Customers {get; set;}

    protected override void OnConfiguring (DbContextOptionsBuilder builder)
        => builder.UseSqlServer("Server=localhost;Database=Practise;Trusted_connection=false;TrustServerCertificate=True;User Id=sa;Password=Password4;");

    protected override void OnModelCreating (ModelBuilder modelBuilder)
        => modelBuilder.Entity<Customer>().ToTable("Customer")
                                        .HasKey(c => c.ID);
}