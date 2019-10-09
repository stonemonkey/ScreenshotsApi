using Microsoft.EntityFrameworkCore;

public class SaaSContext : DbContext
{
    public SaaSContext(DbContextOptions<SaaSContext> options)
        : base(options) { }

    public DbSet<Screenshot> Screenshots { get; set; }    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Screenshot>()
                .HasKey(c => c.Url);
        modelBuilder.Entity<Screenshot>()
                .Property(b => b.Bytes)
                .IsRequired();
    }
}