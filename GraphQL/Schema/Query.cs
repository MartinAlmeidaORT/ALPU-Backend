using Application.Interfaces.Public.Services;
using Domain.Models;
using Application.DTOs.Users;
using Application.DTOs.Country;

namespace GraphQL.Schema;

public class Query
{
    [GraphQLDescription("Healthcheck")]
    public string Ping() => "Pong";

    [UseProjection]
    public IQueryable<IResultUserDTO> GetUsers([Service] IUserService userService)
        => userService.GetAllUsers();

    [UseProjection]
    public IQueryable<ResultCountryDTO> GetCountries([Service] ICountryService countryService)
        => countryService.GetAllCountries();
}
