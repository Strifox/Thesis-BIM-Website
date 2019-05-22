﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Thesis_BIM_Website.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Invoice>(inv =>
            {
                inv.Property(x => x.AmountToPay).HasColumnType("decimal(15, 2)");
                inv.Property(x => x.CompanyName).HasMaxLength(50);
                inv.Property(x => x.BankAccountNumber).HasMaxLength(16);
            });

            builder.Entity<User>(user =>
            {
                user.Property(x => x.UserName);
                user.Property(x => x.PasswordHash);
                user.Property(x => x.Email);
            });
        }
    }
}