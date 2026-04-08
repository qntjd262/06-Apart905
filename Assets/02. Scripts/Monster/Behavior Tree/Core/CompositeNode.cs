using System.Collections.Generic;

public abstract class CompositeNode : Node
{
    protected Node _currentChild;

    protected List<Node> _children = new List<Node>();

    public CompositeNode(params Node[] children)
    {
        _children.AddRange(children);
    }
    public override void OnStop()
    {
        if (_currentChild != null)
        {
            _currentChild.OnStop();
            _currentChild = null;
        }
    }
}
