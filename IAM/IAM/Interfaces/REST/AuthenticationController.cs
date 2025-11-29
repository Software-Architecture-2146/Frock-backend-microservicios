using System.Net.Mime;
using Frock_backend.IAM.Domain.Services;
using Frock_backend.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Frock_backend.IAM.Interfaces.REST.Resources;
using Frock_backend.IAM.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// 1. NUEVOS USINGS
using MassTransit;
using Frock.Contracts;

namespace Frock_backend.IAM.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Authentication endpoints")]
// 2. INYECCIÓN DE IPublishEndpoint EN EL CONSTRUCTOR
public class AuthenticationController(
    IUserCommandService userCommandService, 
    IPublishEndpoint publishEndpoint) : ControllerBase
{
    /**
     * <summary>
     * Sign in endpoint. It allows authenticating a user
     * </summary>
     */
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Sign in",
        Description = "Sign in a user",
        OperationId = "SignIn")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was authenticated", typeof(AuthenticatedUserResource))]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
    {
        var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
        var authenticatedUser = await userCommandService.Handle(signInCommand);
        var resource =
            AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(authenticatedUser.user,
                authenticatedUser.token);
        return Ok(resource);
    }

    /**
     * <summary>
     * Sign up endpoint. It allows creating a new user
     * </summary>
     */
    [HttpPost("sign-up")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Sign-up",
        Description = "Sign up a new user",
        OperationId = "SignUp")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was created successfully")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
    {
        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
        
        // 3. CAPTURAMOS EL USUARIO CREADO
        // (Si esto te da error rojo, avísame, significa que tu servicio devuelve void)
        var createdUser = await userCommandService.Handle(signUpCommand);
        
        // 4. PUBLICAMOS EL EVENTO A RABBITMQ
        if (createdUser != null)
        {
            await publishEndpoint.Publish<IUserCreated>(new
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                // Si tu entidad tiene email, descomenta esto:
                // Email = createdUser.Email, 
                Role = createdUser.Role
            });
        }

        return Ok(new { message = "User created successfully" });
    }
}