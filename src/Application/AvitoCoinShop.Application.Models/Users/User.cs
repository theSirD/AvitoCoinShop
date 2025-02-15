namespace AvitoCoinShop.Application.Models.Users;

public record User
(
    long? UserId,
    string Name,
    int Balance,
    string PasswordHash,
    DateTime CreatedAt);
