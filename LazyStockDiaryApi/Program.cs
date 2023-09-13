using LazyStockDiaryApi.Helpers;
using LazyStockDiaryApi.HostedServices;
using LazyStockDiaryApi.Models;
using LazyStockDiaryApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddDbContext<DataContext>();
builder.Services.AddHostedService<SymbolCacheCleaner>();
builder.Services.AddHostedService<SymbolUpdater>();

builder.Services.AddSingleton<SymbolIntegrityService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();

app.Run();

