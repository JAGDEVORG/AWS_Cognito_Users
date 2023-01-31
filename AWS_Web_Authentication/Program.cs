using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(options =>
{
    options.ResponseType = builder.Configuration["Authentication:Cognito:ResponseType"];
    // MetadataAddress Format: "https: //cognito-idp.{your-region-id}.amazonaws.com/{your-user-pool-id}/.well-known/openid-configuration"
    options.MetadataAddress = builder.Configuration["Authentication:Cognito:MetadataAddress"];
    options.ClientId = builder.Configuration["Authentication:Cognito:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Cognito:ClientSecret"];
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        NameClaimType = "cognito:username",
    };

    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProviderForSignOut = (context) =>
        {
            var logoutUri = builder.Configuration["Authentication:Cognito:LogoutUrl"];
            logoutUri += $"?client_id={options.ClientId}&logout_uri={builder.Configuration["Authentication:Cognito:BaseUrl"]}";
            context.Response.Redirect(logoutUri);
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) 
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
