using IdentityToken.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<IdentityDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("IdentityTokenDb")));
builder.Services.AddControllers();
builder.Services.AddHttpClient("blockfrost", c => {
    c.BaseAddress = new Uri("https://cardano-mainnet.blockfrost.io/api/v0/");
    c.DefaultRequestHeaders.Add("project_id", builder.Configuration.GetValue<string>("BlockfrostProjectId"));
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IdentityToken.API", Version = "v1" });
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

app.UseAuthorization();

app.MapControllers();

app.Run();
