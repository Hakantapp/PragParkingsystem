using System;

namespace PragueParking.Core;

public class ParkeringsAvgift
{
    private readonly ParkeringsPrislista _prislista;

    // Används tills en inläst prislista skickas in.
    public ParkeringsAvgift()
        : this(new ParkeringsPrislista())
    {
    }

    public ParkeringsAvgift(ParkeringsPrislista prislista)
    {
        ArgumentNullException.ThrowIfNull(prislista);
        _prislista = prislista;
    }

    public decimal BeräknaPris(Fordon fordon, TimeSpan parkeringstid)
    {
        ArgumentNullException.ThrowIfNull(fordon);

        if (parkeringstid < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(parkeringstid),
                "Parkeringstiden kan inte vara negativ.");
        }

        TimeSpan gratistid =
            TimeSpan.FromMinutes(_prislista.GratistidMinuter);

        if (parkeringstid <= gratistid)
        {
            return 0m;
        }

        decimal prisPerTimme;

        if (fordon is Bil)
        {
            prisPerTimme = _prislista.BilAvgiftPerTimme;
        }
        else if (fordon is MC)
        {
            prisPerTimme = _prislista.MCAvgiftPerTimme;
        }
        else
        {
            throw new ArgumentException(
                "Fordonstypen saknar ett parkeringspris.",
                nameof(fordon));
        }

        TimeSpan debiterbarTid = parkeringstid - gratistid;

        decimal påbörjadeTimmar =
            (decimal)Math.Ceiling(debiterbarTid.TotalHours);

        return påbörjadeTimmar * prisPerTimme;
    }
}