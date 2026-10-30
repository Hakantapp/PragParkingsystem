using System;

namespace PragueParking.Core;

public class Bil : Fordon
{
    public Bil(
        string registreringsnummer,
        DateTime? incheckningstid = null,
        int storlek = 2)
        : base(registreringsnummer, storlek, incheckningstid)
    {
    }
}