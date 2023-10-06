using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add services to thyne container
builder.Services.AddApplicationServices(builder.Configuration);
// for JWT Authorize
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// add exception middle ware here, an exception handling middleware.
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration.GetSection("AllowCors:Origins").Get<string[]>());
});

app.UseHttpsRedirection();

// to ask do you have a valid token?
app.UseAuthentication();
// okay you have a valid token.
app.UseAuthorization();

app.MapControllers();

app.Run();
