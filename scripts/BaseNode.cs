using Godot;

public static class NodeUtils
{
	public static T FindNodeWithError<T>(Node parent, NodePath path, string nodeName) where T : Node
	{
		T node = parent.GetNode<T>(path);
		if (node == null)
		{
			GD.PrintErr($"{nodeName} node not found at path: {path}");
		}
		return node;
	}
}
