using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    protected List<Node> _children = new List<Node>();

    public CompositeNode(params Node[] children)
    {
        _children.AddRange(children);
    }
}
