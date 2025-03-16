using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Project;
using Project.Business;
using Project.Identity;
using Project.Infrastructure;

using Shared.Middleware;
using System.Configuration;
using System.Text;
using Shared.Interface;
using Project.Interface;
using Project.ActionFilter;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<FilterAction>();  
});
builder.Services.AddScoped<FilterAction>();

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<PhysicalPersonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("Project")));


builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection2")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();


var Settings = "DDDDDJJJJJJJJFFFLLLLLLLLLLLLLDDDDDLLLLLLLLDDDD";
var key = Encoding.ASCII.GetBytes(Settings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = "TestIssuer",
        ValidateAudience = true,
        ValidAudience = "TestAudience"
    };
});


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPhysicalPersonRepository, PhysicalPersonRepository>();
builder.Services.AddScoped<IPhysicalPersonService, PersonsService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<TokenService>();
//builder.Services.AddScoped<ErrorHandlingMiddleware>();

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
        pattern: "{controller=PhysicalPerson}/{action=Details}");



app.Run();