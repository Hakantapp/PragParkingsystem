using System.Collections.Generic;

namespace PragueParking.Filhantering;

public class ParkeringsKonfigurationData
{
    public List<ParkeringsplatsKonfiguration> Parkeringsplatser { get; set; } = new();

    public List<FordonstypKonfiguration> Fordonstyper { get; set; } = new();
}

public class ParkeringsplatsKonfiguration
{
    public int Antal { get; set; }

    public int Storlek { get; set; }
}

public class FordonstypKonfiguration
{
    public string Namn { get; set; } = "";

    public int Storlek { get; set; }
}