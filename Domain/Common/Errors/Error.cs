using System.Net;
using FluentResults;

namespace Domain.Common.Errors;

public class BaseError : Error
{
    public BaseError(string message, HttpStatusCode statusCode) : base(message)
    {
        Metadata.Add("StatusCode", statusCode);
    }

}

// Specific Error Types
public class InternalServerError() : BaseError("Fallo en el servidor.", HttpStatusCode.NotFound);
public class NotFoundError(string Msg) : BaseError(Msg, HttpStatusCode.NotFound);
public class BadRequestError(string Msg) : BaseError(Msg, HttpStatusCode.BadRequest);
public class ValidationError(string Msg) : BaseError(Msg, HttpStatusCode.BadRequest);
public class ConflictError(string Msg) : BaseError(Msg, HttpStatusCode.Conflict);
public class AuthError(string Msg) : BaseError(Msg, HttpStatusCode.Unauthorized);
