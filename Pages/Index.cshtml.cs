using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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


        @ViewData["string"] = "Hello World";
        @ViewData["customers"] = customers;
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