namespace stringsearch;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var wordList = new WordList();

        var trie = new PrefixTree();
        Stopwatch trieInsertionStopwatch = Stopwatch.StartNew();
        trie.InsertWordList(wordList);
        trieInsertionStopwatch.Stop();

        Console.WriteLine("Type a prefix and press return.");

        while (true)
        {
            var query = Console.ReadLine()?.ToUpper();
            Console.WriteLine();

            if (string.IsNullOrEmpty(query))
            {
                continue;
            }

            Stopwatch plinqStopwatch = Stopwatch.StartNew();
            var plinqResult = wordList.Search(query);
            plinqStopwatch.Stop();

            Stopwatch trieStopwatch = Stopwatch.StartNew();
            var trieResult = await trie.Search(query);
            trieStopwatch.Stop();

            if (trieResult?.SelectMany(x => x).ToList().Count > 0)
            {
                foreach (var word in trieResult.SelectMany(x => x))
                {
                    Console.WriteLine(word);
                }
                Console.WriteLine($"\nPLINQ search time: {plinqStopwatch.ElapsedMilliseconds} ms / {plinqStopwatch.ElapsedTicks} ticks");
                Console.WriteLine($"Trie search time: {trieStopwatch.ElapsedMilliseconds} ms / {trieStopwatch.ElapsedTicks} ticks (+ {trieInsertionStopwatch.ElapsedMilliseconds} ms for insertion)\n");
            }
            else
            {
                Console.WriteLine("Prefix not found.\n");
            }
        }
    }
}
