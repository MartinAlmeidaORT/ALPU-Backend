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
using Domain.Interfaces.Public.Services;
using Application.Factories;
using Domain.Interfaces.Public.Singletons;
using Application.Singletons;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Amazon.S3;
using Amazon.Runtime;
using Amazon;


namespace GraphQL.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, ConfigurationManager configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(
            connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<Domain.Enums.UserState>("user_state_enum")
                             .MapEnum<Domain.Enums.BillType>("bill_type_enum")
                             .MapEnum<MembershipState>("membership_state_enum")
                             .MapEnum<ServiceType>("service_type_enum")
                             .MapEnum<PriceAdjustmentType>("price_adjustment_type_enum")
                             .MapEnum<Interval>("interval_enum")
                             .MapEnum<ContractState>("contract_state_enum");
            }
        ).UseSnakeCaseNamingConvention());
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IPriceTable, PriceTable>();
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var credentials = new SessionAWSCredentials(
                   configuration["AWS:AccessKeyId"],
                   configuration["AWS:SecretAccessKey"],
                   configuration["AWS:SessionToken"]
               );

            var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);

            return new AmazonS3Client(credentials, region);
        });
        services.AddSingleton<AmazonS3Service>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAlpuService, AlpuService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IBillService, BillService>();
        services.AddScoped<ICampaignServiceFactory, CampaignServiceFactory>();
        services.AddScoped<IHasher, Hasher>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        return services;
    }

    public static IServiceCollection AddServiceGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
        .AddAuthorization()
        .AddType<AuthPayloadType>()
        .AddType<GoogleAuthType>()
        .AddType<UserInterfaceType>()
        .AddType<BroadcasterType>()
        .AddType<ClientType>()
        .AddType<SupervisorType>()
        .AddType<AdministratorType>()
        .AddType<AccountantType>()
        .AddType<AddressType>()
        .AddType<CountryType>()
        .AddType<AgencyType>()
        .AddType<ServiceInterfaceType>()
        .AddType<ServiceIvrType>()
        .AddType<ServicePeriodType>()
        .AddType<ServiceNarrativeType>()
        .AddType<ServiceDateType>()
        .AddType<PieceType>()
        .AddType<ContractType>()
        .AddType<Types.Objects.BillType>()
        .AddType<BillInputType>()
        .AddType<GoogleAuthInputType>()
        .AddType<CompleteGoogleBroadcasterSignUpInputType>()
        .AddType<CompleteGoogleClientSignUpInputType>()
        .AddType<CampaignInputType>()
        .AddType<CampaignServiceInputType>()
        .AddType<CampaignType>()
        .AddType<EventCampaignServiceType>()
        .AddType<NarrativeCampaignServiceType>()
        .AddType<IvrCampaignServiceType>()
        .AddType<TvCampaignServiceType>()
        .AddType<RadioCampaignServiceType>()
        .AddType<CinemaCampaignServiceType>()
        .AddType<CameraCampaignServiceType>()
        .AddType<OtherMediaCampaignServiceType>()
        .AddType<CampaignType>()
        .AddType<CampaignType>()
        .AddType<ServiceFlagsInputType>()
        .AddType<ServiceFlagsType>()
        .AddQueryType<Query>()
        .AddMutationType<Mutation>()
        .AddSubscriptionType<Subscription>()
        .AddType<AnyType>()           // Allow JSON
        .AddInMemorySubscriptions()
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

    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>(client =>
        {
            client.BaseAddress = new Uri("https://oauth2.googleapis.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddHttpContextAccessor();

        string key = Environment.GetEnvironmentVariable("JWT_KEY") ??
            configuration["JWT:Secret"] ??
            throw new ApplicationException("JWT key is not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });
        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddErrorFilter<GlobalExceptionFilter>();
        return services;
    }
}
