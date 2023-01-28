using Microsoft.EntityFrameworkCore;
using PetsAPI.Models.Domain;

namespace PetsAPI.Data;

public class PetsDbContext : DbContext
{
    public PetsDbContext(DbContextOptions<PetsDbContext> options) : base(options) { }

    public virtual DbSet<Dog> Dogs { get; set; }
}