using IdentityToken.API.Data;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using IdentityToken.API.Hubs;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddIdentityDbContextFactory(o => o.UseNpgsql(builder.Configuration.GetConnectionString("IdentityTokenDb")));
builder.Services.AddControllers();
builder.Services.AddHttpClient("blockfrost", c =>
{
    c.BaseAddress = new Uri("https://cardano-mainnet.blockfrost.io/api/v0/");
    c.DefaultRequestHeaders.Add("project_id", builder.Configuration.GetValue<string>("BlockfrostProjectId"));
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IdentityToken.API", Version = "v1" });
});
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityToken.API v1"));
}

// app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat", o => o.Transports = HttpTransportType.WebSockets);
});
app.UseAuthorization();
app.MapControllers();

// Update All ChatUsers to be offline
var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("IdentityTokenDb"));
using var dbContext = new IdentityDbContext(optionsBuilder.Options);
if(dbContext is not null)
    await dbContext.Database.ExecuteSqlRawAsync("update \"ChatUsers\" set \"IsOnline\" = false");

app.Run();
