﻿namespace TekkenStats.API.Data.Models;

public class PlayerName
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; init; }
    public DateOnly Date { get; init; }
    public required string PlayerId { get; init; }
    public Player Player { get; init; } = null!;
}