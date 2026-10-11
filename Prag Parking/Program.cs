// =====================================================
// -----------------PRAGUE PARKING V1-------------------
// *****************************************************

// 101 element används för att index 0 inte används. Parkeringsplatserna är 1–100


string[] parkeringsGarage = new string[101];

bool KörProgammet = true;

// =====================================================
// ---------------------HUVUDMENY-----------------------
// *****************************************************

while (KörProgammet)
{
    Console.Clear();



    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.WriteLine("===== PRAGUE PARKING =====");

    Console.WriteLine("  1. Parkera fordon");
    Console.WriteLine("-----------------------");
    Console.WriteLine("  2. Sök efter fordon");
    Console.WriteLine("-----------------------");
    Console.WriteLine("  3. Hämta ut fordon");
    Console.WriteLine("-----------------------");
    Console.WriteLine("  4. Flytta fordon");
    Console.WriteLine("-----------------------");
    Console.WriteLine("  5. Visa parkeringen");
    Console.WriteLine("-----------------------");
    Console.WriteLine("  0. Avsluta Programmet");
    Console.WriteLine("-----------------------");

    Console.Write("Välj ett alternativ: ");
    string choice = Console.ReadLine();

    Console.ResetColor();

    switch (choice)
    {
        case "1":
            while (true)
            {
                ParkeraFordon();

                Console.WriteLine("\nTryck P för att registrera ett till fordon.");
                Console.WriteLine("Tryck på valfri annan tangent för att återgå till menyn.");

                if (Console.ReadKey(true).Key != ConsoleKey.P)
                {
                    break; // Bryter loopen om användaren inte trycker på P

                }
            }
            break;

        case "2":
            SökEfterFordon();
            break;

        case "3":
            HämtaUtFordon();
            break;

        case "4":
            FlyttaFordon();
            break;

        case "5":
            VisaGaragePlatser();


            break;

        case "0":
            KörProgammet = false;
            break;

        default:
            Console.WriteLine("Felaktigt val.");
            break;

    }
    if (KörProgammet)
    {
        Console.WriteLine("\nTryck på en tangent för att återgå till menyn.");
        Console.ReadKey(true);
        Console.Clear();

    }
    // Clear the console scrollback buffer
    Console.Write("\x1b[3J");
}


// =====================================================
//------------------ PARKERA FORDON---------------------
// *****************************************************

void ParkeraFordon()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("===== PARKERA FORDON =====");
    Console.WriteLine();

    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.Write("Ange fordonstyp (BIL eller MC): ");
    string fordonsTyp = Console.ReadLine().ToUpper();
    Console.ResetColor();

    // Kontrollera att användaren skrivit CAR eller MC

    if (fordonsTyp != "BIL" && fordonsTyp != "MC")
    {
        Console.WriteLine("Felaktig fordonstyp.");
        return;
    }

    Console.Write("Ange registreringsnummer: ");
    string regNr = Console.ReadLine().ToUpper();

    // Registreringsnummer får vara max 10 tecken

    while (regNr.Length > 10)
    {
        Console.WriteLine("Registreringsnumret får vara max 10 tecken. Försök igen.");
        Console.Write("Ange registreringsnummer: ");
        regNr = Console.ReadLine().ToUpper();
    }

    // Kontrollera om fordonet redan finns

    int BefintligPlats = HittaFordon(regNr);

    if (BefintligPlats != -1)
    {
        Console.WriteLine("Fordonet finns redan på plats " + BefintligPlats + ".");
        return;
    }

    // =================================================
    // -----------------PARKERA BIL---------------------
    // *************************************************

    if (fordonsTyp == "BIL")
    {
        // En bil måste ha en helt tom plats

        for (int i = 1; i <= 100; i++)
        {
            if (string.IsNullOrEmpty(parkeringsGarage[i]))
            {
                parkeringsGarage[i] = "BIL#" + regNr;

                Console.WriteLine();
                Console.WriteLine("Bilen har parkerats.");
                Console.WriteLine("Registreringsnummer: " + regNr);
                Console.WriteLine("Parkeringsplats: " + i);
                return;
            }
        }

        Console.WriteLine("Det finns ingen ledig plats för bilen.");
    }

    // =================================================
    // -----------------PARKERA MC----------------------
    // *************************************************
    else if (fordonsTyp == "MC")
    {
        // Se om det finns en MC som redan står på en plats och som inte delar med någon annan MC

        for (int i = 1; i <= 100; i++)
        {
            if (!string.IsNullOrEmpty(parkeringsGarage[i]))
            {
                if (parkeringsGarage[i].StartsWith("MC#") &&
                    !parkeringsGarage[i].Contains("|"))
                {
                    parkeringsGarage[i] =
                        parkeringsGarage[i] + "|MC#" + regNr;

                    Console.WriteLine();
                    Console.WriteLine("Motorcykeln har parkerats.");
                    Console.WriteLine("Den delar plats med en annan MC.");
                    Console.WriteLine("Parkeringsplats: " + i);
                    return;
                }
            }
        }

        // OM en MC inte kunde dela plats med en annan MC, leta efter en helt tom plats

        for (int i = 1; i <= 100; i++)
        {
            if (string.IsNullOrEmpty(parkeringsGarage[i]))
            {
                parkeringsGarage[i] = "MC#" + regNr;

                Console.WriteLine();
                Console.WriteLine("Motorcykeln har parkerats.");
                Console.WriteLine("Parkeringsplats: " + i);
                return;
            }
        }

        Console.WriteLine("Det finns ingen ledig plats för motorcykeln.");
    }


}

// =====================================================
// -----------------SÖK EFTER FORDON--------------------
// *****************************************************

void SökEfterFordon()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.DarkMagenta;
    Console.WriteLine("===== SÖK FORDON =====");
    Console.WriteLine();

    Console.Write("Ange registreringsnummer: ");
    string regNr = Console.ReadLine().ToUpper();
    Console.ResetColor();
    int parkeringsPlats = HittaFordon(regNr);

    if (parkeringsPlats == -1)
    {
        Console.WriteLine("Fordonet kunde inte hittas.");
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine("Fordonet hittades!");
        Console.WriteLine("Parkeringsplats: " + parkeringsPlats);
        Console.WriteLine("Platsen innehåller: " + parkeringsGarage[parkeringsPlats]);
    }
}

// =====================================================
// ----------METOD SOM LETAR EFTER ETT FORDON-----------
// *****************************************************

int HittaFordon(string regNr)
{
    for (int i = 1; i <= 100; i++)
    {
        if (!string.IsNullOrEmpty(parkeringsGarage[i]))
        {
            // Med en Split delar vi upp två MC om de delar plats enligt "|". Vi får då en array med två element.


            string[] fordon = parkeringsGarage[i].Split('|');

            foreach (string farkost in fordon)
            {
                string[] fordonsInformation = farkost.Split('#');

                if (fordonsInformation.Length == 2)
                {
                    if (fordonsInformation[1] == regNr)
                    {
                        return i;
                    }
                }
            }
        }
    }


    return -1;
}

// *****************************************************
// --------------HÄMTA UT / TA BORT FORDON--------------
// *****************************************************

void HämtaUtFordon()
{
    Console.Clear();

    Console.WriteLine("===== HÄMTA UT FORDON =====");
    Console.WriteLine();

    Console.Write("Ange registreringsnummer: ");
    string regNr = Console.ReadLine().ToUpper();

    int parkeringsPlats = HittaFordon(regNr);

    if (parkeringsPlats == -1)
    {
        Console.WriteLine("Fordonet kunde inte hittas.");
        return;
    }

    // har man 2 mc  behöver man ta reda på vilken mc som ska tas bort och behålla den andra mc:n på platsen.
    // Man behöver göra detta eftersom två MC kan dela på en parkeringsplats. Då är det två fordon som står på samma plats och de är separerade med "|".

    string[] fordon = parkeringsGarage[parkeringsPlats].Split('|');

    // Har 

    if (fordon.Length == 1)
    {
        parkeringsGarage[parkeringsPlats] = null;

        Console.WriteLine();
        Console.WriteLine("Fordonet har hämtats ut.");
        Console.WriteLine("Parkeringsplats " + parkeringsPlats + " är nu tom.");
    }

    // Har två MC delar platsen, ta bort den som hämtas ut och behåll den andra MC:n på platsen.

    else if (fordon.Length == 2)
    {
        // Checkar vilken MC som ska tas bort och behåller den andra MC:n på platsen.

        if (fordon[0].Contains("#" + regNr))
        {
            // Spara den andra MC:n
            parkeringsGarage[parkeringsPlats] = fordon[1];
        }
        else
        {
            // Spara den första MC:n
            parkeringsGarage[parkeringsPlats] = fordon[0];
        }

        Console.WriteLine();
        Console.WriteLine("Motorcykeln har hämtats ut.");
        Console.WriteLine(
            "Den andra motorcykeln står kvar på plats " +
            parkeringsPlats + ".");
    }


}

// =====================================================
// ------------------FLYTTA FORDON----------------------
// *****************************************************

void FlyttaFordon()
{
    Console.Clear();

    Console.WriteLine("===== FLYTTA FORDON =====");
    Console.WriteLine();

    Console.Write("Ange registreringsnummer: ");
    string regNr = Console.ReadLine().ToUpper();

    int tidigarePlats = HittaFordon(regNr);

    if (tidigarePlats == -1)
    {
        Console.WriteLine("Fordonet kunde inte hittas.");
        return;
    }

    Console.WriteLine("Fordonet står på plats " + tidigarePlats + ".");

    Console.Write("Ange ny parkeringsplats 1–100: ");

    int nyPlats;

    if (!int.TryParse(Console.ReadLine(), out nyPlats))
    {
        Console.WriteLine("Du måste ange ett nummer.");
        return;
    }

    if (nyPlats < 1 || nyPlats > 100)
    {
        Console.WriteLine("Platsen måste vara mellan 1 och 100.");
        return;
    }

    // Ta reda på om fordonet är BIL eller MC

    string[] fordonetpåtidigarePlats = parkeringsGarage[tidigarePlats].Split('|');

    string fordonSkaFlyttas = "";

    foreach (string fordon in fordonetpåtidigarePlats)
    {
        if (fordon.Contains("#" + regNr))
        {
            fordonSkaFlyttas = fordon;
            break;
        }
    }

    // =================================================
    // -----------------FLYTTA BIL----------------------
    // *************************************************

    if (fordonSkaFlyttas.StartsWith("BIL#"))
    {
        // Bilar kan bara flyttas till en helt tom plats

        if (!string.IsNullOrEmpty(parkeringsGarage[nyPlats]))
        {
            Console.WriteLine("Den nya platsen är upptagen.");

            return;
        }

        parkeringsGarage[nyPlats] = fordonSkaFlyttas;

        // Flytta bilen från den gamla platsen så blir den tom

        parkeringsGarage[tidigarePlats] = null;

        Console.WriteLine();
        Console.WriteLine(
            "Bilen har flyttats från plats " +
            tidigarePlats + " till plats " + nyPlats + ".");
    }

    // =================================================
    // ------------------FLYTTA MC----------------------
    // *************************************************

    else if (fordonSkaFlyttas.StartsWith("MC#"))
    {
        // MC kan fllyttas till en helt tom plats eller dela med en annan MC som redan står på platsen. Gränsen är att max två MC kan dela på en plats.

        if (string.IsNullOrEmpty(parkeringsGarage[nyPlats]))
        {
            parkeringsGarage[nyPlats] = fordonSkaFlyttas;
        }

        else if (parkeringsGarage[nyPlats].StartsWith("MC#") &&
                 !parkeringsGarage[nyPlats].Contains("|"))
        {
            parkeringsGarage[nyPlats] =
                parkeringsGarage[nyPlats] + "|" + fordonSkaFlyttas;
        }
        else
        {
            Console.WriteLine("Du kan inte parkera eller ställa motorcykeln på den platsen");

            return;
        }

        // Flytta MC från den gamla platsen. Om det bara stod en MC på platsen blir den tom. Om det stod två MC på platsen behåll den som inte flyttades.

        if (fordonetpåtidigarePlats.Length == 1)
        {


            parkeringsGarage[tidigarePlats] = null;
        }
        else
        {
            // det var två MC på platsen, behåll den som inte flyttades.


            if (fordonetpåtidigarePlats[0] == fordonSkaFlyttas)
            {
                parkeringsGarage[tidigarePlats] = fordonetpåtidigarePlats[1];
            }
            else
            {
                parkeringsGarage[tidigarePlats] = fordonetpåtidigarePlats[0];
            }
        }

        Console.WriteLine();
        Console.WriteLine(
            "Motorcykeln har flyttats från plats " +
            tidigarePlats + " till plats " + nyPlats + ".");
    }


}

// =====================================================
// -------------VISA HELA PARKERINGEN-------------------
// *****************************************************

void VisaGaragePlatser()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("===== PRAGUE PARKING =====");
    Console.WriteLine();
    Console.ResetColor();

    for (int i = 1; i <= 100; i++)
    {
        if (string.IsNullOrEmpty(parkeringsGarage[i]))
        {
            Console.WriteLine("Plats " + i + ": TOM");
        }
        else
        {
            Console.WriteLine(
                "Plats " + i + ": " + parkeringsGarage[i]);
        }
    }

}


