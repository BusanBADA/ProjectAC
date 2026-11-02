using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Header("Room local points")]
    public Transform playerSpawnPoint;
    public RoomDoor[] doors;

    [Header("Room Tilemap")]
    [SerializeField] private Tilemap floorTilemap;

    [Header("Optional gameplay hooks")]
    public bool lockDoorsOnEnter;
    public bool isCleared;

    private readonly Dictionary<DoorDirection, RoomDoor> doorByDirection = new Dictionary<DoorDirection, RoomDoor>();

    private void Awake()
    {
        RebuildDoorLookup();
    }

    public void RebuildDoorLookup()
    {
        doorByDirection.Clear();

        if (doors == null) return;

        foreach (RoomDoor door in doors)
        {
            if (door == null) continue;
            doorByDirection[door.direction] = door;
        }
    }

    public RoomDoor GetDoor(DoorDirection direction)
    {
        if (doorByDirection.Count == 0)
        {
            RebuildDoorLookup();
        }

        doorByDirection.TryGetValue(direction, out RoomDoor result);
        return result;
    }

    public void SetDoorConnected(DoorDirection direction, bool connected)
    {
        RoomDoor door = GetDoor(direction);

        if (door != null)
        {
            door.SetConnected(connected);
        }
    }

    public void OpenDoor(DoorDirection direction)
    {
        GetDoor(direction)?.Open();
    }

    public Vector3 GetPlayerSpawnPosition()
    {
        return playerSpawnPoint != null ? playerSpawnPoint.position : transform.position;
    }

    // Tilemap에서 실제 방 크기를 계산
    public Vector2 GetRoomSize()
    {
        if (floorTilemap == null)
        {
            Debug.LogError($"{name}: Floor Tilemap이 연결되지 않았습니다.");
            return Vector2.zero;
        }

        BoundsInt bounds = floorTilemap.cellBounds;

        if (bounds.size.x <= 0 || bounds.size.y <= 0)
        {
            Debug.LogError($"{name}: Floor Tilemap에 타일이 없습니다.");
            return Vector2.zero;
        }

        Vector3 localWidth = floorTilemap.transform.TransformVector(Vector3.right * bounds.size.x * floorTilemap.cellSize.x);

        Vector3 localHeight = floorTilemap.transform.TransformVector(Vector3.up * bounds.size.y * floorTilemap.cellSize.y);

        return new Vector2(localWidth.magnitude,localHeight.magnitude);
    }
}