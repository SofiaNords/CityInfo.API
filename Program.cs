using Microsoft.AspNetCore.StaticFiles;

// Här skapas en WebapplicationBuilder som används för att konfigurera och bygga en ASP.NET Core-applikation.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();


builder.Services.AddProblemDetails();
//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = ctx =>
//    {
//        ctx.ProblemDetails.Extensions.Add("additionalInfo",
//            "Additional info example");
//        ctx.ProblemDetails.Extensions.Add("server",
//            Environment.MachineName);
//    };
//});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
