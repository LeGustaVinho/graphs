# Graphs Library
A comprehensive C# library for creating and manipulating graph and tree data structures. This library provides flexible and efficient implementations of graphs and trees with support for directed, undirected, cyclic, acyclic, and hierarchical structures.

## Features
- **Graph Support:** Create and manipulate graphs with nodes and connections.
- **Tree Support:** Specialized tree structures with parent-child relationships.
- **Directed and Undirected Graphs:** Support for both unidirectional and bidirectional connections.
- **Cycle Detection:** Check for cyclic and acyclic graphs.
- **Hierarchy Management:** Support for parent and child graphs.
- **Traversal Algorithms:** Depth-first and breadth-first (height-first) traversal methods.
- **Visualization:** Simple visualization methods for trees.

## How to install

### - From OpenUPM:

- Open **Edit -> Project Settings -> Package Manager**
- Add a new Scoped Registry (or edit the existing OpenUPM entry)

| Name  | package.openupm.com  |
| ------------ | ------------ |
| URL  | https://package.openupm.com  |
| Scope(s)  | com.legustavinho  |

- Open **Window -> Package Manager**
- Click `+`
- Select `Add package from git URL...`
- Paste `com.legustavinho.legendary-tools-graphs` and click `Add`

## Usage
### Graphs
#### Creating Nodes and Graphs

```csharp
// Create nodes
INode nodeA = new Node();
INode nodeB = new Node();
INode nodeC = new Node();

// Create a graph
IGraph graph = new Graph();

// Add nodes to the graph
graph.Add(nodeA);
graph.Add(nodeB);
graph.Add(nodeC);
```
#### Connecting Nodes
```csharp
// Connect nodes
nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
nodeB.ConnectTo(nodeC, NodeConnectionDirection.Bidirectional);
```
#### Checking for Cycles
```csharp
// Check if the graph is cyclic
bool isCyclic = graph.IsCyclic;

// Check if the graph is acyclic
bool isAcyclic = graph.IsAcyclic;
```

#### Traversing the Graph

```csharp
// Get all nodes in the graph
INode[] allNodes = graph.AllNodes;

// Get neighbours of a node
INode[] neighboursOfA = graph.Neighbours(nodeA);
```
#### Example: Building and Analyzing a Graph

```csharp
// Create nodes
INode node1 = new Node();
INode node2 = new Node();
INode node3 = new Node();

// Create a graph and add nodes
IGraph myGraph = new Graph();
myGraph.Add(node1);
myGraph.Add(node2);
myGraph.Add(node3);

// Connect nodes
node1.ConnectTo(node2, NodeConnectionDirection.Unidirectional);
node2.ConnectTo(node3, NodeConnectionDirection.Unidirectional);
node3.ConnectTo(node1, NodeConnectionDirection.Unidirectional); // Creates a cycle

// Check for cycles
if (myGraph.IsCyclic)
{
    Console.WriteLine("The graph contains a cycle.");
}
else
{
    Console.WriteLine("The graph is acyclic.");
}
```

### Trees

#### Creating Tree Nodes and Trees

```csharp
// Create tree nodes
ITreeNode rootNode = new TreeNode();
ITreeNode childNode1 = new TreeNode();
ITreeNode childNode2 = new TreeNode();

// Create a tree
ITree tree = new Tree();

// Add the root node to the tree
tree.AddTreeNode(rootNode, null);

// Add child nodes to the tree
tree.AddTreeNode(childNode1, rootNode);
tree.AddTreeNode(childNode2, rootNode);
```

#### Removing Nodes

```csharp
// Remove a node from the tree
ITreeNode[] removedNodes;
tree.RemoveTreeNode(childNode1, out removedNodes);
```

#### Traversing the Tree

```csharp
// Depth-first traversal
List<ITreeNode> depthFirstNodes = tree.DepthFirstTraverse();

// Height-first (breadth-first) traversal
List<ITreeNode> heightFirstNodes = tree.HeightFirstTraverse();
```

#### Searching the Tree

```csharp
// Depth-first search
ITreeNode foundNode = tree.DepthFirstSearch(node => node.Id == "some-id");

// Height-first search
ITreeNode foundNode = tree.HeightFirstSearch(node => node.Id == "some-id");
```

#### Visualizing the Tree

```csharp
// Visualize the tree structure
tree.VisualizeTree(showDepth: true);
```

#### Example: Building and Visualizing a Tree

```csharp
// Create tree nodes
ITreeNode root = new TreeNode();
ITreeNode child1 = new TreeNode();
ITreeNode child2 = new TreeNode();
ITreeNode grandChild = new TreeNode();

// Create a tree and add nodes
ITree myTree = new Tree();
myTree.AddTreeNode(root, null); // Root node
myTree.AddTreeNode(child1, root);
myTree.AddTreeNode(child2, root);
myTree.AddTreeNode(grandChild, child1);

// Visualize the tree
myTree.VisualizeTree();

// Output:
// B-Tree
// └── [Depth: 0] [Id: root-node-id]
//     ├── [Depth: 1] [Id: child1-node-id]
//     │   └── [Depth: 2] [Id: grandChild-node-id]
//     └── [Depth: 1] [Id: child2-node-id]
```

## License
This project is licensed under the MIT License.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue for any bugs or feature requests.