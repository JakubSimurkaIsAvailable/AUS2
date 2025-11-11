using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AUS2
{
    internal class Tester
    {
        private Random rng;
        private List<int> InsertedItems = new List<int>();

        public Tester(int seed)
        {
            rng = new Random(seed);
        }

        public void GenerateNumericData(int total = 10_000_000)
        {
            InsertedItems.Clear();
            for (int i = 0; i < total; i++)
                InsertedItems.Add(i);
            Shuffle(InsertedItems);
        }

        public void ResetShuffle()
        {
            Shuffle(InsertedItems);
        }

        public void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public void RunAVLandBSTtestsWithContinuousData(int numberOfNodes)
        {
            var bst = new BST<NumberData>();
            var avl = new AVL<NumberData>();
            List<NumberData> numberDatas = new List<NumberData>();
            for (int i = 0; i < numberOfNodes; i++)
            {
                numberDatas.Add(new NumberData(i));

            }
            Stopwatch sw = new Stopwatch();
            //insert into AVL
            sw.Start();
            foreach (var data in numberDatas)
            {
                avl.Insert(data);
            }
            sw.Stop();
            Console.WriteLine("AVL insertion time with continuous data: " + sw.ElapsedMilliseconds + " ms");
            sw.Reset();
            //insert into BST
            sw.Start();
            foreach (var data in numberDatas)
            {
                bst.Insert(data);
            }
            sw.Stop();
            Console.WriteLine("BST insertion time with continuous data: " + sw.ElapsedMilliseconds + " ms");

            bool isAvl = avl.IsAVL();
            if (isAvl)
            {
                Console.WriteLine("AVL is AVL with continuous data");
            }
            else
            {
                throw new Exception("AVL is not AVL with continuous data");
            }
        }
        public void RunTestsWithPercentualChance(int repetitions, int seed)
        {
            var bst = new BST<NumberData>();
            var avl = new AVL<NumberData>();
            var rb = new RBTree<NumberData>();
            List<List<NumberData>> bstIntervals = new List<List<NumberData>>();
            List<List<NumberData>> avlIntervals = new List<List<NumberData>>();
            List<List<NumberData>> rbIntervals = new List<List<NumberData>>();
            List<List<int>> foundIntervals = new List<List<int>>();

            var structures = new[] {"AVL", "BST", "RB"};

            int chance;
            for (int j = 0; j < structures.Length; j++)
            {
                rng = new Random(seed);
                List<int> insertedValues = new List<int>();
                Console.WriteLine("Current structure " + structures[j]);
                for (int i = 0; i < repetitions; i++)
                {
                    chance = rng.Next(100);
                    if (chance < 40)
                    {
                        var numberItem = new NumberData(InsertedItems[i]);
                        //insert
                            switch (structures[j])
                            {
                                case "BST":
                                    bst.Insert(numberItem);
                                    var foundBST = bst.Find(numberItem);
                                    if(numberItem.Compare(foundBST) != 0)
                                {
                                    throw new Exception(InsertedItems[i] + " not found in BST after insertion");
                                }
                                    
                                    break;
                                case "AVL":
                                    avl.Insert(numberItem);
                                    var foundAVL = avl.Find(numberItem);
                                if (numberItem.Compare(foundAVL) != 0)
                                {
                                    throw new Exception(InsertedItems[i] + " not found in AVL after insertion");
                                }
                                break;
                                case "RB":
                                    rb.Insert(numberItem);
                                    var foundRB = rb.Find(numberItem);
                                if (numberItem.Compare(foundRB) != 0)
                                {
                                    throw new Exception(InsertedItems[i] + " not found in RB after insertion");
                                }
                                break;
                            }
                        
                        Console.WriteLine(InsertedItems[i] + " inserted");
                        insertedValues.Add(InsertedItems[i]);
                    }
                    else if (chance >= 40 && chance < 60)
                    {
                        //delete
                        if (insertedValues.Count == 0)
                        {
                            Console.WriteLine("Tree is empty, nothing to be deleted");
                            continue;
                        }
                        int item = insertedValues[rng.Next(insertedValues.Count)];
                        var numberItem = new NumberData(item);
                        try
                        {
                            switch (structures[j])
                            {
                                case "BST":
                                    bst.Delete(numberItem);
                                    bst.Find(numberItem);
                                    break;
                                case "AVL":
                                    avl.Delete(numberItem);
                                    avl.Find(numberItem);
                                    break;
                                case "RB":
                                    rb.Delete(numberItem);
                                    rb.Find(numberItem);
                                    break;
                            }
                        } catch (Exception ex)
                        {
                            insertedValues.Remove(item);
                            Console.WriteLine(item + " deleted");
                            continue;
                        }
                        Console.WriteLine(item + "in " + structures[j] + "could not be deleted");
                    }
                    else if (chance >= 60 && chance < 80)
                    {
                        //find
                        if(insertedValues.Count == 0)
                        {
                            Console.WriteLine("Tree is empty, nothing to be found");
                            continue;
                        }
                            int item = insertedValues[rng.Next(insertedValues.Count)];
                        NumberData result = null;
                        
                            switch (structures[j])
                            {
                                case "BST":
                                    result = bst.Find(new(item));
                                break;
                                case "AVL":
                                    result = avl.Find(new(item));
                                break;
                                case "RB":
                                    result = rb.Find(new(item));
                                break;
                            }
                        if (result.Value == item)
                        {  
                            Console.WriteLine(item + " found");
                        } else
                        {
                            throw new Exception(item + " not found in " + structures[j]);
                        }
                        
                    }
                    else
                    {
                        if (insertedValues.Count == 0)
                        {
                            Console.WriteLine("Tree is empty, cannot perform interval find");
                            continue;
                        }
                        int a = insertedValues[rng.Next(insertedValues.Count)];
                        int b = insertedValues[rng.Next(insertedValues.Count)];
                        if (a > b)
                        {
                            int temp = a;
                            a = b;
                            b = temp;
                        }
                        var numberMin = new NumberData(a);
                        var numberMax = new NumberData(b);
                        switch (structures[j])
                        {
                            case "BST":
                                var bstFind = bst.FindInterval(numberMin, numberMax);
                                bstIntervals.Add(bstFind);
                                Console.WriteLine("BST Interval from " + a + " to " + b);
                                break;
                            case "AVL":
                                var avlFind = avl.FindInterval(numberMin, numberMax);
                                avlIntervals.Add(avlFind);
                                Console.WriteLine("AVL Interval from " + a + " to " + b);
                                break;
                            case "RB":
                                var rbFind = rb.FindInterval(numberMin, numberMax);
                                rbIntervals.Add(rbFind);
                                Console.WriteLine("RB Interval from " + a + " to " + b);
                                break;
                        }
                        var foundInInterval = insertedValues.Where(x => x >= a && x <= b).ToList();
                        foundIntervals.Add(foundInInterval);

                    }
                    if (structures[j] == "AVL" && i % 10 == 0 && i != 0)
                    {
                        bool isAvl = avl.IsAVL();
                        if(isAvl)
                        {
                            Console.WriteLine("AVL is AVL");
                        } else
                        {
                            throw new Exception("AVL is not AVL");
                        }

                    }
                }
            }
            //go through intervals and compare results
            int intervalsToCheck = foundIntervals.Count / structures.Length;
            for (int i = 0; i < intervalsToCheck; i++)
            {
                var expectedInterval = foundIntervals[i].ToArray().Order();
                var bstInterval = bstIntervals[i].Select(x => x.Value).ToArray();
                var avlInterval = avlIntervals[i].Select(x => x.Value).ToArray();
                var rbInterval = rbIntervals[i].Select(x => x.Value).ToArray();
                bool bstEqual = bstInterval.SequenceEqual(expectedInterval);
                bool avlEqual = avlInterval.SequenceEqual(avlInterval);
                bool rbEqual = rbInterval.SequenceEqual(expectedInterval);

                Console.Write($"Expected interval {i}: ");
                foreach (var val in expectedInterval)
                {
                    Console.Write(val + " ");
                }
                Console.WriteLine();
                
                Console.Write("BST interval: ");
                
                foreach (var val in bstInterval)
                {
                    Console.Write(val + " ");
                }
                Console.WriteLine();
                if (!bstEqual)
                {
                    throw new Exception("BST does not match in interval " + i);
                } else
                {
                    Console.WriteLine("BST found match in interval " + i);
                }

                Console.WriteLine();
                
                Console.Write("AVL interval: ");
                foreach (var val in avlInterval)
                {
                    Console.Write(val + " ");
                }
                Console.WriteLine();
                Console.WriteLine();
                if (!avlEqual)
                {
                    throw new Exception("AVL does not match in interval  " + i);
                }
                else
                {
                    Console.WriteLine("AVL found match in interval " + i);
                }
                Console.WriteLine();
                
                Console.Write("RB interval: ");
                foreach (var val in rbInterval)
                {
                    Console.Write(val + " ");
                }
                Console.WriteLine();
                
                if (!rbEqual)
                {
                    throw new Exception("RB does not match in interval  " + i);
                }
                else
                {
                    Console.WriteLine("RB found match in interval " + i);
                }
                Console.WriteLine();
                



            }


        }
        // --- Small focused test for FindInterval on BST and AVL ---
        public void RunFindIntervalSimpleTest()
        {
            Console.WriteLine("=== FindInterval Simple Test (BST vs AVL) ===");

            // small fixed dataset of 10 values
            int[] values = new[] { 50, 30, 70, 20, 40, 60, 80, 10, 25, 35 };

            var bst = new BST<NumberData>();
            var avl = new AVL<NumberData>();

            foreach (var v in values)
            {
                var d = new NumberData(v);
                if (!bst.Insert(d)) Console.WriteLine($"BST: insert duplicate {v}");
                if (!avl.Insert(new NumberData(v))) Console.WriteLine($"AVL: insert duplicate {v}");
            }

            // intervals to test
            var intervals = new (int min, int max)[]
            {
                (15, 45), // mid-range
                (5, 15),  // lower edge
                (60, 90), // upper edge
                (30, 30), // exact single value
            };
            
            foreach (var (min, max) in intervals)
            {
                Console.WriteLine($"\nInterval [{min}, {max}]:");

                var bstRes = bst.FindInterval(new NumberData(min), new NumberData(max));
                var avlRes = avl.FindInterval(new NumberData(min), new NumberData(max));

                var bstVals = bstRes.Select(x => x.Value).OrderBy(x => x).ToArray();
                var avlVals = avlRes.Select(x => x.Value).OrderBy(x => x).ToArray();

                Console.WriteLine(" BST -> " + (bstVals.Length == 0 ? "(empty)" : string.Join(", ", bstVals)));
                Console.WriteLine(" AVL -> " + (avlVals.Length == 0 ? "(empty)" : string.Join(", ", avlVals)));

                // basic comparison
                bool equal = bstVals.SequenceEqual(avlVals);
                Console.WriteLine(" Match: " + (equal ? "YES" : "NO"));

                if (!equal)
                {
                    var onlyInBst = bstVals.Except(avlVals).ToArray();
                    var onlyInAvl = avlVals.Except(bstVals).ToArray();
                    if (onlyInBst.Length > 0) Console.WriteLine("  Only in BST: " + string.Join(", ", onlyInBst));
                    if (onlyInAvl.Length > 0) Console.WriteLine("  Only in AVL: " + string.Join(", ", onlyInAvl));
                }
            }

            Console.WriteLine("\n=== End of FindInterval Simple Test ===\n");
            
        }


        // --- Per-structure runner that measures ms per phase and writes results to Console ---
        public void RunPerStructureTests(int insertions, int deletions, int finds, int intervalFinds, int maxmins)
        {
            var structures = new[] {"AVL", "BST", "RB"};

            foreach (var structure in structures)
            {
                Console.WriteLine($"--- Running tests for: {structure} ---");

                // Work on a local copy of the input sequence so each structure gets the same input distribution
                var items = new List<int>(InsertedItems);
                Shuffle(items);

                // Local data structures used for this run
                var bst = new BST<NumberData>();
                var avl = new AVL<NumberData>();
                var set = new SortedSet<NumberData>();
                var list = new List<NumberData>();
                var rb = new RBTree<NumberData>();

                // 'present' keeps track of items currently in the structure for finds/intervals
                var present = new List<int>();

                var sw = new Stopwatch();

                // INSERTIONS
                sw.Restart();
                int insertIdx = 0;
                for (int i = 0; i < insertions && insertIdx < items.Count; i++, insertIdx++)
                {
                    var val = items[insertIdx];
                    var data = new NumberData(val);

                    switch (structure)
                    {
                        case "BST":
                            if (!bst.Insert(data)) throw new Exception("BST insert failed");
                            break;
                        case "AVL":
                            if (!avl.Insert(data)) throw new Exception("AVL insert failed");
                            break;
                        case "SortedSet":
                            set.Add(data);
                            break;
                        case "List":
                            list.Add(data);
                            break;
                        case "RB":
                            if (!rb.Insert(data)) throw new Exception("AVL insert failed");
                            break;
                    }

                    present.Add(val);

                    //if (i % 100_000 == 0) Console.WriteLine($"{structure} insertion progress: {i}");
                }
                sw.Stop();
                Console.WriteLine($"{structure} Insertions: {sw.ElapsedMilliseconds} ms");

                // SHUFFLE present for deletion/random access
                Shuffle(present);

                // DELETIONS (delete from the end of 'present' to match original logic)
                sw.Restart();
                for (int i = 0; i < deletions && present.Count > 0; i++)
                {
                    int valueToDelete = present[present.Count - 1];
                    var data = new NumberData(valueToDelete);

                    bool removed = structure switch
                    {
                        "BST" => bst.Delete(data),
                        "AVL" => avl.Delete(data),
                        "SortedSet" => set.Remove(data),
                        "List" => (list.RemoveAll(x => x.Value == valueToDelete) > 0),
                        "RB" => rb.Delete(data),
                        _ => false
                    };

                    if (!removed)
                        throw new Exception($"{structure} delete failed for value {valueToDelete}");

                    present.RemoveAt(present.Count - 1);

                    //if (i % 100_000 == 0) Console.WriteLine($"{structure} deletion progress: {i}");
                }
                sw.Stop();
                Console.WriteLine($"{structure} Deletions: {sw.ElapsedMilliseconds} ms");

                // FINDS
                sw.Restart();
                for (int i = 0; i < finds && present.Count > 0; i++)
                {
                    int idx = rng.Next(0, present.Count);
                    var data = new NumberData(present[idx]);

                    bool found = structure switch
                    {
                        "BST" => bst.Find(data) != null,
                        "AVL" => avl.Find(data) != null,
                        "SortedSet" => set.TryGetValue(data, out var _),
                        "List" => list.Any(x => x.Value == data.Value),
                        "RB" => rb.Find(data) != null,
                        _ => false
                    };

                    if (!found)
                        Console.WriteLine($"Warning: {structure} did not find value {data.Value}");

                    //if (i % 100_000 == 0) Console.WriteLine($"{structure} find progress: {i}");
                }
                sw.Stop();
                Console.WriteLine($"{structure} Finds: {sw.ElapsedMilliseconds} ms");
                
                // INTERVAL FINDS
                sw.Restart();
                for (int i = 0; i < intervalFinds; i++)
                {
                    // choose two values to create an interval
                    int min = rng.Next(present.Count);
                    int max = present[min + 500];
                    min = present[min];
                    var dmin = new NumberData(min);
                    var dmax = new NumberData(max);

                    switch (structure)
                    {
                        case "BST":
                            var bstInterval = bst.FindInterval(dmin, dmax);
                            break;
                        case "AVL":
                            var avlInterval = avl.FindInterval(dmin, dmax);
                            break;
                        case "SortedSet":
                            var setInterval = set.GetViewBetween(dmin, dmax).ToList();
                            break;
                        case "List":
                            var listInterval = list.Where(x => x.Value >= min && x.Value <= max).ToList();
                            break;
                        case "RB":
                            var rbInterval = rb.FindInterval(dmin, dmax);
                            break;
                    }

                    //if (i % 1 == 0) Console.WriteLine($"{structure} interval find progress: {i}");
                }
                sw.Stop();
                Console.WriteLine($"{structure} IntervalFinds: {sw.ElapsedMilliseconds} ms");
                
                // MAX / MIN repeated
                sw.Restart();
                for (int i = 0; i < maxmins; i++)
                {
                    if (present.Count == 0) break;

                    int probe = present[rng.Next(0, present.Count)];
                    var probeData = new NumberData(probe);

                    switch (structure)
                    {
                        case "BST":
                            var bstMax = bst.FindMax(probeData);
                            var bstMin = bst.FindMin(probeData);
                            break;
                        case "AVL":
                            var avlMax = avl.FindMax(probeData);
                            var avlMin = avl.FindMin(probeData);
                            break;
                        case "SortedSet":
                            var setMax = set.Max;
                            var setMin = set.Min;
                            break;
                        case "List":
                            var listMax = list.MaxBy(x => x.Value);
                            var listMin = list.MinBy(x => x.Value);
                            break;
                        case "RB":
                            var rbMax = rb.FindMax(probeData);
                            var rbMin = rb.FindMin(probeData);
                            break;
                    }

                    //if (i % 100_000 == 0) Console.WriteLine($"{structure} max/min progress: {i}");
                }
                sw.Stop();
                Console.WriteLine($"{structure} Max/Min: {sw.ElapsedMilliseconds} ms");

                Console.WriteLine($"--- Finished tests for: {structure} ---\n");
            }
        }
    }
}
