using Application.DTOs.Users;
using Application.Interfaces.Public.Services;

namespace GraphQL.Schema;

public class Mutation
{
    public async Task<ResultClientDTO> CreateClient([Service] IUserService userService, CreateClientDTO input)
        => await userService.CreateClientAsync(input);

    public async Task<ResultBroadcasterDTO> CreateBroadcaster([Service] IUserService userService, CreateBroadcasterDTO input)
        => await userService.CreateBroadcasterAsync(input);
}
