using Application.Interfaces.Public.Services;
using Domain.Models;
using Application.DTOs.Users;

namespace GraphQL.Schema;

public class Query
{
    [GraphQLDescription("Healthcheck")]
    public string Ping() => "Pong";

    [UseProjection]
    public IQueryable<IResultUserDTO> GetUsers([Service] IUserService userService)
        => userService.GetAllUsers();
}
