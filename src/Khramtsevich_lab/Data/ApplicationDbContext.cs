using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Khramtsevich_lab.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Наследуемся от IdentityDbContext с использованием ApplicationUser
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) 
    {
    }
    // Добавляем DbSet для категорий и блюд
    public DbSet<Category> Categories { get; set; }
    public DbSet<Dish> Dishes { get; set; }

// Конфигурация модели Entity Framework Core для установки отношений между сущностями
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Вызов базового метода

        // Конфигурация отношения один-ко-многим
        modelBuilder.Entity<Dish>()
            .HasOne(d => d.Category)
            .WithMany(c => c.Dishes)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
