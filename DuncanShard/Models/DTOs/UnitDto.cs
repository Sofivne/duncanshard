using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace DuncanShard.Models.DTOs;

public record UnitDto(string Id, string Type, string? Planet, string? System, string? DestinationPlanet, string? DestinationSystem, int Health, string? DestinationShard, ImmutableDictionary<string, int>? ResourcesQuantity);