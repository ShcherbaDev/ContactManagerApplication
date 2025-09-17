using System;
using ContactManagerApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApplication.Data
{
	public class ContactContext : DbContext
	{
		public ContactContext()
		{ }

		public ContactContext(DbContextOptions<ContactContext> options) : base(options)
		{ }

		public DbSet<Contact> Contacts { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer();
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Id
			modelBuilder.Entity<Contact>()
				.HasKey(c => c.Id);

			modelBuilder.Entity<Contact>()
				.Property(c => c.Id)
				.ValueGeneratedOnAdd();

			// Name
			modelBuilder.Entity<Contact>()
				.Property(c => c.Name)
				.HasMaxLength(255)
				.IsRequired();

			// Date of birth
			modelBuilder.Entity<Contact>()
				.Property(c => c.BirthDate)
				.IsRequired();

			// Married
			modelBuilder.Entity<Contact>()
				.Property(c => c.IsMarried)
				.IsRequired();

			// Phone
			modelBuilder.Entity<Contact>()
				.Property(c => c.Phone)
				.HasMaxLength(20)
				.IsRequired();

			// Salary
			modelBuilder.Entity<Contact>()
				.Property(c => c.Salary)
				.HasPrecision(10, 2)
				.IsRequired();

			modelBuilder.Entity<Contact>()
				.ToTable("Contacts")
				.HasCheckConstraint("CH_Contact_Salary_Min", "Salary >= 0");

			// Creation and last update dates
			modelBuilder.Entity<Contact>()
				.Property(c => c.CreatedDate)
				.HasDefaultValue(DateTime.UtcNow);

			modelBuilder.Entity<Contact>()
				.Property(c => c.ModifiedDate)
				.HasDefaultValue(DateTime.UtcNow);
		}
	}
}
