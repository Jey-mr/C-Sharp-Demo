using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace demo.Pages;

public class CustomerModel : PageModel
{
    private readonly ILogger<CustomerModel> _logger;

    public CustomerModel(ILogger<CustomerModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string sort)
    {
        if (sort == null)
        {
            ViewData["customers"] = GetCustomers();
        }
        else 
        {
            ViewData["customers"] = GetSortedCustomers(sort);
        }
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
        string? edit = Request.Form["edit"];
        string? editing = Request.Form["editing"];

        if (editing != null)
        {
            int id = Int32.Parse(Request.Form["id"]);
            string? name = Request.Form["name"];
            string? email = Request.Form["email"];
            using var dbContext = new NutshellContext();
            Customer customer = dbContext.Customers.Where(c => c.ID == id).Single();

            if (name != null)
            {
                customer.Name = name;
            }
            
            if (email != null)
            {
                customer.Email = email;
            }

            dbContext.Customers.Update(customer);
            dbContext.SaveChanges();
        }

        if (edit != null)
        {
            ViewData["customers"] = GetCustomers();
            ViewData["CustomerID"] = Int32.Parse(edit);
            return Page();
        }

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

    private List<Customer> GetCustomers()
    {
        using var dbContext = new NutshellContext();
        IEnumerable<Customer> query = dbContext.Customers;
        List<Customer> customers = [];

        foreach (var customer in query)
        {
            customers.Add(customer);
        }

        return customers;
    }

    private List<Customer> GetSortedCustomers(string sort)
    {
        using var dbContext = new NutshellContext();
        IEnumerable<Customer> query;
        List<Customer> customers = [];

        if (sort.Equals("ID"))
        {
            query = dbContext.Customers.OrderBy(c => c.ID);
        }
        else if (sort.Equals("Name"))
        {
            query = dbContext.Customers.OrderBy(c => c.Name);
        }
        else
        {
            query = dbContext.Customers.OrderBy(c => c.Email);
        }

        foreach (var customer in query)
        {
            customers.Add(customer);
        }

        return customers;
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