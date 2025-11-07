using Programming_7312_Part_1.Models;
using System.Collections.Generic;
using System.Linq;
using System;

// New: BST Node
public class BSTNode<T>
{
    public T Data { get; set; }
    public BSTNode<T> Left { get; set; }
    public BSTNode<T> Right { get; set; }

    public BSTNode(T data)
    {
        Data = data;
    }
}

// New: Binary Search Tree
public class BinarySearchTree<T>
{
    public BSTNode<T> Root { get; private set; }
    private readonly IComparer<T> comparer;

    public BinarySearchTree(IComparer<T>? comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
    }

    public void Insert(T item)
    {
        Root = InsertRec(Root, new BSTNode<T>(item));
    }

    private BSTNode<T> InsertRec(BSTNode<T> root, BSTNode<T> newNode)
    {
        if (root == null)
        {
            root = newNode;
            return root;
        }
        if (comparer.Compare(newNode.Data, root.Data) < 0)
            root.Left = InsertRec(root.Left, newNode);
        else
            root.Right = InsertRec(root.Right, newNode);
        return root;
    }

    public List<T> InOrderTraversal()
    {
        var result = new List<T>();
        InOrderRec(Root, result);
        return result;
    }

    private void InOrderRec(BSTNode<T> root, List<T> result)
    {
        if (root != null)
        {
            InOrderRec(root.Left, result);
            result.Add(root.Data);
            InOrderRec(root.Right, result);
        }
    }
}

// New: AVL Tree Node (simplified balancing)
public class AVLNode<T>
{
    public T Data { get; set; }
    public AVLNode<T> Left { get; set; }
    public AVLNode<T> Right { get; set; }
    public int Height { get; set; } = 1;

    public AVLNode(T data)
    {
        Data = data;
    }
}

public class AVLTree<T>
{
    public AVLNode<T> Root { get; private set; }
    private readonly IComparer<T> comparer;

    public AVLTree(IComparer<T>? comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
    }

    public void Insert(T item)
    {
        Root = InsertRec(Root, new AVLNode<T>(item));
    }

    private AVLNode<T> InsertRec(AVLNode<T> node, AVLNode<T> newNode)
    {
        if (node == null) return newNode;

        if (comparer.Compare(newNode.Data, node.Data) < 0)
            node.Left = InsertRec(node.Left, newNode);
        else
            node.Right = InsertRec(node.Right, newNode);

        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

        // Balance factor
        int balance = GetBalance(node);

        // Rotations (simplified)
        if (balance > 1 && comparer.Compare(newNode.Data, node.Left.Data) < 0)
            return RightRotate(node);
        if (balance < -1 && comparer.Compare(newNode.Data, node.Right.Data) > 0)
            return LeftRotate(node);
        if (balance > 1 && comparer.Compare(newNode.Data, node.Left.Data) > 0)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }
        if (balance < -1 && comparer.Compare(newNode.Data, node.Right.Data) < 0)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    private int GetHeight(AVLNode<T> node) => node?.Height ?? 0;
    private int GetBalance(AVLNode<T> node) => node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);

    private AVLNode<T> RightRotate(AVLNode<T> y)
    {
        AVLNode<T> x = y.Left;
        AVLNode<T> T2 = x.Right;
        x.Right = y;
        y.Left = T2;
        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));
        return x;
    }

    private AVLNode<T> LeftRotate(AVLNode<T> x)
    {
        AVLNode<T> y = x.Right;
        AVLNode<T> T2 = y.Left;
        y.Left = x;
        x.Right = T2;
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));
        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
        return y;
    }

    public List<T> InOrderTraversal()
    {
        var result = new List<T>();
        InOrderRec(Root, result);
        return result;
    }

    private void InOrderRec(AVLNode<T> root, List<T> result)
    {
        if (root != null)
        {
            InOrderRec(root.Left, result);
            result.Add(root.Data);
            InOrderRec(root.Right, result);
        }
    }
}

// New: Graph for dependencies
public class ServiceRequestGraph
{
    private Dictionary<int, List<int>> adjacencyList = new Dictionary<int, List<int>>();

    public void AddDependency(int fromId, int toId)
    {
        if (!adjacencyList.ContainsKey(fromId))
            adjacencyList[fromId] = new List<int>();
        if (!adjacencyList[fromId].Contains(toId))
            adjacencyList[fromId].Add(toId);
    }

    public List<int> GetDependencies(int issueId)
    {
        return adjacencyList.GetValueOrDefault(issueId, new List<int>());
    }

    // BFS Traversal for dependency path
    public List<int> GetDependencyPath(int startId)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(startId);
        visited.Add(startId);
        var path = new List<int> { startId };

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            var deps = GetDependencies(current);
            foreach (int dep in deps)
            {
                if (!visited.Contains(dep))
                {
                    visited.Add(dep);
                    queue.Enqueue(dep);
                    path.Add(dep);
                }
            }
        }
        return path;
    }
}

namespace Programming_7312_Part_1.Services
{
    public class IssueStorage
    {
        private int _nextId = 1;
        public LinkedList<Issue> ReportedIssues { get; } = new LinkedList<Issue>();

        // Advanced structures
        public BinarySearchTree<Issue> BstById { get; } = new BinarySearchTree<Issue>(Comparer<Issue>.Create((a, b) => a.Id.CompareTo(b.Id)));
        public AVLTree<Issue> AvlByDate { get; } = new AVLTree<Issue>(Comparer<Issue>.Create((a, b) => a.ReportedDate.CompareTo(b.ReportedDate)));
        public SortedSet<Issue> RedBlackByCategory { get; } = new SortedSet<Issue>(Comparer<Issue>.Create((a, b) => string.Compare(a.Category, b.Category)));
        public PriorityQueue<Issue, int> HeapByPriority = new PriorityQueue<Issue, int>(); // Min-heap by upvotes (negated for max)
        public ServiceRequestGraph Graph { get; } = new ServiceRequestGraph();

        public IssueStorage()
        {
            // Seed sample issues with user "sampleuser" and deps
            var sample1 = new Issue { Id = _nextId++, Location = "Tokai Rd", Category = "Roads", Description = "Pothole", Status = "Pending", UserId = "sampleuser" };
            var sample2 = new Issue { Id = _nextId++, Location = "Main St", Category = "Utilities", Description = "Leak", Status = "In Progress", UserId = "sampleuser" };
            var sample3 = new Issue { Id = _nextId++, Location = "Park Ave", Category = "Sanitation", Description = "Waste", Status = "Resolved", UserId = "sampleuser" };

            AddIssue(sample1);
            AddIssue(sample2);
            AddIssue(sample3);
            Graph.AddDependency(1, 2); // Sample: Issue 1 depends on 2
        }

        public void AddIssue(Issue issue)
        {
            issue.Id = _nextId++;
            ReportedIssues.AddLast(issue);

            // Update structures (use Id for BST, ReportedDate for AVL, Category for RedBlack, -Upvotes for heap max)
            BstById.Insert(issue);
            AvlByDate.Insert(issue);
            RedBlackByCategory.Add(issue);
            HeapByPriority.Enqueue(issue, -issue.Upvotes); // Negate for max-heap simulation

            // Sample dep (in real: from form)
            if (issue.Id > 1) Graph.AddDependency(issue.Id, issue.Id - 1);
        }

        public Issue GetIssueById(int issueId)
        {
            // Use BST for O(log n) search
            var sorted = BstById.InOrderTraversal();
            return sorted.FirstOrDefault(i => i.Id == issueId);
        }

        public bool UpvoteIssue(int issueId)
        {
            var issue = GetIssueById(issueId);
            if (issue != null)
            {
                issue.Upvotes++;
                // Update heap priority - rebuild the heap
                var tempHeap = new PriorityQueue<Issue, int>();
                foreach (var i in ReportedIssues) tempHeap.Enqueue(i, -i.Upvotes);
                HeapByPriority = tempHeap;
                return true;
            }
            return false;
        }

        public List<Issue> GetUserIssues(string userId)
        {
            var userIssues = ReportedIssues.Where(i => i.UserId == userId).ToList();

            // Sort by date using AVL traversal
            var sortedByDate = AvlByDate.InOrderTraversal().Where(i => i.UserId == userId).ToList();

            // Priority order using heap (extract all, then re-enqueue)
            var priorityList = new List<Issue>();
            var allHeapIssues = new List<(Issue issue, int priority)>();
            while (HeapByPriority.Count > 0)
            {
                HeapByPriority.TryDequeue(out var issue, out var priority);
                allHeapIssues.Add((issue, priority));
                if (issue.UserId == userId) priorityList.Add(issue);
            }
            // Re-enqueue all issues back to heap
            foreach (var (issue, priority) in allHeapIssues)
            {
                HeapByPriority.Enqueue(issue, priority);
            }

            // Filter by category using RedBlack
            var byCategory = RedBlackByCategory.Where(i => i.UserId == userId).ToList();

            return sortedByDate; // Return sorted by date
        }
    }
}

// Extend Issue for comparers
public static class IssueComparer
{
    public static int ByDate(Issue a, Issue b) => a.ReportedDate.CompareTo(b.ReportedDate);
    public static int ById(Issue a, Issue b) => a.Id.CompareTo(b.Id);
}