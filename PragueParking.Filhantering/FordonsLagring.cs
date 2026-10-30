namespace PragueParking.Filhantering;


using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PragueParking.Core;





public static class FordonsLagring
{
    // Sparar alla parkerade fordon i en JSON-fil.
    public static void Spara(Parkeringsgarage garage, string sökväg)
    {
        List<ParkeratFordonData> fordonsData = new();

        // Går igenom alla parkeringsplatser i garaget.
        for (int nummer = 1; nummer <= garage.AntalPlatser; nummer++)
        {
            Parkeringsplats? plats = garage.HämtaPlats(nummer);

            if (plats is null)
                continue;

            // Sparar uppgifterna för varje fordon på platsen.
            foreach (Fordon fordon in plats.ParkeradeFordon)
            {
                string fordonstyp;

                if (fordon is Bil)
                    fordonstyp = "Bil";
                else if (fordon is MC)
                    fordonstyp = "MC";
                else
                    throw new InvalidOperationException(
                        "Fordonstypen kan inte sparas.");

                fordonsData.Add(new ParkeratFordonData
                {
                    Platsnummer = plats.Nummer,
                    Fordonstyp = fordonstyp,
                    Registreringsnummer = fordon.RegistreingsNummer,
                    Incheckningstid = fordon.Incheckningstid
                });
            }
        }

        // Gör om fordonsuppgifterna till läsbar JSON-text.
        string json = JsonSerializer.Serialize(
            fordonsData,
            new JsonSerializerOptions { WriteIndented = true });

        // Skriver JSON-texten till filen.
        File.WriteAllText(sökväg, json);
    }

    // Läser fordon från en JSON-fil och försöker parkera dem i garaget.
    public static void LäsIn(Parkeringsgarage garage, string sökväg)
    {
        // Första gången finns filen kanske inte ännu.
        if (!File.Exists(sökväg))
            return;

        // Läser hela filens innehåll som text.
        string json = File.ReadAllText(sökväg);

        // Gör om JSON-texten till en lista med fordonsuppgifter.
        List<ParkeratFordonData>? fordonsData =
            JsonSerializer.Deserialize<List<ParkeratFordonData>>(json);

        if (fordonsData is null)
            return;

        // Skapar rätt fordonstyp och försöker parkera den på sparad plats.
        foreach (ParkeratFordonData data in fordonsData)
        {
            Fordon fordon;

            if (data.Fordonstyp == "Bil")
                fordon = new Bil(data.Registreringsnummer, data.Incheckningstid);
            else if (data.Fordonstyp == "MC")
                fordon = new MC(data.Registreringsnummer, data.Incheckningstid);
            else
                continue;

            garage.FörsökParkera(data.Platsnummer, fordon);
        }
    }
}