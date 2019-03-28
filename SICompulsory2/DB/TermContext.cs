using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SICompulsory2.Models;
namespace SICompulsory2.DB
{
    public class TermContext : DbContext
    {
        public TermContext(DbContextOptions<TermContext> options) : base(options) { }

        public DbSet<Term> Terms { get; set; }
        public DbSet<SpecialCharacters> SpecialCharacters {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Term>()
                .Property(b => b.Text)
                .IsRequired();
            modelBuilder.Entity<Term>()
                .HasKey(x => x.Text);
            modelBuilder.Entity<SpecialCharacters>()
                .HasKey(x => new {x.Acronym,x.SpecialCharacter })  ;
        }
    }
}
