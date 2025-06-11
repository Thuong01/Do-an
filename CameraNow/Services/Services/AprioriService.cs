using Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AprioriService
    {
        private readonly IOrderService _orderService;

        public AprioriService(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Hàm tìm frequent itemsets
        public Dictionary<HashSet<Guid>, int> FindFrequentItemsets(double minSupport)
        {
            var transactions = _orderService.GetTransactionData();

            var itemCount = transactions.SelectMany(t => t)
                                        .GroupBy(i => i)
                                        .ToDictionary(g => new HashSet<Guid> { g.Key }, g => g.Count());

            var frequentItemsets = new Dictionary<HashSet<Guid>, int>(HashSetComparer<Guid>.Instance);
            var currentItemsets = itemCount
                                    .Where(kv => (double)kv.Value / transactions.Count >= minSupport)
                                    .ToDictionary(kv => kv.Key, kv => kv.Value);

            while (currentItemsets.Any())
            {
                foreach (var kv in currentItemsets)
                {
                    frequentItemsets[kv.Key] = kv.Value;
                }

                var candidates = GenerateCandidateItemsets(currentItemsets.Keys.ToList());

                currentItemsets = candidates
                            .ToDictionary(
                                candidate => candidate,
                                candidate => transactions.Count(t => candidate.IsSubsetOf(t)),
                                HashSetComparer<Guid>.Instance
                            )
                            .Where(kv => (double)kv.Value / transactions.Count >= minSupport)
                             .ToDictionary(kv => kv.Key, kv => kv.Value, HashSetComparer<Guid>.Instance);
                            }

            return frequentItemsets;
        }

        // Hàm sinh candidate itemsets
        private List<HashSet<Guid>> GenerateCandidateItemsets(List<HashSet<Guid>> frequentItemsets)
        {
            //var candidates = new List<List<Guid>>();
            var candidates = new List<HashSet<Guid>>();

            for (int i = 0; i < frequentItemsets.Count; i++)
            {
                for (int j = i + 1; j < frequentItemsets.Count; j++)
                {
                    var union = frequentItemsets[i].Union(frequentItemsets[j]).ToHashSet();

                    if (union.Count == frequentItemsets[i].Count + 1 && !candidates.Contains(union, HashSetComparer<Guid>.Instance))
                    {
                        candidates.Add(union);
                    }
                }
            }

            return candidates;
        }

        // Kiểm tra subsets có frequent không
        private bool HasInfrequentSubset(List<Guid> candidate, List<List<Guid>> frequentItemsets)
        {
            var subsets = candidate.Select((x, i) => candidate.Where((y, j) => j != i).ToList()).ToList();

            foreach (var subset in subsets)
            {
                if (!frequentItemsets.Any(fi => fi.SequenceEqual(subset)))
                {
                    return true;
                }
            }

            return false;
        }


        // Hàm tạo luật kết hợp
        public List<AssociationRule> GenerateAssociationRules(double minConfidence)
        {
            var frequentItemsets = FindFrequentItemsets(0.1); // minSupport = 10%
            var transactions = _orderService.GetTransactionData();
            var rules = new List<AssociationRule>();

            foreach (var itemset in frequentItemsets.Where(fi => fi.Key.Count > 1))
            {
                //var supportItemset = (double)frequentItemsets[itemset.Key] / transactions.Count;
                var itemsetSupport = (double)frequentItemsets[itemset.Key] / transactions.Count;
                // Sinh tất cả các tập con không rỗng của itemset
                var subsets = GetNonEmptySubsets(itemset.Key.ToList());

                foreach (var antecedent in subsets)
                {
                    var consequent = itemset.Key.Except(antecedent).ToHashSet();

                    if (consequent.Count == 0) continue;

                    if (frequentItemsets.TryGetValue(antecedent, out int antecedentCount))
                    {
                        double confidence = (double)frequentItemsets[itemset.Key] / antecedentCount;

                        if (confidence >= minConfidence)
                        {
                            var rule = new AssociationRule
                            {
                                Antecedent = antecedent.ToList(),
                                Consequent = consequent.ToList(),
                                Support = itemsetSupport,
                                Confidence = confidence
                            };
                            rules.Add(rule);
                        }
                    }

                    // B => A (chiều ngược lại)
                    if (frequentItemsets.TryGetValue(consequent, out int consequentCount))
                    {
                        double reverseConfidence = (double)frequentItemsets[itemset.Key] / consequentCount;

                        if (reverseConfidence >= minConfidence)
                        {
                            var reverseRule = new AssociationRule
                            {
                                Antecedent = consequent.ToList(),
                                Consequent = antecedent.ToList(),
                                Support = itemsetSupport,
                                Confidence = reverseConfidence
                            };
                            rules.Add(reverseRule);
                        }
                    }
                }
            }

            return rules;
        }
        private List<HashSet<Guid>> GetNonEmptySubsets(List<Guid> items)
        {
            var result = new List<HashSet<Guid>>();
            int subsetCount = 1 << items.Count;

            for (int i = 1; i < subsetCount - 1; i++) // bỏ tập rỗng và chính nó
            {
                var subset = new HashSet<Guid>();
                for (int j = 0; j < items.Count; j++)
                {
                    if ((i & (1 << j)) != 0)
                        subset.Add(items[j]);
                }
                result.Add(subset);
            }

            return result;
        }


        private List<string[]> GenerateCandidateItemSets(List<string[]> frequentItemSets, int k)
        {
            var candidates = new List<string[]>();

            for (int i = 0; i < frequentItemSets.Count; i++)
            {
                for (int j = i + 1; j < frequentItemSets.Count; j++)
                {
                    var merged = frequentItemSets[i].Take(k - 2)
                        .SequenceEqual(frequentItemSets[j].Take(k - 2))
                        ? frequentItemSets[i].Concat(new[] { frequentItemSets[j].Last() }).OrderBy(x => x).ToArray()
                        : null;

                    if (merged != null && HasAllSubsets(frequentItemSets, merged, k - 1))
                        candidates.Add(merged);
                }
            }

            return candidates;
        }

        private bool HasAllSubsets(List<string[]> frequentItemSets, string[] candidate, int subsetSize)
        {
            var subsets = GetSubsets(candidate, subsetSize);
            return subsets.All(subset => frequentItemSets.Any(fs => fs.SequenceEqual(subset)));
        }

        private List<string[]> GetSubsets(string[] itemSet, int size)
        {
            var subsets = new List<string[]>();
            GenerateSubsets(itemSet, size, 0, new string[size], subsets);
            return subsets;
        }

        private void GenerateSubsets(string[] itemSet, int size, int start, string[] current, List<string[]> subsets)
        {
            if (size == 0)
            {
                subsets.Add(current.ToArray());
                return;
            }

            for (int i = start; i <= itemSet.Length - size; i++)
            {
                current[current.Length - size] = itemSet[i];
                GenerateSubsets(itemSet, size - 1, i + 1, current, subsets);
            }
        }
    }

    public class AssociationRule
    {
        public List<Guid> Antecedent { get; set; }
        public List<Guid> Consequent { get; set; }
        public double Support { get; set; }
        public double Confidence { get; set; }
    }

    public class ItemSetEqualityComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] x, string[] y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(string[] obj)
        {
            return obj.Aggregate(0, (hash, item) => hash ^ item.GetHashCode());
        }
    }

    public class HashSetComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public static readonly HashSetComparer<T> Instance = new HashSetComparer<T>();

        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            return x.SetEquals(y);
        }

        public int GetHashCode(HashSet<T> obj)
        {
            int hash = 19;
            foreach (var item in obj.OrderBy(i => i))
                hash = hash * 31 + item.GetHashCode();
            return hash;
        }
    }
}
