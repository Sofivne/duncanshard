using System.Collections.Immutable;

namespace DuncanShard.Models.DTOs;

public record UnitLocationDto(string System, string? Planet, ImmutableDictionary<string, int>? ResourcesQuantity);