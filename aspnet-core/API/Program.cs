global using API.Services.UserService;
using API;
using API.Services.BackgroundService;
using Domain.DomainModel.Interface;
using Domain.DomainModel.Interface.RolesAndAccess;
using Domain.DomainModel.Interface.User;
using Infrastructure.Repositories;
using Infrastructure.Repositories.User;
using Infrastucture;
using Infrastucture.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Email;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region AddScoped for Controller Dependency Injecton 
// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// User Roles & Permissions
builder.Services.AddScoped<IAccessRepository, AccessRepository>();
builder.Services.AddScoped<IAccessTypeRepository, AccessTypeRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IModuleGroupRepository, ModuleGroupRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRoleAccessRepository, RoleAccessRepository>();
builder.Services.AddScoped<IRoleModuleAccessRepository, RoleModuleAccessRepository>();

builder.Services.AddScoped<IAutomobileRepository, AutomobileRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerCreditRepository, CustomerCreditRepository>();
builder.Services.AddScoped<ICustomerNoteRepository, CustomerNoteRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IDriverLogRepository, DriverLogRepository>(); 
builder.Services.AddScoped<IDriverLogDetailRepository, DriverLogDetailRepository>();
builder.Services.AddScoped<IDropShipRepository, DropShipRepository>();
builder.Services.AddScoped<IItemMasterlistReferenceRepository, ItemMasterlistReferenceRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderNoteRepository, OrderNoteRepository>();
builder.Services.AddScoped<IPartsManifestRepository, PartsManifestRepository>();
builder.Services.AddScoped<IPartsManifestDetailRepository, PartsManifestDetailRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPartsCatalogRepository, PartsCatalogRepository>();
builder.Services.AddScoped<IPartsPickingRepository, PartsPickingRepository>();
builder.Services.AddScoped<IPartsPickingDetailRepository, PartsPickingDetailRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentDetailRepository, PaymentDetailRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderDetailRepository, PurchaseOrderDetailRepository>();
builder.Services.AddScoped<ISequenceRepository, SequenceRepository>();
builder.Services.AddScoped<IStockPartsLocationRepository, StockPartsLocationRepository>();
builder.Services.AddScoped<IStockSettingsRepository, StockSettingsRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IVendorCatalogRepository, VendorCatalogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IWarehouseLocationRepository, WarehouseLocationRepository>();
builder.Services.AddScoped<IWarehouseStockRepository, WarehouseStockRepository>();
builder.Services.AddScoped<IWarehouseTrackingRepository, WarehouseTrackingRepository>();
builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
#endregion

//builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using Bearer scheme (\"bearer {token})\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false
        };
    });

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("API"));
});

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

#region CORS Policy
builder.Services.AddCors(options => options.AddPolicy(
    name: "CORSPolicy",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200",
                           "http://localhost",
                           "https://perfectfitwest.com",
                           "https://perfectfitwest.com/pfos",
                           "https://www.perfectfitwest.com",
                           "https://www.perfectfitwest.com/pfos",
                           "http://perfectfitwest.com",
                           "http://perfectfitwest.com/pfos",
                           "http://www.perfectfitwest.com",
                           "http://www.perfectfitwest.com/pfos").AllowAnyMethod().AllowAnyHeader();
        //policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    })
);

#endregion

builder.Services.AddHostedService<BackgroundWorkerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<BasicAuthenticationHandler>("Token");

app.UseHttpsRedirection();

app.UseCors("CORSPolicy");

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    //FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    //RequestPath = new PathString("/Resources")
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Assets")),
    RequestPath = new PathString("/Assets")
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
