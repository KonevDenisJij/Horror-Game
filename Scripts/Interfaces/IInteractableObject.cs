public interface IInteractableObject
{
    string InteractText { get; }
    string InteractionSignalName { get; set; }

    void Interact();
}