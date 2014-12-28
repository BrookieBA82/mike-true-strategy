using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class Combinations {

        public static IList<TItem> SelectRandomItemsFromList<TItem>(IList<TItem> items, int minItems, int maxItems) {
            if ((minItems <= maxItems) && (minItems <= items.Count) && (items.Count > 0)) {
                IDictionary<int, long> offsetList = new Dictionary<int, long>();
                long maxOffset = 0;
                int currentItems = 0;
                for (currentItems = minItems; currentItems <= Math.Min(maxItems, items.Count); currentItems++) {
                    if (currentItems > minItems) {
                        offsetList[currentItems - 1] = maxOffset;
                    }
                    maxOffset += ComputeCombination(items.Count, currentItems);
                }

                offsetList[currentItems] = maxOffset;

                int countItems = Math.Min(maxItems, items.Count);
                long selectionIndex = RandomNumberManager.Next(maxOffset);
                for (currentItems = minItems; currentItems < Math.Min(maxItems, items.Count); currentItems++) {
                    if(offsetList[currentItems] > selectionIndex) {
                        countItems = currentItems;
                        break;
                    }
                }

                return SelectRandomItemsFromList<TItem>(items, countItems);
            }

            return new List<TItem>();
        }

        private static IList<TItem> SelectRandomItemsFromList<TItem>(IList<TItem> items, int numItems) {
            IList<TItem> selectedItems = new List<TItem>();
            if ((numItems <= items.Count) && (items.Count > 0) && (numItems > 0)) {
                IList<TItem> remainingItems = new List<TItem>(items);
                for (int selectionNumber = 1; selectionNumber <= numItems; selectionNumber++) {
                    int selectedIndex = RandomNumberManager.Next(remainingItems.Count);
                    TItem selectedItem = remainingItems[selectedIndex];
                    remainingItems.RemoveAt(selectedIndex);
                    selectedItems.Add(selectedItem);
                }
            }

            return selectedItems;
        }

        private static long ComputeCombination(int options, int selections) {
            if (options < selections) {
                return 0;
            }

            List<int> optionList = new List<int>();
            List<int> selectionList = new List<int>();

            for (int selection = 1; selection <= selections; selection++) {
                optionList.Add(options - selection + 1);
                selectionList.Add(selection);
            }

            bool factorFound;
            do {
                factorFound = false;
                for (int selectionIndex = 0; selectionIndex < selectionList.Count && !factorFound; selectionIndex++) {
                    for (int optionIndex = optionList.Count - 1; optionIndex > 0 && !factorFound; optionIndex--) {
                        int option = optionList[optionIndex];
                        int selection = selectionList[selectionIndex];

                        if ((option % selection == 0) && (option / selection >= 1)) {
                            int result = option / selection;
                            optionList.RemoveAt(optionIndex);
                            selectionList.RemoveAt(selectionIndex);
                            optionList.Add(result);
                            optionList = new List<int>(optionList.OrderByDescending<int, int>(p => p));
                            factorFound = true;
                        }
                    }
                }

            } while (factorFound);

            decimal retValue = 1.0m;
            for (int index = 0; index < Math.Max(optionList.Count, selectionList.Count); index++) {
                if (index < optionList.Count) {
                    retValue *= optionList[index];
                }
                if (index < selectionList.Count) {
                    retValue /= selectionList[index];
                }
            }

            return (long) Math.Round(retValue, 0);
        }
    }
}
