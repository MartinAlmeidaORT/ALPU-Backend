using Application.Interfaces.Public.Services;
using Domain.Models;

namespace GraphQL.Schema;

public class Query
{
    [GraphQLDescription("Healthcheck")]
    public string Ping() => "Pong";

    [UseProjection]
    public IQueryable<User> GetUsers([Service] IUserService userService) => userService.GetAllUsers();

    [UseProjection]
    public IQueryable<Country> GetCountries([Service] ICountryService countryService) => countryService.GetAllCountries();

    [UseProjection]
    [UseSorting]
    public IQueryable<Service> GetServices([Service] IAlpuService alpuService) => alpuService.GetAllServices();
}
