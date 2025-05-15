// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var userInput = "(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)";

// Validate the input string
ValidateInput(userInput);

// Parse the input string
var parsedInput = ParseField(userInput.Replace(" ", ""));

Console.WriteLine("Would you like the user attributes to be printed in alphabetical order? (y/n)");
var response = Console.ReadLine()?.ToLower();

if (response == "y")
{
    PrintTreeAlphabetically(parsedInput);
}
else if (response == "n")
{
    PrintTree(parsedInput);
}
else
{
    Console.WriteLine("Invalid response. Printing user attributes in the order they were entered:");
    PrintTree(parsedInput);
}
static void ValidateInput(string input)
{
    // Check if empty
    if (string.IsNullOrWhiteSpace(input))
    {
        throw new ArgumentException("Input string cannot be null or empty.");
    }

    // Check for balanced parentheses
    int openParentheses = 0;
    int closeParentheses = 0;

    foreach (char c in input)
    {
        if (c == '(') openParentheses++;
        else if (c == ')') closeParentheses++;
    }

    if (openParentheses != closeParentheses)
    {
        throw new ArgumentException("Mismatched parentheses in input.");
    }
    
    // Check for invalid characters
    foreach (char c in input)
    {
        if (!char.IsLetterOrDigit(c) && c != '(' && c != ')' && c != ',' && !char.IsWhiteSpace(c))
        {
            throw new ArgumentException($"Invalid character '{c}' in input string.");
        }
    }
}

static UserAttributeNode ParseField(string userInput)
{
    // Set the parent node
    var user = new UserAttributeNode("user", 0);
    var stack = new Stack<UserAttributeNode>();
    stack.Push(user);
    
    // Split the input string into tokens using regex to parse user attributes
    var pattern = @"([a-zA-Z0-9]+)|([()])";
    var tokens = Regex.Matches(userInput, pattern).Select(m => m.Value).ToList();

    foreach (var token in tokens)
    {
        // If the token is an attribute name, create a new child node
        if (token != "(" && token != ")")
        {
            var currentNode = stack.Peek();
            var newNode = new UserAttributeNode(token, currentNode.Depth + 1);
            currentNode.Children.Add(newNode);
        }
        // If the token is an (, push the current node into the stack and set HasChildren to true for the parent node
        else if (token == "(")
        {
            var currentNode = stack.Peek();
            if (currentNode.Children.Count > 0)
            {
                currentNode.HasChildren = true;
                var newParent = currentNode.Children.Last();
                stack.Push(newParent);
            }

        }
        // If the token is a ), pop the last node from the stack
        else if (token == ")")
        {
            // Pop out as long as not at root level
            if (stack.Count > 1)
            {
                stack.Pop();
            }
        }
    }
    return user;
}

static void PrintTree(UserAttributeNode node)
{
    // Print the nodes, excluding the root node
    foreach (var child in node.Children)
    {
        // Use the depth to indent the attributes
        Console.WriteLine($"{new string(' ', child.Depth * 2)}- {child.Name}");
        PrintTree(child);
    }
}
    
static void PrintTreeAlphabetically(UserAttributeNode node)
{
    // Print the nodes alphabetically, excluding the root node
   foreach (var child in node.Children.OrderBy(c => c.Name))
    {
        // Use the depth to indent the attributes
        Console.WriteLine($"{new string(' ', child.Depth * 2)}- {child.Name}");
        PrintTreeAlphabetically(child);
    }
}

public class UserAttributeNode(string name, int depth)
{
    public string Name { get; set; } = name;
    public int Depth { get; set; } = depth;
    public bool HasChildren { get; set; } = false;
    public List<UserAttributeNode> Children { get; set; } = new List<UserAttributeNode>();
}