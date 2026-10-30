using PragueParking.Core;
using Spectre.Console;
using PragueParking.Filhantering;
using System.IO;
using Spectre.Console.Rendering;

namespace PragueParkingV2;



internal class ParkeringsMeny
{
    // Menyn använder samma garage under hela programkörningen.

    private const string Fordonsfil = "fordon.json";
    private const string Prisfil = "Parkeringspriser.txt";

    private readonly Parkeringsgarage _garage;

    private readonly ParkeringsKonfigurationData _konfiguration;

    private readonly string _prisfilSökväg;


    private ParkeringsAvgift _parkeringsAvgift;


    public ParkeringsMeny(Parkeringsgarage garage,
        ParkeringsKonfigurationData konfiguration)

    {
        ArgumentNullException.ThrowIfNull(garage);
        ArgumentNullException.ThrowIfNull(konfiguration);

        _garage = garage;
        _konfiguration = konfiguration;

        _prisfilSökväg = Path.Combine(
            AppContext.BaseDirectory,
            Prisfil);

        ParkeringsPrislista prislista =
            PrislistaLagring.LäsIn(_prisfilSökväg);

        _parkeringsAvgift = new ParkeringsAvgift(prislista);
    }





    // Kör menyn tills användaren väljer att avsluta.
    public void Kör()
    {

        FordonsLagring.LäsIn(_garage, Fordonsfil);

        bool körProgrammet = true;



        while (körProgrammet)

        {
            Console.Clear();

            string val = VäljMenyval();

            Console.Clear();

            switch (val)
            {
                case "1. Parkera fordon":
                    ParkeraFordon();
                    break;

                case "2. Flytta fordon":
                    OmplaceraFordon();
                    break;

                case "3. Sök fordon":
                    SökFordon();
                    break;

                case "4. Hämta fordon":
                    HämtaFordon();
                    break;

                case "5. Visa parkerade fordon":
                    VisaParkeradeFordon();
                    break;

                case "6. Läs in prislistan igen":
                    LäsInPrislistanIgen();
                    break;

                case "7. Visa parkeringskarta":
                    VisaParkeringskarta();
                    break;

                case "0. Avsluta":
                    körProgrammet = false;
                    break;
            }

            if (körProgrammet)
            {
                Console.WriteLine();
                Console.WriteLine("Tryck på valfri tangent för att återgå till menyn.");
                Console.ReadKey(true);
            }
        }
    }






    // Sparar garagets aktuella innehåll i JSON-filen.
    private void SparaGarage()
    {
        FordonsLagring.Spara(_garage, Fordonsfil);
    }





    // Visar menyn och returnerar användarens val.
    private string VäljMenyval()
    {
        var rubrik = new Panel(
                Align.Center(
                    new Markup(
                        "[bold white]PRAG PARKING[/]\n" +
                        "[grey]Parkeringshantering i Prag[/]")))
            .Header("[cyan]HUVUDMENY[/]", Justify.Center)
            .Border(BoxBorder.Double)
            .BorderColor(Color.Cyan1)
            .Padding(1, 1);

        AnsiConsole.Write(Align.Center(rubrik));
        AnsiConsole.WriteLine();

        string[] menyval =
        {
        "1. Parkera fordon",
        "2. Flytta fordon",
        "3. Sök fordon",
        "4. Hämta fordon",
        "5. Visa parkerade fordon",
        "6. Läs in prislistan igen",
        "7. Visa parkeringskarta",
        "0. Avsluta"
    };

        string[] centreradeMenyval = new string[menyval.Length];

        for (int i = 0; i < menyval.Length; i++)
        {
            // Tar med platsen som Spectres markeringspil använder.
            centreradeMenyval[i] = CentreraText(menyval[i], 2);
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold cyan]" + CentreraText("Vad vill du göra?") + "[/]")
                .PageSize(10)
                .WrapAround()
                .HighlightStyle(
    new Style(Color.Cyan1, decoration: Decoration.Bold))
                .AddChoices(centreradeMenyval))
            .Trim();
    }

    private static string CentreraText(string text, int extraBredd = 0)
    {
        int indrag = Math.Max(
            0,
            (Console.WindowWidth - text.Length - extraBredd) / 2);

        return new string(' ', indrag) + text;
    }


    private string FrågaEfterRegistreringsnummer()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Ange ett registreringsnummer[/]")
                .Validate(
                    nummer => !string.IsNullOrWhiteSpace(nummer),
                    "[red]Registreringsnumret får inte vara tomt.[/]"));
    }


    // Frågar efter ett platsnummer inom garagegränsen.
    private int FrågaEfterPlatsnummer(string fråga)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>(fråga)
                .Validate(
                    nummer => nummer >= 1 && nummer <= _garage.AntalPlatser,
                    $"[red]Ange ett platsnummer mellan 1 och {_garage.AntalPlatser}.[/]"));
    }


    private int? HämtaFordonstorlek(string namn)
    {
        foreach (FordonstypKonfiguration fordonstyp
                 in _konfiguration.Fordonstyper)
        {
            if (string.Equals(
                    fordonstyp.Namn,
                    namn,
                    StringComparison.OrdinalIgnoreCase))
            {
                return fordonstyp.Storlek;
            }
        }

        return null;
    }


    // Skapar ett fordon och försöker parkera det på vald plats.
    private void ParkeraFordon()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold cyan]PARKERA FORDON[/]").RuleStyle("cyan"));
        AnsiConsole.MarkupLine("[grey]Välj fordonstyp med piltangenterna och bekräfta med Enter.[/]");

        string fordonstyp = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold yellow]Vilket fordon vill du parkera?[/]")
                .PageSize(3)
                .HighlightStyle(new Style(Color.Black, Color.BlueViolet))
                .AddChoices("Bil", "MC", "Avbryt registrering"));
        if (fordonstyp == "Avbryt registrering")
        {
            AnsiConsole.MarkupLine("[yellow]Registreringen avbröts.[/]");
            return;
        }

        int? storlek = HämtaFordonstorlek(fordonstyp);

        if (storlek is null)
        {
            AnsiConsole.MarkupLine(
                $"[red]Fordonstypen {fordonstyp} saknas i konfigurationen.[/]");
            return;
        }

        string registreringsnummer = FrågaEfterRegistreringsnummer();

        // Kontrollera om registreringsnumret redan finns i garaget.
        int? befintligPlats =
            _garage.HittaPlatsFörFordon(registreringsnummer);

        if (befintligPlats is not null)
        {
            AnsiConsole.MarkupLine(
                $"[red]Registreringsnumret finns redan på plats {befintligPlats}.[/]");
            return;
        }

        try
        {
            Fordon fordon;

            if (fordonstyp == "Bil")
            {
                fordon = new Bil(registreringsnummer, storlek: storlek.Value);
            }
            else
            {
                fordon = new MC(registreringsnummer, storlek: storlek.Value);
            }


            while (true)
            {
                int platsnummer =
                    FrågaEfterPlatsnummer("[yellow]Ange platsnummer[/]");

                bool parkerad =
                    _garage.FörsökParkera(platsnummer, fordon);

                if (parkerad)
                {
                    SparaGarage();
                    AnsiConsole.MarkupLine("[green]Fordonet parkerades.[/]");
                    break;
                }

                AnsiConsole.MarkupLine(
                    "[yellow]Fordonet kunde inte parkeras på den platsen.[/]");

                bool försökIgen =
                    AnsiConsole.Confirm("Vill du försöka med en annan plats?");

                if (!försökIgen)
                {
                    break;
                }
            }
        }
        catch (ArgumentException)
        {
            AnsiConsole.MarkupLine(
                "[red]Ogiltigt registreringsnummer. Kontrollera numret och försök igen.[/]");
        }
    }


    private string? FrågaEfterRegistreringsnummerMedEsc()
    {
        AnsiConsole.Markup(
            "[yellow]Ange registreringsnummer ([bold]Esc[/] för att avbryta):[/] ");

        var tecken = new List<char>();

        while (true)
        {
            ConsoleKeyInfo tangent = Console.ReadKey(intercept: true);

            if (tangent.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Åtgärden avbröts.[/]");
                return null;
            }

            if (tangent.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();

                string registreringsnummer =
                    new string(tecken.ToArray()).Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(registreringsnummer))
                {
                    AnsiConsole.MarkupLine("[yellow]Åtgärden avbröts.[/]");
                    return null;
                }

                return registreringsnummer;
            }

            if (tangent.Key == ConsoleKey.Backspace)
            {
                if (tecken.Count > 0)
                {
                    tecken.RemoveAt(tecken.Count - 1);
                    Console.Write("\b \b");
                }

                continue;
            }

            if (!char.IsControl(tangent.KeyChar))
            {
                tecken.Add(tangent.KeyChar);
                Console.Write(tangent.KeyChar);
            }
        }
    }


    // Försöker flytta ett fordon till en vald plats.
    private void OmplaceraFordon()
    {
        string? registreringsnummer =
      FrågaEfterRegistreringsnummerMedEsc();

        if (registreringsnummer is null)
        {
            return;
        }

        while (true)
        {
            int målplatsnummer =
                FrågaEfterPlatsnummer("[yellow]Ange den nya platsen[/]");

            bool flyttades =
                _garage.FlyttaFordon(registreringsnummer, målplatsnummer);

            if (flyttades)
            {
                SparaGarage();
                AnsiConsole.MarkupLine("[green]Fordonet flyttades.[/]");
                break;
            }

            AnsiConsole.MarkupLine(
                "[yellow]Fordonet kunde inte flyttas till den platsen.[/]");

            if (!AnsiConsole.Confirm("Vill du försöka med en annan plats?"))
            {
                break;
            }
        }
    }




    // Söker efter ett registreringsnummer och visar platsen om fordonet hittas.
    private void SökFordon()
    {
        string? registreringsnummer =
    FrågaEfterRegistreringsnummerMedEsc();

        if (registreringsnummer is null)
        {
            return;
        }

        int? platsnummer =
            _garage.HittaPlatsFörFordon(registreringsnummer);

        if (platsnummer is null)
        {
            AnsiConsole.MarkupLine("[yellow]Fordonet hittades inte.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine(
                $"[green]Fordonet finns på plats {platsnummer}.[/]");
        }
    }




    // Tar bort ett fordon från garaget med hjälp av registreringsnumret.
    private void HämtaFordon()
    {
        string? registreringsnummer =
    FrågaEfterRegistreringsnummerMedEsc();

        if (registreringsnummer is null)
        {
            return;
        }

        Fordon? fordon = _garage.HittaFordon(registreringsnummer);

        if (fordon is null)
        {
            AnsiConsole.MarkupLine("[yellow]Fordonet hittades inte.[/]");
            return;
        }

        TimeSpan parkeringstid = DateTime.Now - fordon.Incheckningstid;
        decimal avgift = _parkeringsAvgift.BeräknaPris(fordon, parkeringstid);

        bool borttaget = _garage.TaBortFordon(registreringsnummer);

        if (!borttaget)
        {
            AnsiConsole.MarkupLine("[yellow]Fordonet kunde inte hämtas.[/]");
            return;
        }

        SparaGarage();

        int timmar = (int)parkeringstid.TotalHours;
        int minuter = parkeringstid.Minutes;

        AnsiConsole.MarkupLine("[green]Fordonet hämtades.[/]");
        AnsiConsole.MarkupLine(
            $"Parkerat i {timmar} timmar och {minuter} minuter.");
        AnsiConsole.MarkupLine($"Avgift: {avgift:0} CZK");
    }





    // Visar fordonen på alla platser som inte är tomma.
    private void VisaParkeradeFordon()
    {
        var tabell = new Table();

        tabell.Title("[bold cyan]Parkerade fordon[/]");
        tabell.Border(TableBorder.Rounded);

        tabell.AddColumn("Plats");
        tabell.AddColumn("Fordon");
        tabell.AddColumn("Registreringsnummer");

        bool hittadeFordon = false;

        for (int nummer = 1; nummer <= _garage.AntalPlatser; nummer++)
        {
            Parkeringsplats? plats = _garage.HämtaPlats(nummer);

            if (plats is null)
            {
                continue;
            }

            foreach (Fordon fordon in plats.ParkeradeFordon)
            {
                tabell.AddRow(
                    plats.Nummer.ToString(),
                    fordon.GetType().Name,
                    Markup.Escape(fordon.RegistreingsNummer));

                hittadeFordon = true;
            }
        }

        if (!hittadeFordon)
        {
            AnsiConsole.MarkupLine(
                "[yellow]Det finns inga parkerade fordon.[/]");
            return;
        }

        AnsiConsole.Write(tabell);
    }


    private void VisaParkeringskarta()
    {
        const int antalKolumner = 10;

        var karta = new Grid();

        for (int kolumn = 0; kolumn < antalKolumner; kolumn++)
        {
            karta.AddColumn();
        }

        int heltLedigaPlatser = 0;
        int delvisLedigaPlatser = 0;

        for (int förstaPlats = 1;
             förstaPlats <= _garage.AntalPlatser;
             förstaPlats += antalKolumner)
        {
            IRenderable[] rutor = new IRenderable[antalKolumner];

            for (int kolumn = 0; kolumn < antalKolumner; kolumn++)
            {
                int nummer = förstaPlats + kolumn;

                if (nummer > _garage.AntalPlatser)
                {
                    rutor[kolumn] = new Panel(" ")
                        .BorderColor(Color.Grey);
                    continue;
                }

                Parkeringsplats? plats = _garage.HämtaPlats(nummer);

                string färgnamn;
                Color färg;

                if (plats is null)
                {
                    färgnamn = "grey";
                    färg = Color.Grey;
                }
                else if (plats.ParkeradeFordon.Count == 0)
                {
                    färgnamn = "green";
                    färg = Color.Green;
                    heltLedigaPlatser++;
                }
                else if (plats.PlatsTillgänglig > 0)
                {
                    färgnamn = "yellow";
                    färg = Color.Yellow;
                    delvisLedigaPlatser++;
                }
                else
                {
                    färgnamn = "red";
                    färg = Color.Red;
                }

                rutor[kolumn] = new Panel(
                        $"[{färgnamn}]{nummer:000}[/]")
                    .BorderColor(färg)
                    .Padding(1, 0)
                    .Expand();
            }

            karta.AddRow(rutor);
        }
        AnsiConsole.Write(karta);

        AnsiConsole.MarkupLine(
            $"[green]Helt lediga platser: {heltLedigaPlatser}[/]");

        AnsiConsole.MarkupLine(
            $"[yellow]Delvis lediga platser: {delvisLedigaPlatser}[/]");

        AnsiConsole.MarkupLine(
            "[green]Grön = ledig[/]  [yellow]Gul = delvis ledig[/]  [red]Röd = full[/]");
    }


    private void LäsInPrislistanIgen()
    {
        try
        {
            ParkeringsPrislista prislista =
                PrislistaLagring.LäsIn(_prisfilSökväg);

            _parkeringsAvgift = new ParkeringsAvgift(prislista);

            AnsiConsole.MarkupLine(
                "[green]Prislistan lästes in igen.[/]");
        }
        catch (FileNotFoundException)
        {
            AnsiConsole.MarkupLine("[red]Prisfilen kunde inte hittas.[/]");
        }
        catch (FormatException)
        {
            AnsiConsole.MarkupLine(
                "[red]Prisfilen innehåller ett felaktigt värde eller format.[/]");
        }
        catch (IOException)
        {
            AnsiConsole.MarkupLine("[red]Prisfilen kunde inte läsas.[/]");
        }
    }
}