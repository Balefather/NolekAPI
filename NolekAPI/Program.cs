using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NolekAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NolekAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NolekAPIContext") ?? throw new InvalidOperationException("Connection string 'NolekAPIContext' not found.")));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(p => p.AddPolicy("AllowAllOrigins", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

// Add static files middleware to serve index.html as the default route
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
