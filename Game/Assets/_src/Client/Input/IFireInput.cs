namespace Game.Client.Input
{
    public interface IFireInput
    {
        bool IsPressed();
        bool WasPressedThisFrame();
        bool WasReleasedThisFrame();
    }
}