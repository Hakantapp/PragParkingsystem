using System;
using System.Text;
using System.Collections.Generic;



namespace PragueParking.Core;

public class Parkeringsgarage
{
    private const int StandardAntalPlatser = 100;

    private readonly int _antalPlatser;
    private readonly Parkeringsplats?[] _platser;

    public int AntalPlatser => _antalPlatser;

    public Parkeringsgarage(int antalPlatser = StandardAntalPlatser)
    {
        if (antalPlatser <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(antalPlatser));
        }

        _antalPlatser = antalPlatser;
        _platser = new Parkeringsplats?[antalPlatser + 1];

        for (int nummer = 1; nummer <= _antalPlatser; nummer++)
        {
            _platser[nummer] = new Parkeringsplats(nummer);
        }
    }
    public Parkeringsgarage(IReadOnlyList<int> platsstorlekar)
    {
        ArgumentNullException.ThrowIfNull(platsstorlekar);

        if (platsstorlekar.Count == 0)
        {
            throw new ArgumentException(
                "Garaget måste ha minst en parkeringsplats.",
                nameof(platsstorlekar));
        }

        _antalPlatser = platsstorlekar.Count;
        _platser = new Parkeringsplats?[_antalPlatser + 1];

        for (int index = 0; index < platsstorlekar.Count; index++)
        {
            int platsnummer = index + 1;
            int storlek = platsstorlekar[index];

            _platser[platsnummer] =
                new Parkeringsplats(platsnummer, storlek);
        }
    }

    public Parkeringsplats? HämtaPlats(int nummer)
    {
        if (nummer < 1 || nummer > _antalPlatser)
        {
            return null;
        }

        return _platser[nummer];
    }

    // Returnerar true om registreringsnumret redan finns i garaget.
    public bool RegistreringsnummerFinns(string registreringsnummer)
    {
        return HittaPlatsFörFordon(registreringsnummer) is not null;
    }

    // Parkerar fordonet om platsen finns och registreringsnumret inte redan används.
    public bool FörsökParkera(int platsnummer, Fordon fordon)
    {
        ArgumentNullException.ThrowIfNull(fordon);

        if (RegistreringsnummerFinns(fordon.RegistreingsNummer))
        {
            return false;
        }

        Parkeringsplats? plats = HämtaPlats(platsnummer);

        if (plats is null)
        {
            return false;
        }

        return plats.FörsökParkera(fordon);
    }

    // Returnerar platsnumret där fordonet finns, eller null om det inte hittas.
    public int? HittaPlatsFörFordon(string registreringsnummer)
    {
        for (int nummer = 1; nummer <= _antalPlatser; nummer++)
        {
            Parkeringsplats? plats = _platser[nummer];

            if (plats is not null &&
                plats.HarRegistreingsNummer(registreringsnummer))
            {
                return nummer;
            }
        }

        return null;
    }


    public Fordon? HittaFordon(string registreringsnummer)
    {
        if (string.IsNullOrWhiteSpace(registreringsnummer))
        {
            return null;
        }

        string söktRegistreringsnummer = registreringsnummer.Trim();

        for (int nummer = 1; nummer <= _antalPlatser; nummer++)
        {
            Parkeringsplats? plats = HämtaPlats(nummer);

            if (plats is null)
            {
                continue;
            }

            foreach (Fordon fordon in plats.ParkeradeFordon)
            {
                if (string.Equals(
                        fordon.RegistreingsNummer,
                        söktRegistreringsnummer,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return fordon;
                }
            }
        }

        return null;
    }

    public bool TaBortFordon(string registreringsnummer)
    {
        int? platsnummer = HittaPlatsFörFordon(registreringsnummer);

        if (platsnummer is null)
        {
            return false;
        }

        Parkeringsplats? plats = HämtaPlats(platsnummer.Value);

        if (plats is null)
        {
            return false;
        }

        return plats.TaBortFordon(registreringsnummer);
    }

    // Flyttar fordonet om det finns och målplatsen kan ta emot det.
    public bool FlyttaFordon(string registreringsnummer, int målplatsnummer)
    {
        int? ursprungligtPlatsnummer =
            HittaPlatsFörFordon(registreringsnummer);

        if (ursprungligtPlatsnummer is null)
        {
            return false;
        }

        Parkeringsplats? ursprungsplats =
            HämtaPlats(ursprungligtPlatsnummer.Value);

        Parkeringsplats? målplats =
            HämtaPlats(målplatsnummer);

        if (ursprungsplats is null ||
            målplats is null ||
            ursprungsplats == målplats)
        {
            return false;
        }

        Fordon? fordon =
            ursprungsplats.HittaFordon(registreringsnummer);

        if (fordon is null)
        {
            return false;
        }

        if (!målplats.KanTaEmot(fordon))
        {
            return false;
        }

        if (!ursprungsplats.TaBortFordon(registreringsnummer))
        {
            return false;
        }

        if (målplats.FörsökParkera(fordon))
        {
            return true;
        }

        // Försöker återställa fordonet om flytten misslyckas.
        ursprungsplats.FörsökParkera(fordon);
        return false;
    }
}