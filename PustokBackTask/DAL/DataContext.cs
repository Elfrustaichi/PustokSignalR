﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PustokBackTask.Models;

namespace PustokBackTask.DAL
{
    public class DataContext: IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookImage> BookImages { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Slider> Sliders { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<BookTag> BookTags { get; set; }

        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookTag>().HasKey(x => new { x.TagId, x.BookId });

            modelBuilder.Entity<Setting>().HasKey(x => x.Key);

            base.OnModelCreating(modelBuilder);
        }

    }
}
