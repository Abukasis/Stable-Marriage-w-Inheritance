using System;
using System.Collections.Generic;
using System.Linq;
namespace StableMarriage {

    public class Person {
        public int id;
        public List<int> preference;
        public Person(int id, List<int> preference) {
            this.id = id;
            this.preference = preference;
        }
        public int ranking(int id) {
            return preference.IndexOf(id);
        }
    }

    public class Man : Person {
        public List<int> proposedTo;
        public Man(int id, List<int> preference) : base(id, preference) {
            proposedTo = new List<int>();
        }
        public int getNextWomenToProposeTo() {
            foreach (int w in preference)
                if (!proposedTo.Contains(w))
                    return w;
            return -1;
        }
    }

    public class Woman : Person {
        public int marriedTo;
        public Woman(int id, List<int> preference) : base(id, preference) {
            marriedTo = -1;
        }
        public bool wouldSwitchFor(int newMan) {
            if (marriedTo == -1) return true; // first proposal
            if (ranking(marriedTo) > ranking(newMan)) return true;
            else return false;
        }
        public int divorceAndMarry(int newMan) {
            int oldManId = this.marriedTo;
            this.marriedTo = newMan;
            return oldManId;
        }
    }

    class Match {
        public Man man { get; set; }
        public Woman women { get; set; }
        public int getRanking(bool isWoman) {
            if (isWoman) return mansRankingForWomen();
            else return womensRankingForMan();
        }
        private int womensRankingForMan() { return women.ranking(man.id) + 1; }
        private int mansRankingForWomen() { return man.ranking(women.id) + 1; }

        public bool wouldSwitchWith(Match m) {
            if (women.wouldSwitchFor(m.man.id) && m.man.ranking(women.id) < m.mansRankingForWomen())
                return true;
            else return false;
        }

        public string bestDeal() {
            var womanRank = getRanking(true);
            var manRank = getRanking(false);
            if (womanRank < manRank) return "MAN";
            else if (womanRank > manRank) return "WOMAN";
            else return "EQUAL";
        }

        public override string ToString() {
            var output = "";
                output += $"Man:{man.id} Rank:{getRanking(isWoman: false)}, "
                        + $"Women:{women.id} Rank:{getRanking(isWoman: true)}, "
                        + $"Deal={bestDeal()}";
            return output;
        }
    }

    public class Program {

        static void Main(string[] args) {
            Dictionary<int, Man> men = new Dictionary<int, Man> {
                { 0, new Man(0 , new List<int>{ 0 , 3 , 2 , 1 , 4 }) },
                { 1, new Man(1 , new List<int>{ 0 , 3 , 2 , 4 , 1 }) },
                { 2, new Man(2 , new List<int>{ 3 , 4 , 1 , 2 , 0 }) },
                { 3, new Man(3 , new List<int>{ 2 , 1 , 4 , 3 , 0 }) },
                { 4, new Man(4 , new List<int>{ 4 , 2 , 3 , 1 , 0  }) },
            };

            var women = new Dictionary<int, Woman> {
                { 0, new Woman(0 , new List<int>{ 1 , 4 , 3 , 2 , 0 }) },
                { 1, new Woman(1 , new List<int>{ 1 , 3 , 2 , 4 , 0 }) },
                { 2, new Woman(2 , new List<int>{ 2 , 1 , 3 , 4 , 0 }) },
                { 3, new Woman(3 , new List<int>{ 0 , 3 , 2 , 4 , 1 }) },
                { 4, new Woman(4 , new List<int>{ 0 , 2 , 3 , 4 , 1 }) },
            };
            StableMarriage(men, women);
        }

        static public void StableMarriage(Dictionary<int, Man> men, Dictionary<int, Woman> women) {
            List<int> availMen = men.Keys.ToList();
            Dictionary<int, Match> matches = new Dictionary<int, Match>();
            while (availMen.Any()) {
                var m = men[availMen.First()];
                bool manIsProposing = true;
                while (manIsProposing) {
                    var w = women[m.getNextWomenToProposeTo()];
                    m.proposedTo.Add(w.id);
                    if (w.wouldSwitchFor(m.id)) {
                        if (matches.ContainsKey(w.id))
                            matches.Remove(w.id);
                        int oldMan = w.divorceAndMarry(m.id);
                        if (oldMan != -1) availMen.Add(oldMan);
                        availMen.Remove(m.id);
                        matches.Add(w.id, new Match { man = m, women = w });
                        manIsProposing = false;
                    }
                }
            }
            int i = 0;
            foreach (var k in matches.Keys)
                Console.WriteLine($"Match #{++i} : {matches[k].ToString()} \n");
        }
    }
}