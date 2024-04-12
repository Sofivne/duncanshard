namespace DuncanShard.Models.DTOs;

public record UserDto(string Id, string Pseudo, DateTime DateOfCreation, Dictionary<string, int>? ResourcesQuantity);