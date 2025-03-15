using Godot;

public partial class BaseNode : Node
{
	protected T GetNodeWithError<T>(NodePath path, string nodeName) where T : Node
	{
		T node = GetNode<T>(path);
		if (node == null)
		{
			GD.PrintErr($"{nodeName} node not found at path: {path}");
		}
		return node;
	}
}
