# language: es
Característica: CU10 - Registrar factura institucional

  Antecedentes:
    Dado existe un token válido de administrador

  Escenario: Registrar una factura exitosamente
    Cuando se envía una solicitud para registrar una factura con datos válidos
    Entonces el sistema guarda la factura y retorna el registro creado

  Escenario: Token inválido o ausente
    Dado no se envía token de autenticación
    Cuando se envía una solicitud para registrar una factura
    Entonces el sistema retorna un error de autorización

  Escenario: Token de un rol sin permisos
    Dado existe un token válido de locutor
    Cuando se envía una solicitud para registrar una factura
    Entonces el sistema retorna un error de autorización

  Escenario: Registrar factura con ID de contrato asociado
    Cuando se envía una solicitud para registrar una factura con un contrato válido
    Entonces el sistema guarda la factura y notifica a los usuarios del contrato

  Escenario: Registrar factura con ID de contrato inexistente
    Cuando se envía una solicitud para registrar una factura con un contrato inexistente
    Entonces el sistema retorna un error indicando que el contrato no existe
