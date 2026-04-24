using Domain.Models;

namespace Domain.Common;

public record AuthPayload(string Token, User User);
