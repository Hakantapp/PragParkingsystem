using System.Security.Cryptography.X509Certificates;

namespace PragueParking.Core;

public class Fordon
{
    public DateTime Incheckningstid { get; private set; }
    public string RegistreingsNummer { get; private set; }
    public int Storlek { get; }
    public Fordon(
        string registreringsNummer,
        int storlek,
        DateTime? incheckningstid = null)

    {
        



        if (storlek <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(storlek),
            "Storleken måste vara större än 0.");
        }
        // Stoppar tomma värden och värden som innehåller blanksteg.
        if (string.IsNullOrWhiteSpace(registreringsNummer))
        {
            throw new ArgumentException("Registreringsnummer saknas");
        }

        string normaliseratNummer = registreringsNummer.Trim().ToUpperInvariant();

        if (normaliseratNummer.Length > 10)
        {
            throw new ArgumentException("Registreringsnumret är för långt.");
        }

        bool innehållerBokstavEllerSiffra = false;

        foreach (char tecken in normaliseratNummer)
        {
            if (char.IsLetterOrDigit(tecken))
            {
                innehållerBokstavEllerSiffra = true;
            }
            else if (tecken != '-')
            {
                throw new ArgumentException(
                    "Registreringsnumret innehåller ogiltiga tecken.");
            }
        }

        if (!innehållerBokstavEllerSiffra)
        {
            throw new ArgumentException(
                "Registreringsnumret måste innehålla minst en bokstav eller siffra.");
        }

        Incheckningstid =incheckningstid ??  DateTime.Now;

        RegistreingsNummer = normaliseratNummer;

        Storlek = storlek;
    }
}