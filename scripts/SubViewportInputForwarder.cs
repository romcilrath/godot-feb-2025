using Godot;

public partial class SubViewportInputForwarder : TextureRect
{
    private SubViewport subViewport;

    public override void _Ready()
    {
        subViewport = GetNode<SubViewport>("SubViewport");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            Vector2 localPos = GetLocalMousePosition();
            Vector2 viewportSize = subViewport.Size;
            Vector2 textureSize = Size;

            // Convert to SubViewport coordinates
            Vector2 subViewportPos = new Vector2(
                (localPos.X / textureSize.X) * viewportSize.X,
                (localPos.Y / textureSize.Y) * viewportSize.Y
            );

            // Clone the event and set new position
            InputEventMouse newEvent = (InputEventMouse)@event.Duplicate();
            newEvent.Position = subViewportPos;

            // Push the transformed input event to the SubViewport
            subViewport.PushInput(newEvent, true);
        }
    }
}
