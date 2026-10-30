# Prague Parking V2

 Det här är ett konsolprogram för att hantera ett parkeringshus i Prag. 
Programmet kan följande - Parkera, flytta, söka efter och hämta fordon samt visa parkeringskartan.


# Krav för programmet

- .NET 10 SDK för att bygga programmet från källkoden
- .NET 10 runtime för att köra den publicerade versionen


# Kör programmet

1. Öppna lösningens ' PragParkingsystem.slnx' fil i Visual Studio.
2. Kontrollera att 'PragueParkingV2' är valt som startprojekt.
3. Starta programmet med (Ctrl + F5) 

# Filer 
- 'ParkeringsKonfiguration.json' innehåller  parkeringsplatsernas antal och storlekar smat fordonstypnernas storlekar.
- 'Parkeringspriser.txt' innehåller priserna. Rader som börjar med # är kommentarer.
- 'fordon.json' innehåller parkerade fordon och kan skapas eller uppdateras av programmet.

# Tester

Öppna (Test Explorer) i Visual Studio och välj  (Run All Tests) för att köra MSTest/testerna.