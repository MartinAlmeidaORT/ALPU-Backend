using GraphQL.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddDatabase(builder.Configuration);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddServiceGraphQL();


builder.Services.AddExternalServices(builder.Configuration);

builder.Services.AddGlobalExceptionHandler();

builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();

app.MapGraphQL(); // Default endpoint is /graphql

await app.RunAsync();
