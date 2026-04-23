using GraphQL.Classes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddServiceGraphQL();

builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.MapGraphQL(); // Default endpoint is /graphql

app.Run();
