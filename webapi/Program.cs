using Content.Providers;
using Microsoft.OpenApi.Models;

//var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        name: "v1",
        info: new OpenApiInfo
        {
            Title = "Content API",
            Version = "alpha"
        });
    options.CustomSchemaIds(x => x.FullName);
});

// https://stackoverflow.com/questions/46940710/getting-value-from-appsettings-json-in-net-core
DatabaseProvider.Init("Data Source=content.db; foreign keys=true; Pooling=True; Max Pool Size=100;");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();