using System.Collections.Generic;
using System.Linq;

namespace stringsearch;

public class Node
{
    public char Value { get; set; }
    public List<Node> Children { get; set; }
    public Node Parent { get; set; }
    public int Depth { get; set; }

    public Node(char value, int depth, Node parent)
    {
        Value = value;
        Children = new List<Node>();
        Depth = depth;
        Parent = parent;
    }

    /// <summary>
    /// Returns the child under this node that matches the given character <paramref name="c"/> using LINQ.
    /// </summary>
    public Node FindChild(char c)
    {
        return Children.FirstOrDefault(child => child.Value == c);
    }
}