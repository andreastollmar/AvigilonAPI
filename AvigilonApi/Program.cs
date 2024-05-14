
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<HttpClientProivider>();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<HandleMediaResponse>();



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
