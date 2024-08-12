using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;
using Domain.DomainModel.Entity.Lookup;
using Domain.DomainModel.Entity.RolesAndAccess;
using Domain.DomainModel.Helper;
using Domain.DomainModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture
{
    public class DataContext: DbContext, IUnitOfWork
    {
        //private IDbContextTransaction _currentTransaction;

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Stored Procedures
        public static DbSet<SyncOrderResult> SyncOrderResult { get; set; }

        #region Lookups
        public DbSet<CustomerType> CustomerTypes => Set<CustomerType>();
        public DbSet<LocationType> LocationTypes => Set<LocationType>();
        public DbSet<PositionType> PositionTypes => Set<PositionType>();
        public DbSet<PaymentType> PaymentTypes => Set<PaymentType>();
        #endregion

        #region Roles And Accesses
        public DbSet<Access> Accesses => Set<Access>();
        public DbSet<AccessType> AccessTypes => Set<AccessType>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<ModuleGroup> ModuleGroups => Set<ModuleGroup>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RoleAccess> RoleAccesses => Set<RoleAccess>();
        public DbSet<RoleModuleAccess> RoleModuleAccesses => Set<RoleModuleAccess>();
        public DbSet<UserType> UserTypes => Set<UserType>();
        public DbSet<User> Users => Set<User>();
        #endregion

        #region Entities
        public DbSet<Automobile> Automobiles => Set<Automobile>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<CustomerCredit> CustomerCredits => Set<CustomerCredit>();
        public DbSet<CustomerNote> CustomerNotes => Set<CustomerNote>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<DriverLog> DriverLogs => Set<DriverLog>();
        public DbSet<DriverLogDetail> DriverLogDetails => Set<DriverLogDetail>();
        public DbSet<ItemMasterlistReference> ItemMasterlistReferences => Set<ItemMasterlistReference>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<OrderNote> OrderNotes => Set<OrderNote>();
        public DbSet<PartsCatalog> PartsCatalogs => Set<PartsCatalog>();
        public DbSet<PartsPicking> PartsPickings => Set<PartsPicking>();
        public DbSet<PartsPickingDetail> PartsPickingDetails => Set<PartsPickingDetail>();
        public DbSet<PartsManifest> PartsManifests => Set<PartsManifest>();
        public DbSet<PartsManifestDetail> PartsManifestDetails => Set<PartsManifestDetail>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentDetail> PaymentDetails => Set<PaymentDetail>();
        public DbSet<PaymentTerm> PaymentTerms => Set<PaymentTerm>();
        public DbSet<PriceLevel> PriceLevels => Set<PriceLevel>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
        public DbSet<SalesRepresentative> SalesRepresentatives => Set<SalesRepresentative>();
        public DbSet<Sequence> Sequences => Set<Sequence>();
        public DbSet<StockPartsLocation> StockPartsLocations => Set<StockPartsLocation>();
        public DbSet<StockSettings> StockSettings => Set<StockSettings>();
        
        public DbSet<Vendor> Vendors => Set<Vendor>();
        public DbSet<VendorCatalog> VendorCatalogs => Set<VendorCatalog>();
        
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>(); 
        public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();
        public DbSet<WarehouseTracking> WarehouseTrackings => Set<WarehouseTracking>();
        public DbSet<Zone> Zones => Set<Zone>();
        #endregion

        //public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        //public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }

            return true;
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<Customer>().HasMany( (CustomerType).WithMany(c => c.)
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SyncOrderResult>().HasNoKey();
        }
    }
}
