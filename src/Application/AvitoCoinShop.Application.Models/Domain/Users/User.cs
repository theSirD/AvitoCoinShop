namespace AvitoCoinShop.Application.Models.Domain.Users;

public record User
(
    long? UserId,
    string Name,
    int Balance,
    string PasswordHash,
    DateTime CreatedAt);
