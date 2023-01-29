var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

//Cognito Configuration: Its for cognito Identity
builder.Services.AddCognitoIdentity();

//If we need changes in password policy then we can configure like this
//builder.Services.AddCognitoIdentity(config =>
//{
//    config.Password = new Microsoft.AspNetCore.Identity.PasswordOptions
//    {
//        RequireDigit = false,
//        RequiredLength = 6,
//        RequiredUniqueChars = 0,
//        RequireLowercase = false,
//        RequireNonAlphanumeric = false,
//        RequireUppercase = false
//    };
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AWS: AuthAPI");
    });
}


//Cognito Configuration: 
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
