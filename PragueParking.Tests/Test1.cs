using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking.Core;

namespace PragueParking.Tests;

[TestClass]
public class ParkeringsplatsTester
{
    [TestMethod]
    public void TvåMCKanParkeraMenInteEnTredje()
    {
        Parkeringsplats plats = new Parkeringsplats(1);

        bool förstaMC = plats.FörsökParkera(new MC("KLE235"));
        bool andraMC = plats.FörsökParkera(new MC("GHJ025"));
        bool tredjeMC = plats.FörsökParkera(new MC("MBD201"));

        Assert.IsTrue(förstaMC);
        Assert.IsTrue(andraMC);
        Assert.IsFalse(tredjeMC);
    }
    [TestMethod]
    public void BilTarHelaPlatsenSåAttMCInteKanParkera()
    {
        Parkeringsplats plats = new Parkeringsplats(2);

        bool bilParkerad = plats.FörsökParkera(new Bil("BHJ526"));
        bool mcParkerad = plats.FörsökParkera(new MC("KTG457"));

        Assert.IsTrue(bilParkerad);
        Assert.IsFalse(mcParkerad);
    }
    [TestMethod]
    public void FörstaTioMinuternaÄrGratisFörBilOchMC()
    {
        ParkeringsAvgift avgiftsberäknare = new();

        decimal bilavgift = avgiftsberäknare.BeräknaPris(
            new Bil("BIL201"),
            TimeSpan.FromMinutes(10));

        decimal mcavgift = avgiftsberäknare.BeräknaPris(
            new MC("MC201"),
            TimeSpan.FromMinutes(10));

        Assert.AreEqual(0m, bilavgift);
        Assert.AreEqual(0m, mcavgift);
    }

    [TestMethod]
    public void EfterGratistidenDebiterasPåbörjadTimme()
    {
        ParkeringsAvgift avgiftsberäknare = new();

        decimal bilavgift = avgiftsberäknare.BeräknaPris(
            new Bil("HAS452"),
            TimeSpan.FromMinutes(11));

        decimal mcavgift = avgiftsberäknare.BeräknaPris(
            new MC("FGD502"),
            TimeSpan.FromMinutes(11));

        Assert.AreEqual(20m, bilavgift);
        Assert.AreEqual(10m, mcavgift);
    }
}