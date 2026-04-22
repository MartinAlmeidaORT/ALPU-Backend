using GraphQL.Classes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddServiceGraphQL();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL(); // Default endpoint is /graphql

app.Run();
