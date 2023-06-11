using System;
using System.Collections.Generic;
using System.Linq;

namespace stringsearch;

class WordList : List<string>
{

    public WordList(int wordLength = 4)
    {
        this.AddRange(GenerateWordList(wordLength));
    }

    /// <summary>
    /// Returns all entries in WordList whose first letters match the <paramref name="query"/> using PLINQ.
    /// </summary>
    public List<string> Search(string query)
    {
        return this.AsParallel().Where(x => x.StartsWith(query)).ToList();
    }

    /// <summary>
    /// Generates an IEnumerable populated with every possible combination of 4 capital letters in a random order.
    /// </summary>
    private static IEnumerable<string> GenerateWordList(int wordLength)
    {
        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var wordArray = alphabet.Select(x => x.ToString());
        for (int i = 0; i < wordLength - 1; i++)
        {
            wordArray = wordArray.SelectMany(word => alphabet, (word, letter) => word + letter);
        }

        return Shuffle(wordArray);
    }

    /// <summary>
    /// Fisher-Yates-Durstenfeld shuffle to randomly sort an IEnumerable of strings.
    /// </summary>
    private static IEnumerable<string> Shuffle(IEnumerable<string> source)
    {
        var rng = new Random();
        var tempList = source.ToList();
        for (int i = 0; i < tempList.Count; i++)
        {
            int j = rng.Next(i, tempList.Count);
            yield return tempList[j];

            tempList[j] = tempList[i];
        }
    }
}