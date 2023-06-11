namespace stringsearch;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PrefixTree
{
    readonly Node root;

    public PrefixTree()
    {
        root = new Node('+', 0, null);
    }

    /// <summary>
    /// Returns all entries in the trie whose first letters match the <paramref name="query"/>.
    /// </summary>
    public async Task<IEnumerable<string>[]> Search(string query)
    {
        var lowestMatchingNode = FindDeepestMatchingNode(query);

        if (lowestMatchingNode.Depth == query.Length)
        {
            return await Task.WhenAll(lowestMatchingNode.Children.Select(child => SearchWorker(query, child)));
        }

        return null;
    }

    /// <summary>
    /// Builds a trie from the entries in <paramref name="items"/>.
    /// </summary>
    public void InsertWordList(IEnumerable<string> items)
    {
        foreach (string item in items)
        {
            var commonPrefix = FindDeepestMatchingNode(item);
            var current = commonPrefix;

            for (var i = current.Depth; i < item.Length; i++)
            {
                var newNode = new Node(item[i], current.Depth + 1, current);
                current.Children.Add(newNode);
                current = newNode;
            }

            current.Children.Add(new Node('$', current.Depth + 1, current));
        }
    }

    /// <summary>
    /// Finds the deepest node whose traversal matches the characters in <paramref name="item"/>.
    /// </summary>
    private Node FindDeepestMatchingNode(string item)
    {
        var currentNode = root;
        foreach (char c in item)
        {
            var nextChild = currentNode.FindChild(c);
            if (nextChild == null)
            {
                break;
            }
            currentNode = nextChild;
        }

        return currentNode;
    }

    /// <summary>
    /// Creates an asynchronous task which collects all search results matching the <paramref name="query"/> under a specific <paramref name="node"/>.
    /// </summary>
    private static async Task<IEnumerable<string>> SearchWorker(string query, Node node)
    {
        var task = new Task<List<string>>(() =>
        {
            var result = new List<string>();
            var suffixList = CollectSuffixOptions(node);
            foreach (var suffix in suffixList)
            {
                result.Add(query + suffix);
            }

            return result;
        });
        task.Start();

        return await task;
    }

    /// <summary>
    /// Collects possible suffixes for any traversal that continues after the node <paramref name="parent"/>.
    /// </summary>
    private static IEnumerable<string> CollectSuffixOptions(Node parent)
    {
        var sb = new StringBuilder();
        sb.Append(parent.Value);
        return RecursiveSuffixCollector(parent, sb).Select(suffix => suffix.TrimEnd('$'));
    }

    /// <summary>
    /// Recursively extends possible suffixes for any traversal that continues after the node <paramref name="parent"/>.
    /// </summary>
    private static IEnumerable<string> RecursiveSuffixCollector(Node parent, StringBuilder current)
    {
        if (parent.Children.Count == 0)
        {
            yield return current.ToString();
        }
        else
        {
            foreach (var child in parent.Children)
            {
                current.Append(child.Value);

                foreach (var value in RecursiveSuffixCollector(child, current))
                {
                    yield return value;
                }

                --current.Length;
            }
        }
    }
}