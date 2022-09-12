﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ServicingSystem.Entities;

namespace ServicingSystem.DAL
{
    internal partial class ServicingDbContext : DbContext
    {
        public ServicingDbContext()
        {
        }

        public ServicingDbContext(DbContextOptions<ServicingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerVehicle> CustomerVehicles { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobDetail> JobDetails { get; set; }
        public virtual DbSet<JobDetailPart> JobDetailParts { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<StandardJob> StandardJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.ContactPhone).IsFixedLength();

                entity.Property(e => e.PostalCode).IsFixedLength();

                entity.Property(e => e.Province).IsFixedLength();
            });

            modelBuilder.Entity<CustomerVehicle>(entity =>
            {
                entity.HasKey(e => e.VehicleIdentification)
                    .HasName("PK_CustomerVehicles_VehicleIdentification");

                entity.Property(e => e.VehicleIdentification).IsFixedLength();

                entity.Property(e => e.Make).IsFixedLength();

                entity.Property(e => e.Model).IsFixedLength();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerVehicles)
                    .HasForeignKey(d => d.CustomerID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerVehiclesCustomers_CustomerID");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.PostalCode).IsFixedLength();

                entity.Property(e => e.Province).IsFixedLength();

                entity.Property(e => e.SocialInsuranceNumber).IsFixedLength();
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.VehicleIdentification).IsFixedLength();

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.EmployeeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobsEmployees_EmployeeID");

                entity.HasOne(d => d.VehicleIdentificationNavigation)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.VehicleIdentification)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobsCustomerVehicles_VehicleIdentification");
            });

            modelBuilder.Entity<JobDetail>(entity =>
            {
                entity.Property(e => e.StatusCode)
                    .HasDefaultValueSql("('I')")
                    .IsFixedLength();

                entity.HasOne(d => d.Coupon)
                    .WithMany(p => p.JobDetails)
                    .HasForeignKey(d => d.CouponID)
                    .HasConstraintName("FK_JobDetailsCoupons_CouponID");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.JobDetails)
                    .HasForeignKey(d => d.EmployeeID)
                    .HasConstraintName("FK_JobDetailsEmployees_EmployeeID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobDetails)
                    .HasForeignKey(d => d.JobID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobDetailsJobs_JobID");
            });

            modelBuilder.Entity<JobDetailPart>(entity =>
            {
                entity.HasOne(d => d.JobDetail)
                    .WithMany(p => p.JobDetailParts)
                    .HasForeignKey(d => d.JobDetailID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobDetailPartsJobDetails_JobDetailID");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.JobDetailParts)
                    .HasForeignKey(d => d.PartID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobDetailPartsParts_PartID");
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.Property(e => e.Refundable)
                    .HasDefaultValueSql("('Y')")
                    .IsFixedLength();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Parts)
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartsCategories_CategoryID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}