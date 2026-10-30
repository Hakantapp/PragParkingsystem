using System.IO;
using System.Text.Json;

namespace PragueParking.Filhantering;

public static class ParkeringsKonfigurationLagring
{
    public static ParkeringsKonfigurationData LäsIn(string sökväg)
    {
        if (!File.Exists(sökväg))
        {
            throw new FileNotFoundException(
                "Konfigurationsfilen kunde inte hittas.",
                sökväg);
        }

        string json = File.ReadAllText(sökväg);

        ParkeringsKonfigurationData? konfiguration =
            JsonSerializer.Deserialize<ParkeringsKonfigurationData>(json);

        if (konfiguration is null)
        {
            throw new InvalidDataException(
                "Konfigurationsfilen innehåller inga data.");
        }

        KontrolleraKonfiguration(konfiguration);

        return konfiguration;
    }

    private static void KontrolleraKonfiguration(
        ParkeringsKonfigurationData konfiguration)
    {
        if (konfiguration.Parkeringsplatser.Count == 0)
        {
            throw new InvalidDataException(
                "Konfigurationen saknar parkeringsplatser.");
        }

        if (konfiguration.Fordonstyper.Count == 0)
        {
            throw new InvalidDataException(
                "Konfigurationen saknar fordonstyper.");
        }

        foreach (ParkeringsplatsKonfiguration plats
                 in konfiguration.Parkeringsplatser)
        {
            if (plats.Antal <= 0 || plats.Storlek <= 0)
            {
                throw new InvalidDataException(
                    "Antal platser och platsstorlek måste vara större än 0.");
            }
        }

        foreach (FordonstypKonfiguration fordonstyp
                 in konfiguration.Fordonstyper)
        {
            if (string.IsNullOrWhiteSpace(fordonstyp.Namn) ||
                fordonstyp.Storlek <= 0)
            {
                throw new InvalidDataException(
                    "Fordonstypen måste ha ett namn och en storlek större än 0.");
            }
        }
    }
}