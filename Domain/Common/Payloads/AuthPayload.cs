using Domain.Models;

namespace Domain.Common.Payloads;

public record AuthPayload(string Token, User User);
