
using Avigilon.Core.Interfaces;
using Avigilon.Infrastructure.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IHandleMediaResponse, HandleMediaResponse>();
builder.Services.AddScoped<IAvigilonApiCalls, AvigilonApiCalls>();
builder.Services.AddScoped<IInputValidations, InputValidations>();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("AllowAllOrigins",
  policy =>
  {
      policy.AllowAnyHeader()
      .AllowAnyMethod()
      .AllowAnyOrigin();
  }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
