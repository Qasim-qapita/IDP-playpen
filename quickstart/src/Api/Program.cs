using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Authority = "http://localhost:5001";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Audience validation is disabled here because access to the api is modeled with ApiScopes only.
            //By default, no audience will be emitted unless the api is modeled with ApiResources instead.
            //See https://docs.duendesoftware.com/identityserver/v6/apis/aspnetcore/jwt/#adding-audience-validation
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    
    // this just adds the policy, in order to put into action, need to map this policy to controllers, just like on ["POLICY_MAPPED"] <- use this keyword to find the line
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "myapi");
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// ["POLICY_MAPPED"] 
// order for this line doesn't matter, we can put it before UseAuthorization and it will still works
// this will return forbidden if client doesnt match with the provided policy
app.MapControllers().RequireAuthorization("ApiScope");

app.MapControllers();

app.Run();
