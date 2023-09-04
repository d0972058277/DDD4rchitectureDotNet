using System.Reflection;
using Architecture;
using FluentValidation;
using FluentValidation.AspNetCore;
using Project.WebApi;
using Project.WebApi.Swashbuckle;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

SystemDateTime.InitUtcNow(() => DateTime.UtcNow);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<ValidatorSchemaFilter>();
    options.OperationFilter<JsonOperationFilter>();
    options.ExampleFilters();
    options.SupportNonNullableReferenceTypes();
    options.CustomSchemaIds(type => MD5TypeName.Get(type));
}).AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.DisableDataAnnotationsValidation = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.ShowExtensions();

        options.EnableDeepLinking();

        // Api 的 Schema 全部展開
        options.DefaultModelExpandDepth(int.MaxValue);

        // 把最底下的 Schemes 縮起來
        options.DefaultModelsExpandDepth(0);

        options.DisplayRequestDuration();

        options.DocExpansion(DocExpansion.None);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
