using Microsoft.Extensions.Caching.Memory;
using Services.Interfaces.Services;
using System.Data;
namespace Services.Services
{
    public class AprioriService
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IMemoryCache _cache;
        private static readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

        public AprioriService(IOrderService orderService, IMemoryCache cache, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _cache = cache;
        }

        public async Task<List<AssociationRule>> GetCachedAssociationRulesAsync(double minConfidence, bool useCartData = false)
        {
            var cacheKey = $"AprioriRules_{minConfidence}";

            if (_cache.TryGetValue(cacheKey, out List<AssociationRule> cachedRules))
            {
                return cachedRules;
            }

            await _cacheLock.WaitAsync();
            try
            {
                // Double-check cache after acquiring lock
                if (_cache.TryGetValue(cacheKey, out cachedRules))
                {
                    return cachedRules;
                }

                // Generate and cache new rules
                var rules = await Task.Run(() => GenerateAssociationRules(minConfidence, useCartData));
                _cache.Set(cacheKey, rules, TimeSpan.FromHours(1)); // Cache for 1 hour
                return rules;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        private async Task<List<HashSet<Guid>>> GetNonEmptySubsetsAsync(List<Guid> items)
        {
            return await Task.Run(() =>
            {
                var result = new List<HashSet<Guid>>();
                int subsetCount = 1 << items.Count;

                for (int i = 1; i < subsetCount - 1; i++)
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
            });
        }


        // Hàm tìm frequent itemsets
        public Dictionary<HashSet<Guid>, int> FindFrequentItemsets(double minSupport, bool useCartData = false)
        {
            List<List<Guid>> transactions;

            if (useCartData)
            {
                transactions = _cartService.GetCartsData();
            }
            else
            {
                transactions = _orderService.GetTransactionData();
            }

            if (transactions.Count == 0)
            {
                return new Dictionary<HashSet<Guid>, int>();
            }

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

        // Hàm tạo luật kết hợp
        public List<AssociationRule> GenerateAssociationRules(double minConfidence, bool useCartData = false)
        {
            var frequentItemsets = FindFrequentItemsets(0.1, useCartData); // minSupport = 10%
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
