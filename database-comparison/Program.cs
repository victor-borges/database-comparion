using Microsoft.EntityFrameworkCore;
using DatabaseComparison.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mySqlServerVersion = new MySqlServerVersion(new Version(8, 0, 31));

builder.Services.AddDbContext<MySqlDbContext>(dbContextOptions => dbContextOptions
    .UseMySql(builder.Configuration.GetConnectionString("MySQL")!, mySqlServerVersion, providerOptions =>
        providerOptions.EnableRetryOnFailure().MigrationsHistoryTable("__ef_migrations_history"))
    .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<PostgresDbContext>(dbContextOptions => dbContextOptions
    .UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")!, providerOptions =>
        providerOptions.EnableRetryOnFailure().MigrationsHistoryTable("__ef_migrations_history"))
    .UseSnakeCaseNamingConvention());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
