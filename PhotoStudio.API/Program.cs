using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Repository;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using Neo4jClient;
using PhotoStudio.GraphDB.Services;
using PhotoStudio;
using PhotoStudio.Domain.MappingProfiles;
using PhotoStudio.Infrastructure.MongoDB;
using PhotoStudio.Repository.Repositories;
using PhotoStudio.Infrastructure;
using MongoDB.Driver;
using PhotoStudio.Infrastructure.Neo4j;
using Stripe;
using PhotoStudio.Infrastructure.MongoDB.MongoRepository;
using PhotoStudio.Application.Services;
using System.Text.Json.Serialization;
using Microsoft.Extensions.FileProviders;


string pass = "NPPPElaAmina";

// Konfiguracija Serilog 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
    .WriteTo.Console()
    .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Conditional(
        _ => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development",
        wt => wt.Seq("http://localhost:5341")
    )
    .CreateLogger();

try
{
    Log.Logger.Information("Aplikacija se pokreće");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.File("logs.txt"));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowHostFrontend",
            builder =>
            {
                builder.WithOrigins("http://mielle-001-site1.atempurl.com")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
    });


    // Konfiguracija Neo4j
    // var client = new GraphClient(new Uri("http://localhost:7474/"), "neo4j", pass);
    // await client.ConnectAsync();
    // builder.Services.AddSingleton<IGraphClient>(client);
    // builder.Services.AddSingleton<UsersGraphDbContext>();
    // builder.Services.AddSingleton<PaymentsGraphDbContext>();
    // builder.Services.AddSingleton<EmployeesGraphDbContext>();
    //builder.Services.AddSingleton<BookingGraphDbContext>();
    //builder.Services.AddSingleton<AlbumsGraphDbContext>();

    // Neo4jDatabaseInitializerService
    // builder.Services.AddSingleton<Neo4jDatabaseInitializerService>();
    builder.Services.AddSingleton<StripeClient>(new StripeClient("your-stripe-secret-key"));

    // Konfiguracija MongoDB
    /*builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27017"));
    builder.Services.AddSingleton(provider =>
    {
        var client = provider.GetRequiredService<IMongoClient>();
        return client.GetDatabase("photostudio");
    });
    builder.Services.AddSingleton<MongoService>();
    builder.Services.AddHostedService<DatabaseInitializerService>();

    //Registracija MongoDB repozitorijuma
    builder.Services.AddScoped<IPaymentRepository, MongoPaymentRepository>();
    builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
    builder.Services.AddScoped<IAlbumRepository, MongoAlbumRepository>();
    builder.Services.AddScoped<IMediaRepository, MongoMediaRepository>();
    builder.Services.AddScoped<IBookingRepository, MongoBookingRepository>();
    builder.Services.AddScoped<IEmployeeRepository, MongoEmployeeRepository>();*/


    // Konfiguracija DbContext
    builder.Services.AddDbContext<PhotoStudioContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("PhotoStudioContext");
        if (string.IsNullOrEmpty(connectionString))
        {
            Log.Logger.Error("Connection string for PhotoStudioContext is missing or empty.");
            throw new InvalidOperationException("Connection string for PhotoStudioContext is missing or empty.");
        }
        options.UseSqlServer(connectionString);
    });

    builder.Services.AddAutoMapper(typeof(MappingProfile));

    builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
    builder.Services.AddScoped<IBookingRepository, BookingRepository>();
    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IMediaRepository, MediaRepository>();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<PhotoStudio.Application.Services.MediaService>();
    builder.Services.AddScoped<AlbumService>();
    builder.Services.AddScoped<BookingService>();
    builder.Services.AddScoped<EmployeeService>();
    builder.Services.AddScoped<PaymentService>();
    builder.Services.AddScoped<UserService>();

    var secret = builder.Configuration["Jwt:Secret"];
    if (string.IsNullOrEmpty(secret))
    {
        Log.Logger.Error("JWT Secret is missing or empty.");
        throw new InvalidOperationException("JWT Secret is missing or empty.");
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });




    builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        opts.JsonSerializerOptions.WriteIndented = true;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    // Kreiranje aplikacije
    var app = builder.Build();

    // Inicijalizacija Neo4j baze podataka
    //var initializer = app.Services.GetRequiredService<Neo4jDatabaseInitializerService>();
    //await initializer.InitializeDatabaseAsync();

   // if (app.Environment.IsDevelopment())
   // {
        app.UseSwagger();
        app.UseSwaggerUI();
    //}


    app.UseHttpsRedirection();
    app.UseCors("AllowHostFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<LoggingMiddleware>();
    app.MapControllers();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
        RequestPath = "/uploads"
    });


    app.Run();

}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Aplikacija nije uspela da se pokrene");
}
finally
{
    Log.CloseAndFlush();
}