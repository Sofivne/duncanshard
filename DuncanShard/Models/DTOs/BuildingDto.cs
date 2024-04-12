namespace DuncanShard.Models.DTOs;

public record BuildingDto(
    string Id,
    bool IsBuilt,
    string Type,
    string Planet,
    string System,
    DateTime? EstimatedBuildTime,
    string? ResourceCategory);