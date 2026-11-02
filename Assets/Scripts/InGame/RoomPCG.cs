
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a fixed room graph once per stage.
/// Each room occupies one square grid cell.
/// </summary>
public class RoomPCG : MonoBehaviour
{
    [Header("Room definitions")]
    private readonly List<RoomDefinition> roomDefinitions = new List<RoomDefinition>();

    private const string ROOM_DEFINITION_PATH = "RoomDefinitions";

    [Header("Generation")]
    [SerializeField, Min(1)] private int targetRoomCount = 12;
    [SerializeField, Min(0)] private int seed = 12345;

    [Header("Grid to world")]
    [SerializeField] private Vector2 roomStep = new Vector2(20f, 20f);
    [SerializeField] private Transform roomsRoot;

    public int Seed => seed;
    public IReadOnlyList<Room> SpawnedRooms => spawnedRooms;
    public Room StartRoom { get; private set; }

    private readonly List<Room> spawnedRooms = new List<Room>();

    private readonly Dictionary<Vector2Int, Room> roomAtCell = new Dictionary<Vector2Int, Room>();

    private readonly Dictionary<Room, RoomDefinition> definitionOf = new Dictionary<Room, RoomDefinition>();

    private readonly Dictionary<Room, int> rotationStepsOf = new Dictionary<Room, int>();

    private readonly Dictionary<Room, Vector2Int> cellOf = new Dictionary<Room, Vector2Int>();

    private System.Random rng;

    // Up, Right, Down, Left
    private static readonly Vector2Int[] Directions = {Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};

    private static DoorDirection ToDoorDirection(int index)
    {
        return (DoorDirection)index;
    }

    private static DoorDirection Opposite(DoorDirection direction)
    {
        return (DoorDirection)(((int)direction + 2) % 4);
    }

    // World direction -> local direction
    private static DoorDirection WorldToLocal(DoorDirection worldDirection, int rotationSteps)
    {
        return (DoorDirection)(((int)worldDirection + rotationSteps) % 4);
    }

    // Local direction -> world direction
    private static DoorDirection LocalToWorld(DoorDirection localDirection, int rotationSteps)
    {
        return (DoorDirection)(((int)localDirection - rotationSteps + 4) % 4);
    }

    private bool HasWorldDoor(RoomDefinition definition, DoorDirection worldDirection, int rotationSteps)
    {
        DoorDirection localDirection = WorldToLocal(worldDirection, rotationSteps);
        return definition.HasDoor(localDirection);
    }

    public bool Generate(int stageSeed)
    {
        Debug.Log("[RoomPCG] Generate started.", this);
        ClearGenerated();
        seed = stageSeed;
        rng = new System.Random(seed);
        if (!LoadRoomDefinitions())
        {
            return false;
        }

        // Start room candidates
        List<RoomDefinition> starts = roomDefinitions.FindAll(r => r.isStartRoom);

        if (starts.Count == 0)
        {
            Debug.LogError("[RoomPCG] No start-room definition found.", this);
            return false;
        }

        // Spawn start room with random rotation
        RoomDefinition startDef = starts[rng.Next(starts.Count)];
        int startRotation = rng.Next(4);
        StartRoom = SpawnRoom(startDef, Vector2Int.zero, startRotation);

        if (StartRoom == null)
        {
            Debug.LogError("시작 방 생성 실패");
            return false;
        }
        roomStep = StartRoom.GetRoomSize();

        Debug.Log($"자동으로 읽은 방 크기: {roomStep}");

        List<Room> frontier = new List<Room>{StartRoom};

        int safety = Mathf.Max(100, targetRoomCount * 100);

        // Generate additional rooms
        while (spawnedRooms.Count < targetRoomCount && frontier.Count > 0 && safety-- > 0)
        {
            Room source = frontier[rng.Next(frontier.Count)];
            List<int> openDirections = GetAvailableDirections(source);

            if (openDirections.Count == 0)
            {
                frontier.Remove(source);
                continue;
            }

            Shuffle(openDirections);
            bool placed = false;
            foreach (int directionIndex in openDirections)
            {
                DoorDirection direction = ToDoorDirection(directionIndex);
                Vector2Int sourceCell = cellOf[source];
                Vector2Int targetCell = sourceCell + Directions[directionIndex];
                List<RoomDefinition> candidates = GetCandidates();
                Shuffle(candidates);
                foreach (RoomDefinition candidate in candidates)
                {
                    // Try all four rotations in random order
                    List<int> rotations = new List<int> { 0, 1, 2, 3 };
                    Shuffle(rotations);
                    foreach (int rotation in rotations)
                    {
                        // Candidate must have a door facing source
                        if (!HasWorldDoor(candidate, Opposite(direction), rotation))
                        {
                            continue;
                        }

                        if (!CanPlace(candidate, targetCell, rotation))
                        {
                            continue;
                        }

                        Room newRoom = SpawnRoom(candidate, targetCell,rotation);

                        if (newRoom == null)
                        {
                            continue;
                        }

                        frontier.Add(newRoom);
                        placed = true;
                        break;
                    }

                    if (placed)
                    {
                        break;
                    }
                }

                if (placed)
                {
                    break;
                }
            }

            if (!placed)
            {
                frontier.Remove(source);
            }
        }

        // Finalize all door connections.
        // Connected doors remain closed until explicitly opened.
        RefreshAllDoorConnections();

        if (spawnedRooms.Count < targetRoomCount)
        {
            Debug.LogWarning($"[RoomPCG] Generated {spawnedRooms.Count}/{targetRoomCount} rooms. " + "Check room door configurations or room definitions.", this);
        }
        Debug.Log($"[RoomPCG] Generation completed. Rooms: {spawnedRooms.Count}", this);
        return true;
    }

    public void ClearGenerated()
    {
        foreach (Room room in spawnedRooms)
        {
            if (room != null)
            {
                Destroy(room.gameObject);
            }
        }

        spawnedRooms.Clear();
        roomAtCell.Clear();
        definitionOf.Clear();
        rotationStepsOf.Clear();
        cellOf.Clear();

        StartRoom = null;
    }

    private Room SpawnRoom(RoomDefinition definition, Vector2Int anchorCell, int rotationSteps)
    {
        if (definition == null || definition.roomPrefab == null)
        {
            return null;
        }

        Vector3 worldPosition = new Vector3(anchorCell.x * roomStep.x, anchorCell.y * roomStep.y, 0f);

        Quaternion rotation = Quaternion.Euler(0f, 0f,rotationSteps * 90f);

        GameObject instance = Instantiate(definition.roomPrefab, worldPosition, rotation, roomsRoot != null ? roomsRoot : transform);

        Room room = instance.GetComponent<Room>();

        if (room == null)
        {
            Debug.LogError($"Room prefab '{definition.roomPrefab.name}' needs Room component.", this);

            Destroy(instance);
            return null;
        }

        room.RebuildDoorLookup();

        spawnedRooms.Add(room);

        roomAtCell[anchorCell] = room;
        definitionOf[room] = definition;
        rotationStepsOf[room] = rotationSteps;
        cellOf[room] = anchorCell;

        return room;
    }

    private List<RoomDefinition> GetCandidates()
    {
        List<RoomDefinition> candidates = new List<RoomDefinition>();

        foreach (RoomDefinition def in roomDefinitions)
        {
            if (def == null)
                continue;

            if (def.roomPrefab == null)
                continue;

            if (def.weight <= 0)
                continue;

            // isStartRoom이어도 재사용 가능
            candidates.Add(def);
        }

        return candidates;
    }

    private List<int> GetAvailableDirections(Room room)
    {
        List<int> result = new List<int>();

        RoomDefinition definition = definitionOf[room];
        int rotation = rotationStepsOf[room];
        Vector2Int cell = cellOf[room];

        for (int i = 0; i < 4; i++)
        {
            DoorDirection worldDirection = ToDoorDirection(i);

            if (!HasWorldDoor(definition, worldDirection, rotation))
            {
                continue;
            }

            Vector2Int targetCell = cell + Directions[i];

            if (!roomAtCell.ContainsKey(targetCell))
            {
                result.Add(i);
            }
        }

        return result;
    }

    private bool CanPlace(RoomDefinition candidate, Vector2Int anchorCell, int candidateRotation)
    {
        if (roomAtCell.ContainsKey(anchorCell))
        {
            return false;
        }

        for (int i = 0; i < 4; i++)
        {
            DoorDirection worldDirection = ToDoorDirection(i);

            Vector2Int neighborCell = anchorCell + Directions[i];

            if (!roomAtCell.TryGetValue(neighborCell, out Room neighbor))
            {
                continue;
            }

            RoomDefinition neighborDefinition = definitionOf[neighbor];

            int neighborRotation = rotationStepsOf[neighbor];

            bool candidateHasDoor = HasWorldDoor(candidate, worldDirection, candidateRotation);

            bool neighborHasDoor = HasWorldDoor(neighborDefinition, Opposite(worldDirection), neighborRotation);

            // Both sides must have a door,
            // or neither side must have a door.
            if (candidateHasDoor != neighborHasDoor)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Recalculate all door connections using world directions.
    /// Does not open doors.
    /// </summary>
    private void RefreshAllDoorConnections()
    {
        foreach (Room room in spawnedRooms)
        {
            RoomDefinition definition = definitionOf[room];
            int rotation = rotationStepsOf[room];
            Vector2Int cell = cellOf[room];

            for (int i = 0; i < 4; i++)
            {
                DoorDirection worldDirection = ToDoorDirection(i);

                DoorDirection localDirection = WorldToLocal(worldDirection, rotation);

                Vector2Int neighborCell = cell + Directions[i];

                bool connected = false;

                if (HasWorldDoor(definition, worldDirection, rotation) && roomAtCell.TryGetValue(neighborCell, out Room neighbor))
                {
                    RoomDefinition neighborDefinition = definitionOf[neighbor];

                    int neighborRotation = rotationStepsOf[neighbor];

                    connected = HasWorldDoor(neighborDefinition, Opposite(worldDirection), neighborRotation);
                }

                // Set connection only. Door remains closed.
                room.SetDoorConnected(localDirection, connected);
            }
        }
    }

    /// <summary>
    /// Opens both sides of every connected passage
    /// adjacent to the specified room.
    /// </summary>
    public void OpenConnectedDoors(Room room)
    {
        if (room == null || !definitionOf.ContainsKey(room) || !rotationStepsOf.ContainsKey(room))
        {
            return;
        }

        int rotation = rotationStepsOf[room];
        Vector2Int cell = cellOf[room];

        for (int i = 0; i < 4; i++)
        {
            DoorDirection worldDirection = ToDoorDirection(i);

            Vector2Int neighborCell = cell + Directions[i];

            if (!roomAtCell.TryGetValue(neighborCell, out Room neighbor))
            {
                continue;
            }

            int neighborRotation = rotationStepsOf[neighbor];

            if (!HasWorldDoor(definitionOf[room], worldDirection, rotation))
            {
                continue;
            }

            if (!HasWorldDoor(definitionOf[neighbor], Opposite(worldDirection), neighborRotation))
            {
                continue;
            }

            DoorDirection localDirection = WorldToLocal(worldDirection, rotation);

            DoorDirection neighborLocalDirection = WorldToLocal(Opposite(worldDirection), neighborRotation);

            // Open both sides of the same passage.
            room.OpenDoor(localDirection);
            neighbor.OpenDoor(neighborLocalDirection);
        }
    }

    /// <summary>
    /// Opens all connected passages in the generated stage.
    /// Unconnected doors remain closed.
    /// </summary>
    public void OpenAllConnectedDoors()
    {
        foreach (Room room in spawnedRooms)
        {
            OpenConnectedDoors(room);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private bool LoadRoomDefinitions()
    {
        roomDefinitions.Clear();

        RoomDefinition[] loadedDefinitions = Resources.LoadAll<RoomDefinition>(ROOM_DEFINITION_PATH);

        if (loadedDefinitions == null || loadedDefinitions.Length == 0)
        {
            Debug.LogError($"[RoomPCG] No RoomDefinitions found at Resources/{ROOM_DEFINITION_PATH}", this);

            return false;
        }

        foreach (RoomDefinition definition in loadedDefinitions)
        {
            if (definition == null)
                continue;

            if (definition.roomPrefab == null)
            {
                Debug.LogWarning($"[RoomPCG] {definition.name} has no Room Prefab.", this);

                continue;
            }

            roomDefinitions.Add(definition);
        }

        roomDefinitions.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

        Debug.Log($"[RoomPCG] Loaded {roomDefinitions.Count} Room Definitions.", this);

        foreach (RoomDefinition definition in roomDefinitions)
        {
            Debug.Log($"[RoomPCG] Loaded: {definition.name} " + $"(Prefab: {definition.roomPrefab.name})", this);
        }

        return roomDefinitions.Count > 0;
    }
}
