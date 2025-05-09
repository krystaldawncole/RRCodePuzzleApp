// See https://aka.ms/new-console-template for more information

using System.Text;

string userInput = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";

// Parse the input string
var parsedTree = ParseField(userInput);
PrintTree(parsedTree);

// Parser logic
static FieldNode ParseField(string userInput)
{
    var user = new FieldNode("user", 0);
    var stack = new Stack<FieldNode>();
    stack.Push(user);

    var treeBuilder = new StringBuilder();

    foreach (char c in userInput)
    {
        if (char.IsWhiteSpace(c)) continue;
        if (char.IsLetterOrDigit(c))
        {
            treeBuilder.Append(c);
        }
        else
        {
            if (treeBuilder.Length > 0)
            {
                var fieldName = treeBuilder.ToString();
                var currentNode = stack.Peek();
                var newNode = new FieldNode(fieldName, stack.Peek().Depth + 1);
                currentNode.Children.Add(newNode);
                treeBuilder.Clear();
            }

            if (c == '(')
            {
                var parentNode = stack.Peek();
                parentNode.HasChildren = true;
                var newParent = parentNode.Children.Last();
                stack.Push(newParent);
            }
            else if (c == ')')
            {
                if (stack.Count > 1)
                {
                    stack.Pop();
                }
            }
        }
    }

    if (treeBuilder.Length > 0)
    {
        var fieldName = treeBuilder.ToString();
        var currentNode = stack.Peek();
        var newNode = new FieldNode(fieldName, stack.Peek().Depth + 1);
        currentNode.Children.Add(newNode);
        treeBuilder.Clear();
    }

    return user;
}

// Print the tree structure
static void PrintTree(FieldNode node)
{
    //print the node name starting from the starting depth
    if (node.Depth >= 1)
    {
        Console.WriteLine($"{new string(' ', node.Depth * 2)}- {node.Name}");
    }
    foreach (var child in node.Children)
    {
        PrintTree(child);
    }
}

public class FieldNode
{
    public string Name { get; set; }
    public int Depth { get; set; }
    public bool HasChildren { get; set; }
    public List<FieldNode> Children { get; set; } = new List<FieldNode>();
    public FieldNode(string name, int depth)
    {
        Name = name;
        Depth = depth;
        HasChildren = false;
    }
}