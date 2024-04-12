namespace DuncanShard.Models.DTOs;

public record SystemDto(string Name, IEnumerable<Planet> Planets);