
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public DoorDirection direction;

    [Tooltip("Visual shown while the door is closed.")]
    public GameObject closedVisual;

    public Collider2D blockingCollider;

    public bool IsConnected => isConnected;
    public bool IsOpen => isOpen;

    private bool isConnected;
    private bool isOpen;

    public void SetConnected(bool connected)
    {
        isConnected = connected;

        // Connection does not mean the door is open.
        // Every door starts closed.
        CloseDoor();
    }

    public void Open()
    {
        // An unconnected door can never be opened.
        if (!isConnected)
        {
            return;
        }

        isOpen = true;

        if (closedVisual != null)
        {
            closedVisual.SetActive(false);
        }

        if (blockingCollider != null)
        {
            blockingCollider.enabled = false;
        }
    }

    private void CloseDoor()
    {
        isOpen = false;

        if (closedVisual != null)
        {
            closedVisual.SetActive(true);
        }

        if (blockingCollider != null)
        {
            blockingCollider.enabled = true;
        }
    }
}
