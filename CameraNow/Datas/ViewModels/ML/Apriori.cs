namespace Datas.ViewModels.ML
{
    public class Apriori
    {
        public double MinSupport { get; set; }
        public double MinConfidence { get; set; }

        public Apriori(double minSupport = 0.1, double minConfidence = 0.5)
        {
            MinSupport = minSupport;
            MinConfidence = minConfidence;
        }

        public Dictionary<List<string>, double> GetFrequentItemsets(List<List<string>> transactions, int? maxItemsetSize = null)
        {
            var frequentItemsets = new Dictionary<List<string>, double>();
            var candidateItemsets = new Dictionary<List<string>, int>();

            // Lấy tất cả các item duy nhất
            var uniqueItems = transactions.SelectMany(t => t).Distinct().ToList();

            // Khởi tạo với itemsets kích thước 1
            foreach (var item in uniqueItems)
            {
                var itemset = new List<string> { item };
                var support = CalculateSupport(itemset, transactions);
                if (support >= MinSupport)
                {
                    frequentItemsets.Add(itemset, support);
                }
            }

            // Tiếp tục với các itemsets lớn hơn
            int k = 2;
            while (true)
            {
                if (maxItemsetSize.HasValue && k > maxItemsetSize.Value)
                    break;

                var candidateKItemsets = GenerateCandidateItemsets(
                    frequentItemsets.Where(f => f.Key.Count == k - 1).Select(f => f.Key).ToList(), k);

                if (candidateKItemsets.Count == 0)
                    break;

                candidateItemsets.Clear();

                foreach (var transaction in transactions)
                {
                    foreach (var candidate in candidateKItemsets)
                    {
                        if (candidate.All(item => transaction.Contains(item)))
                        {
                            if (!candidateItemsets.ContainsKey(candidate))
                                candidateItemsets[candidate] = 0;
                            candidateItemsets[candidate]++;
                        }
                    }
                }

                foreach (var itemset in candidateItemsets)
                {
                    var support = (double)itemset.Value / transactions.Count;
                    if (support >= MinSupport)
                    {
                        frequentItemsets.Add(itemset.Key, support);
                    }
                }

                if (candidateItemsets.Count == 0)
                    break;

                k++;
            }

            return frequentItemsets;
        }

        public Dictionary<List<string>, List<AssociationRule>> GenerateAssociationRules(Dictionary<List<string>, double> frequentItemsets)
        {
            var rules = new Dictionary<List<string>, List<AssociationRule>>();

            foreach (var itemset in frequentItemsets.Keys.Where(k => k.Count > 1))
            {
                var itemsetSupport = frequentItemsets[itemset];

                for (int i = 1; i < itemset.Count; i++)
                {
                    var antecedentCombinations = GetCombinations(itemset, i);

                    foreach (var antecedent in antecedentCombinations)
                    {
                        var consequent = itemset.Except(antecedent).ToList();
                        var antecedentSupport = frequentItemsets[antecedent];
                        var confidence = itemsetSupport / antecedentSupport;

                        if (confidence >= MinConfidence)
                        {
                            if (!rules.ContainsKey(antecedent))
                                rules[antecedent] = new List<AssociationRule>();

                            rules[antecedent].Add(new AssociationRule
                            {
                                Antecedent = antecedent,
                                Consequent = consequent,
                                Support = itemsetSupport,
                                Confidence = confidence
                            });
                        }
                    }
                }
            }

            return rules;
        }

        private double CalculateSupport(List<string> itemset, List<List<string>> transactions)
        {
            int count = transactions.Count(t => itemset.All(i => t.Contains(i)));
            return (double)count / transactions.Count;
        }

        private List<List<string>> GenerateCandidateItemsets(List<List<string>> frequentItemsets, int k)
        {
            var candidates = new List<List<string>>();

            for (int i = 0; i < frequentItemsets.Count; i++)
            {
                for (int j = i + 1; j < frequentItemsets.Count; j++)
                {
                    var itemset1 = frequentItemsets[i];
                    var itemset2 = frequentItemsets[j];

                    if (itemset1.Take(k - 2).SequenceEqual(itemset2.Take(k - 2)) &&
                        string.Compare(itemset1.Last(), itemset2.Last()) < 0)
                    {
                        var newItemset = itemset1.Union(itemset2).ToList();
                        if (!HasInfrequentSubset(newItemset, frequentItemsets, k - 1))
                        {
                            candidates.Add(newItemset);
                        }
                    }
                }
            }

            return candidates;
        }

        private bool HasInfrequentSubset(List<string> candidate, List<List<string>> frequentItemsets, int k)
        {
            var subsets = GetCombinations(candidate, k);
            return subsets.Any(s => !frequentItemsets.Contains(s));
        }

        private List<List<string>> GetCombinations(List<string> items, int k)
        {
            if (k == 0)
                return new List<List<string>> { new List<string>() };

            if (!items.Any())
                return new List<List<string>>();

            var first = items[0];
            var rest = items.Skip(1).ToList();

            var combinations = new List<List<string>>();

            foreach (var c in GetCombinations(rest, k - 1))
            {
                var newCombination = new List<string> { first };
                newCombination.AddRange(c);
                combinations.Add(newCombination);
            }

            combinations.AddRange(GetCombinations(rest, k));

            return combinations;
        }
    }

    public class AssociationRule
    {
        public List<string> Antecedent { get; set; }
        public List<string> Consequent { get; set; }
        public double Support { get; set; }
        public double Confidence { get; set; }
    }
}
