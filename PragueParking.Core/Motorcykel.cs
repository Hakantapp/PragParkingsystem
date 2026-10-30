using System;

namespace PragueParking.Core;

public class MC : Fordon
{
    public MC(
        string registreringsnummer,
        DateTime? incheckningstid = null,
        int storlek = 1)
        : base(registreringsnummer, storlek, incheckningstid)
    {
    }
}