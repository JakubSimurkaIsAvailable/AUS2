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
    
    for (int seed = 5; seed < 6; seed++)
    {
        //Console.WriteLine(seed);
        

        // Activate per-structure tests (each structure runs in isolation and writes ms to console)
        //tester.RunPerStructureTests(insertions, deletions, finds, intervalFinds, maxmins);
        //tester.RunFindIntervalSimpleTest();
        
    }
    Tester tester = new Tester(1);
    tester.GenerateNumericData();
    Console.WriteLine("Data generated");
    tester.RunPerStructureTests(insertions, deletions, finds, intervalFinds, maxmins);
    
    
    /*
    for (int i = 10000; i < 10001; i++)
    {
        tester.RunTestsWithPercentualChance(51, i);
    }
    */
    //tester.RunAVLandBSTtestsWithContinuousData(100000);


}

Main();

