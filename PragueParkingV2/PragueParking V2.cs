using System;
using PragueParking.Core;
using System.IO;
using System.Collections.Generic;
using PragueParking.Filhantering;


namespace PragueParkingV2;

internal class Program
{
    static void Main(string[] args)
    {
        string sökväg = Path.Combine(
      AppContext.BaseDirectory,
      "ParkeringsKonfiguration.json");

        ParkeringsKonfigurationData konfiguration =
            ParkeringsKonfigurationLagring.LäsIn(sökväg);

        List<int> platsstorlekar = new();

        foreach (ParkeringsplatsKonfiguration plats
                 in konfiguration.Parkeringsplatser)
        {
            for (int antal = 0; antal < plats.Antal; antal++)
            {
                platsstorlekar.Add(plats.Storlek);
            }
        }

        Parkeringsgarage garage = new(platsstorlekar);
        ParkeringsMeny meny = new(garage, konfiguration);
        meny.Kör();


    }
}