namespace AvitoCoinShop.Application.Models.Dto;

public record SendCoinsRequest(
    string ToUser,
    int Amount);