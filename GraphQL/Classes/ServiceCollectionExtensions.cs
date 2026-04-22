using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.Public.Repositories;
using Domain.Enums;
using DataAccess.EntityFramework;
using DataAccess.Repositories;
using Application.DTOs.Users;
using Application.Interfaces.Public.Services;
using Application.Services;
using GraphQL.Schema;
using Application.DTOs.Auth;

namespace GraphQL.Classes;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, ConfigurationManager configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(
            connectionString, options => options
                .MapEnum<Domain.Enums.UserState>("user_state_enum")
                .MapEnum<BillType>("bill_type_enum")
                .MapEnum<MembershipState>("membership_state_enum")));
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    public static IServiceCollection AddServiceGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
        .AddType<ResultUserDTO>()
        .AddType<ResultBroadcasterDTO>()
        .AddType<ResultClientDTO>()
        // .AddInputObjectType<GoogleAuthInput>()
        // .AddInputObjectType<RegisterClientGoogleDTO>()
        // .AddInputObjectType<RegisterBroadcasterGoogleDTO>()
        .AddType<AuthPayload>()
        .AddInterfaceType<IResultUserDTO>()
        .AddQueryType<Query>()
        .AddMutationType<Mutation>()
        .AddProjections()             // Optimizes SQL queries
        .AddFiltering()               // Allow users to filter results
        .AddSorting()                 // Allow users to sort results
        .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);
        return services;
    }
}
