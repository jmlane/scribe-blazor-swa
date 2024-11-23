using System;

namespace Scribe.Shared;

public class Character
{
    public Guid Id { get; set;}
    public string? Name { get; set; }
    public string? Player { get; set; }
    public string? Archetype { get; set; }
    public int Brawn { get; set; }
    public int Agility { get; set; }
    public int Intellect { get; set; }
    public int Cunning { get; set; }
    public int Willpower { get; set; }
    public int Presence { get; set; }
}
