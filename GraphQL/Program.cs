using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.Public.Repositories;
using Application.Interfaces.Public.Services;
using DataAccess.EntityFramework;
using Application.Services;
using Domain.Enums;
using GraphQL.Schema;
using DataAccess.Repositories;
using Application.DTOs.Users;

var builder = WebApplication.CreateBuilder(args);

// Standard DI registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IBroadcasterRepository, BroadcasterRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// builder.Services.AddScoped<IContractService, ContractService>();
// builder.Services.AddScoped<IBillService, BillService>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (connectionString == null)
{
    Console.WriteLine("Connection string not found.");
    Environment.Exit(1);
}

builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(
    connectionString, options => options
        .MapEnum<Domain.Enums.UserState>("user_state_enum")
        .MapEnum<BillType>("bill_type_enum")
        .MapEnum<MembershipState>("membership_state_enum")));

// NpgsqlDataSourceBuilder dataSourceBuilder = new(connectionString)
// {
//     DefaultNameTranslator = new NpgsqlSnakeCaseNameTranslator()
// };
// dataSourceBuilder.MapEnum<Domain.Enums.UserState>("user_state_enum"); // Name must match PG exactly
// dataSourceBuilder.MapEnum<BillType>("bill_type_enum"); // Name must match PG exactly
// dataSourceBuilder.MapEnum<MembershipState>("membership_state_enum"); // Name must match PG exactly
// NpgsqlDataSource dataSource = dataSourceBuilder.Build();

// builder.Services.AddDbContext<DatabaseContext>(options =>
//     options.UseNpgsql(dataSource)
//     .LogTo(Console.WriteLine, LogLevel.Information)
//     .EnableSensitiveDataLogging()
// );

builder.Services
    // .AddSingleton(dataSource)
    .AddGraphQLServer()
    .AddQueryType<Query>()        // We'll create this class next
    .AddType<ResultUserDTO>()
    .AddType<ResultBroadcasterDTO>()
    .AddType<ResultClientDTO>()
    .AddInterfaceType<IResultUserDTO>()
    .AddMutationType<Mutation>()
    .AddProjections()             // Optimizes SQL queries
    .AddFiltering()               // Allow users to filter results
    .AddSorting()                // Allow users to sort results
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGraphQL(); // Default endpoint is /graphql

app.Run();
