public abstract class BehaviorNode
{
    public abstract bool Evaluate();
}

public class SelectorNode : BehaviorNode
{
    private BehaviorNode[] children;

    public SelectorNode(params BehaviorNode[] nodes) => children = nodes;

    public override bool Evaluate()
    {
        foreach (var node in children)
        {
            if (node.Evaluate())
                return true;
        }
        return false;
    }
}

public class SequenceNode : BehaviorNode
{
    private BehaviorNode[] children;

    public SequenceNode(params BehaviorNode[] nodes) => children = nodes;

    public override bool Evaluate()
    {
        foreach (var node in children)
        {
            if (!node.Evaluate())
                return false;
        }
        return true;
    }
}

public class ConditionNode : BehaviorNode
{
    private System.Func<bool> condition;

    public ConditionNode(System.Func<bool> condition) => this.condition = condition;

    public override bool Evaluate() => condition.Invoke();
}

public class ActionNode : BehaviorNode
{
    private System.Action action;

    public ActionNode(System.Action action) => this.action = action;

    public override bool Evaluate()
    {
        action.Invoke();
        return true;
    }
}
