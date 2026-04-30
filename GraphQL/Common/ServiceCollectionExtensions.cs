using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.Public.Repositories;
using Domain.Enums;
using DataAccess.EntityFramework;
using DataAccess.Repositories;
using Application.Interfaces.Public.Services;
using Application.Services;
using GraphQL.Schema;
using GraphQL.Types.Objects;
using Domain.Interfaces.Private;
using DataAccess.Security;
using GraphQL.Types.Inputs;
using DataAccess.ExternalServices;

namespace GraphQL.Common;

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
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IHasher, Hasher>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        return services;
    }

    public static IServiceCollection AddServiceGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
        .AddType<AuthPayloadType>()
        .AddType<GoogleAuthType>()
        .AddType<UserInterfaceType>()
        .AddType<BroadcasterType>()
        .AddType<ClientType>()
        .AddType<AddressType>()
        .AddType<CountryType>()
        .AddType<AgencyType>()
        .AddType<GoogleAuthInputType>()
        .AddType<CompleteGoogleBroadcasterSignUpInputType>()
        .AddType<CompleteGoogleClientSignUpInputType>()
        .AddQueryType<Query>()
        .AddMutationType<Mutation>()
        .AddProjections()             // Optimizes SQL queries
        .AddFiltering()               // Allow users to filter results
        .AddSorting()                 // Allow users to sort results
        .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);
        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        string[] allowedUrls = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? throw new ArgumentNullException("Cors allowed origins not found.");

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(allowedUrls)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>(client =>
        {
            client.BaseAddress = new Uri("https://oauth2.googleapis.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        return services;
    }

    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddErrorFilter<GlobalExceptionFilter>();
        return services;
    }
}
