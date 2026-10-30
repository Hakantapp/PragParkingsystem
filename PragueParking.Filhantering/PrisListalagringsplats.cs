using System.Globalization;
using System.IO;
using PragueParking.Core;

namespace PragueParking.Filhantering;

public static class PrislistaLagring
{
    public static ParkeringsPrislista LäsIn(string sökväg)
    {
        if (!File.Exists(sökväg))
        {
            throw new FileNotFoundException(
                "Prislistan kunde inte hittas.",
                sökväg);
        }

        ParkeringsPrislista prislista = new();

        foreach (string ursprungligRad in File.ReadAllLines(sökväg))
        {
            string rad = ursprungligRad.Trim();

            // Hoppar över tomma rader och kommentarer.
            if (rad.Length == 0 || rad.StartsWith("#"))
            {
                continue;
            }

            string[] delar = rad.Split('=', 2);

            if (delar.Length != 2)
            {
                throw new FormatException(
                    $"Fel format på raden: {rad}");
            }

            string namn = delar[0].Trim();
            string värde = delar[1].Trim();

            switch (namn)
            {
                case "Bil":
                    if (!decimal.TryParse(
                            värde,
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture,
                            out decimal bilpris) ||
                        bilpris < 0)
                    {
                        throw new FormatException(
                            "Bilpriset måste vara ett giltigt, positivt tal.");
                    }

                    prislista.BilAvgiftPerTimme = bilpris;
                    break;

                case "MC":
                    if (!decimal.TryParse(
                            värde,
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture,
                            out decimal mcpris) ||
                        mcpris < 0)
                    {
                        throw new FormatException(
                            "MC-priset måste vara ett giltigt, positivt tal.");
                    }

                    prislista.MCAvgiftPerTimme = mcpris;
                    break;

                case "GratisMinuter":
                    if (!int.TryParse(värde, out int gratisminuter) ||
                        gratisminuter < 0)
                    {
                        throw new FormatException(
                            "GratisMinuter måste vara ett giltigt tal.");
                    }

                    prislista.GratistidMinuter = gratisminuter;
                    break;

                default:
                    throw new FormatException(
                        $"Okänd inställning i prislistan: {namn}");
            }
        }

        return prislista;
    }
}