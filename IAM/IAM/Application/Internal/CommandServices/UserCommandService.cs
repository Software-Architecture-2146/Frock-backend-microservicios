using Frock_backend.IAM.Application.Internal.OutboundServices;
using Frock_backend.IAM.Domain.Model.Aggregates;
using Frock_backend.IAM.Domain.Model.Commands;
using Frock_backend.IAM.Domain.Repositories;
using Frock_backend.IAM.Domain.Services;
using Frock_backend.shared.Domain.Repositories;
// No necesitas el using de ValueObjects aquí porque el command ya lo trae,
// pero déjalo por si acaso.

namespace Frock_backend.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    public async Task<(User user, string token)> Handle(SignInCommand command)
    {
        var user = await userRepository.FindByEmailAsync(command.Email);

        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        var token = tokenService.GenerateToken(user);

        return (user, token);
    }

    public async Task<User?> Handle(SignUpCommand command)
    {
        if (await userRepository.ExistsByUsernameAsync(command.Username))
             throw new Exception($"Username '{command.Username}' is already taken");
        
        if (await userRepository.ExistsByEmail(command.Email))
             throw new Exception($"Email '{command.Email}' is already registered");

        var hashedPassword = hashingService.HashPassword(command.Password);
        
        // --- CORRECCIÓN FINAL Y SIMPLE ---
        // Como command.Role YA ES UN ENUM, lo pasamos directo.
        // No hace falta convertir nada.
        var user = new User(
            command.Email, 
            command.Username, 
            hashedPassword, 
            command.Role // <--- ¡Directo y sin escalas!
        );

        try
        {
            await userRepository.AddAsync(user);
            await unitOfWork.CompleteAsync();
            return user; 
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while creating user: {e.Message}");
        }
    }
}