using System;
using System.Collections.Generic;

namespace PragueParking.Core;

public class Parkeringsplats
{
    private const int StandardPlatsstorlek = 2;

    private readonly List<Fordon> _fordon = new();
    private int _platsTillgänglig;

    public int Nummer { get; }

    // Platsens totala kapacitet.
    public int Storlek { get; }

    // Den kapacitet som fortfarande är ledig.
    public int PlatsTillgänglig => _platsTillgänglig;

    public IReadOnlyList<Fordon> ParkeradeFordon => _fordon;

    public Parkeringsplats(
        int nummer,
        int storlek = StandardPlatsstorlek)
    {
        if (nummer <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nummer));
        }

        if (storlek <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(storlek));
        }

        Nummer = nummer;
        Storlek = storlek;
        _platsTillgänglig = storlek;
    }

    public bool FörsökParkera(Fordon fordon)
    {
        ArgumentNullException.ThrowIfNull(fordon);

        if (!KanTaEmot(fordon))
        {
            return false;
        }

        _fordon.Add(fordon);
        _platsTillgänglig -= fordon.Storlek;

        return true;
    }

    public Fordon? HittaFordon(string registreringsnummer)
    {
        if (string.IsNullOrWhiteSpace(registreringsnummer))
        {
            return null;
        }

        string söktNummer = registreringsnummer
            .Replace(" ", "")
            .Trim();

        foreach (Fordon fordon in _fordon)
        {
            string sparatNummer = fordon.RegistreingsNummer
                .Replace(" ", "")
                .Trim();

            if (string.Equals(
                    sparatNummer,
                    söktNummer,
                    StringComparison.OrdinalIgnoreCase))
            {
                return fordon;
            }
        }

        return null;
    }

    public bool KanTaEmot(Fordon fordon)
    {
        ArgumentNullException.ThrowIfNull(fordon);

        return fordon.Storlek <= _platsTillgänglig;
    }

    public bool HarRegistreingsNummer(string registreringsnummer)
    {
        return HittaFordon(registreringsnummer) is not null;
    }

    public bool TaBortFordon(string registreringsnummer)
    {
        Fordon? fordon = HittaFordon(registreringsnummer);

        if (fordon is null)
        {
            return false;
        }

        _platsTillgänglig += fordon.Storlek;
        _fordon.Remove(fordon);

        return true;
    }
}