using UnityEngine;

public class RoomClearHandler : MonoBehaviour
{
    private Room room;
    private EliminationMode eliminationMode;

    
    private void Awake()
    {
     room = GetComponent<Room>();
        eliminationMode = GetComponent<EliminationMode>();
        

    }

    private void OnEnable()
    {
        eliminationMode.onCleared += HandleRoomClear;
    }

    private void HandleRoomClear()
    {
        if (room == null)
            return;

        room.isCleared = true;

        foreach (RoomDoor door in room.doors)
        {
            if (door == null)
                continue;

            door.SetConnected(true);
            door.Open();

            Debug.Log(
                $"{door.name} 연결: {door.IsConnected}, 열림: {door.IsOpen}"
            );
        }
    }


    private void OnDisable()
    {
        eliminationMode.onCleared -= HandleRoomClear;
    }




}
