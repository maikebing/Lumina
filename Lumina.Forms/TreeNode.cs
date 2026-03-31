namespace Lumina.Forms;

/// <summary>
/// Represents a node in a <see cref="TreeView"/>.
/// </summary>
public class TreeNode
{
    /// <summary>
    /// Initializes a node with no text.
    /// </summary>
    public TreeNode()
        : this(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a node with the specified text.
    /// </summary>
    /// <param name="text">The node text.</param>
    public TreeNode(string text)
    {
        Text = text;
        Nodes = new TreeNodeCollection();
    }

    /// <summary>
    /// Initializes a node with the specified text and child nodes.
    /// </summary>
    /// <param name="text">The node text.</param>
    /// <param name="children">The child nodes.</param>
    public TreeNode(string text, TreeNode[] children)
        : this(text)
    {
        Nodes.AddRange(children);
    }

    /// <summary>
    /// Gets or sets the design-time name of the node.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display text of the node.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets the child nodes.
    /// </summary>
    public TreeNodeCollection Nodes { get; }
}

/// <summary>
/// Represents a collection of tree nodes.
/// </summary>
public sealed class TreeNodeCollection : IEnumerable<TreeNode>
{
    private readonly List<TreeNode> _items = [];

    /// <summary>
    /// Adds a node to the collection.
    /// </summary>
    /// <param name="node">The node to add.</param>
    public void Add(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        _items.Add(node);
    }

    /// <summary>
    /// Adds a batch of nodes to the collection.
    /// </summary>
    /// <param name="nodes">The nodes to add.</param>
    public void AddRange(IEnumerable<TreeNode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        foreach (TreeNode node in nodes)
        {
            Add(node);
        }
    }

    /// <summary>
    /// Adds a batch of nodes to the collection.
    /// </summary>
    /// <param name="nodes">The nodes to add.</param>
    public void AddRange(params TreeNode[] nodes)
    {
        AddRange((IEnumerable<TreeNode>)nodes);
    }

    /// <summary>
    /// Gets the number of nodes in the collection.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Gets the node at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    public TreeNode this[int index] => _items[index];

    /// <inheritdoc />
    public IEnumerator<TreeNode> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}