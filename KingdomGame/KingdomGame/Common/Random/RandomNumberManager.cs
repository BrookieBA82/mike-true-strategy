using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public interface IRandomNumberGenerator {

        int Next(int maxValue);

        long Next(long maxValue);

    }

    public class RandomNumberGenerator : IRandomNumberGenerator {

        Random _random = new Random();

        public int Next(int maxValue) {
            return _random.Next(maxValue);
        }

        public long Next(long maxValue) {
            return (long) (_random.NextDouble() * maxValue);
        }
    }

    public class ScriptedNumberGenerator : IRandomNumberGenerator {

        int _index;
        IList<decimal> _percentageList;

        public ScriptedNumberGenerator(IList<decimal> percentageList) {
            _index = 0;
            _percentageList = new List<decimal>(percentageList);
        }

        public int Next(int maxValue) {
            decimal percentage = _percentageList[_index];
            _index = (_index + 1) % _percentageList.Count;

            return (int) Math.Round(percentage * maxValue);
        }

        public long Next(long maxValue) {
            decimal percentage = _percentageList[_index];
            _index = (_index + 1) % _percentageList.Count;

            return (long) Math.Round(percentage * maxValue);
        }
    }

    public class RandomNumberManager {

        private static IRandomNumberGenerator Generator;

        static RandomNumberManager() {
            RandomNumberManager.Generator = new RandomNumberGenerator();
        }

        public static void SetRandomNumberGenerator(IRandomNumberGenerator generator) {
            RandomNumberManager.Generator = generator;
        }

        public static int Next(int maxValue) {
            return Generator.Next(maxValue);
        }

        public static long Next(long maxValue) {
            return Generator.Next(maxValue);
        }
    }
}
