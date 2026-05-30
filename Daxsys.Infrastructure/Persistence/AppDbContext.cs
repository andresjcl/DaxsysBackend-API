using Daxsys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daxsys.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<PointOfSale> PointsOfSale => Set<PointOfSale>();
    public DbSet<CompanyParameter> CompanyParameters => Set<CompanyParameter>();
    public DbSet<CompanyDatabase> CompanyDatabases => Set<CompanyDatabase>();
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();
    public DbSet<UserWarehouse> UserWarehouses => Set<UserWarehouse>();
    public DbSet<UserPointOfSale> UserPointsOfSales => Set<UserPointOfSale>();
    public DbSet<UserAccess> UserAccesses => Set<UserAccess>();
    public DbSet<UserDocument> UserDocuments => Set<UserDocument>();
    public DbSet<UserDocumentAccess> UserDocumentAccesses => Set<UserDocumentAccess>();
    public DbSet<Menu> Menus => Set<Menu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
