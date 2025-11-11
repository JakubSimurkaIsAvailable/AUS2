using AUS2;
using System.Collections;
using System.Diagnostics.Metrics;

static void Main()
{
    
    int insertions = 10000000;
    int deletions = 2000000;
    int finds = 5000000;
    int intervalFinds = 1000000;
    int maxmins = 2000000;
    
    Tester tester = new Tester(1);
    tester.GenerateNumericData();
    Console.WriteLine("Data generated");
    tester.RunPerStructureTests(insertions, deletions, finds, intervalFinds, maxmins);

    for (int i = 10000; i < 10001; i++)
    {
        tester.RunTestsWithPercentualChance(51, i);
    }

    tester.RunAVLandBSTtestsWithContinuousData(100000);


}

Main();

