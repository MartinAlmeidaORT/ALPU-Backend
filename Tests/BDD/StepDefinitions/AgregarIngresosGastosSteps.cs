using Reqnroll;
using FluentAssertions;
using Application.Interfaces.Public.Services;
using Domain.Models;
using Domain.Enums;
using Domain.Interfaces.Public.Services;
using Domain.Common.Inputs;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Domain.Common.Payloads;
using FluentResults;

namespace Tests.BDD.StepDefinitions;

[Binding]
public class RegistrarFacturaSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IBillService _billServiceMock;
    private readonly IContractService _contractServiceMock;
    private readonly IUserService _userServiceMock;

    private BillInput _input = new();
    private Bill? _registroGuardado;
    private Exception? _excepcion;
    private IHttpContextAccessor _httpContextAccessor;

    public RegistrarFacturaSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _billServiceMock = Substitute.For<IBillService>();
        _contractServiceMock = Substitute.For<IContractService>();
        _userServiceMock = Substitute.For<IUserService>();

        _userServiceMock
            .GetUserByIdAsync(Arg.Any<int>())
            .Returns(new Broadcaster { UserId = 1 });

        _billServiceMock
            .RegisterBillAsync(Arg.Any<BillInput>())
            .Returns(callInfo =>
            {
                var input = callInfo.Arg<BillInput>();
                _registroGuardado = new Bill
                {
                    BillId = 1,
                    Title = input.Title,
                    Description = input.Description,
                    Date = input.Date,
                    Amount = input.Amount,
                    Type = input.Type,
                    ContractId = input.ContractId
                };

                RegisterBillPayload payload = new(_registroGuardado, "");
                return Task.FromResult(Result.Ok(payload));
            });

        _contractServiceMock
            .GetContractByIdAsync(1)
            .Returns(new Contract { ContractId = 1 });

        _contractServiceMock
            .GetContractByIdAsync(Arg.Is<int>(id => id != 1))
            .ReturnsNull();
    }

    // ─── Background ─────────────────────────────────────────────────────────────

    [Given(@"existe un token válido de administrador")]
    public void GivenTokenAdministrador()
        => _httpContextAccessor = CrearContexto("administrador");

    [Given(@"existe un token válido de locutor")]
    public void GivenTokenLocutor()
        => _httpContextAccessor = CrearContexto("locutor");

    [Given(@"no se envía token de autenticación")]
    public void GivenSinToken()
        => _httpContextAccessor = CrearContextoSinToken();

    // ─── Pasos de acción ────────────────────────────────────────────────────────

    [When(@"se envía una solicitud para registrar una factura con datos válidos")]
    public async Task WhenRegistrarFacturaValida()
    {
        _input = new BillInput
        {
            Title = "Alquiler oficina",
            Description = "Pago mayo 2026",
            Date = new DateOnly(2026, 5, 26),
            Amount = 15000m,
            Type = BillType.Expense
        };

        await EjecutarRegistro();
    }

    [When(@"se envía una solicitud para registrar una factura")]
    public async Task WhenRegistrarFacturaSinDatos()
    {
        _input = new BillInput
        {
            Title = "Factura de prueba",
            Description = "Descripción",
            Date = new DateOnly(2026, 5, 26),
            Amount = 1000m,
            Type = BillType.Expense
        };

        await EjecutarRegistro();
    }

    [When(@"se envía una solicitud para registrar una factura con un contrato válido")]
    public async Task WhenRegistrarFacturaConContrato()
    {
        _input = new BillInput
        {
            Title = "Pago contrato locutor",
            Description = "Pago correspondiente al contrato",
            Date = new DateOnly(2026, 5, 26),
            Amount = 8500m,
            Type = BillType.Income,
            ContractId = 1
        };

        await EjecutarRegistro();
    }

    [When(@"se envía una solicitud para registrar una factura con un contrato inexistente")]
    public async Task WhenRegistrarFacturaContratoInexistente()
    {
        _input = new BillInput { ContractId = 999 };
        await EjecutarRegistro();
    }

    // ─── Pasos de verificación ──────────────────────────────────────────────────

    [Then(@"el sistema guarda la factura y retorna el registro creado")]
    public void ThenFacturaGuardada()
    {
        _excepcion.Should().BeNull();
        _registroGuardado.Should().NotBeNull();
        _billServiceMock.Received(1).RegisterBillAsync(Arg.Any<BillInput>());
    }

    [Then(@"el sistema retorna un error de autorización")]
    public void ThenErrorAutorizacion()
    {
        _excepcion.Should().NotBeNull();
        _excepcion.Should().BeOfType<UnauthorizedAccessException>();
    }

    [Then(@"el sistema guarda la factura y notifica a los usuarios del contrato")]
    public void ThenFacturaGuardadaConNotificacion()
    {
        _registroGuardado.Should().NotBeNull();
        _userServiceMock.Received(1)
            .AddNotificationAsync(
                Arg.Any<User>(),
                Arg.Any<string>(),
                Arg.Any<string>());
    }

    [Then(@"el sistema retorna un error indicando que el contrato no existe")]
    public void ThenErrorContratoInexistente()
    {
        _excepcion.Should().NotBeNull();
        _excepcion!.Message.Should().Contain("contrato");
    }

    // ─── Helpers privados ───────────────────────────────────────────────────────

    private async Task EjecutarRegistro()
    {
        try
        {
            var rol = _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.Role);

            if (rol is not ("administrador" or "contador" or "supervisor"))
                throw new UnauthorizedAccessException();

            if (_input.ContractId.HasValue)
            {
                var contrato = await _contractServiceMock
                    .GetContractByIdAsync(_input.ContractId.Value)
                    ?? throw new Exception("El contrato no existe");
            }

            await _billServiceMock.RegisterBillAsync(_input);

            if (_registroGuardado?.ContractId != null)
            {
                var user = await _userServiceMock.GetUserByIdAsync(1);
                await _userServiceMock.AddNotificationAsync(
                    user!,
                    "Pago de contrato registrado",
                    $"Se registró un pago para el contrato {_registroGuardado.ContractId}");
            }
        }
        catch (Exception ex)
        {
            _excepcion = ex;
        }
    }

    private static IHttpContextAccessor CrearContexto(string rol)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, rol),
            new(JwtRegisteredClaimNames.Sub, "1")
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(principal);

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(httpContext);
        return accessor;
    }

    private static IHttpContextAccessor CrearContextoSinToken()
    {
        var httpContext = Substitute.For<HttpContext>();
        httpContext.User.Returns(new ClaimsPrincipal());

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(httpContext);
        return accessor;
    }
}
