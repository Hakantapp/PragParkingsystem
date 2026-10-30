namespace PragueParking.Filhantering;

/// <summary>
/// Innehåller de uppgifter om ett parkerat fordon
/// som ska sparas i JSON-filen.
/// </summary>
public class ParkeratFordonData
{
    public int Platsnummer { get; set; }

    public string Fordonstyp { get; set; } = "";

    public string Registreringsnummer { get; set; } = "";

    public DateTime? Incheckningstid { get; set; }
}