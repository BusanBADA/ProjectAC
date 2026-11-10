using UnityEngine;

public class InGameScene : MonoBehaviour
{
    [SerializeField] private RoomPCG roomPCG;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private int stageSeed = 12345;

    [SerializeField]
    private CameraFollow cameraFollow;//플레이어가 카메라 추적을 위해 추가

    private GameObject playerInstance;

    private void Start()
    {
        StartStage(stageSeed);
    }

    public void StartStage(int seed)
    {
        if (roomPCG == null)
        {
            Debug.LogError("[InGameScene] RoomPCG is not assigned.", this);
            return;
        }

        if (!roomPCG.Generate(seed)) return;

        if (playerInstance != null) Destroy(playerInstance);

        Room startRoom = roomPCG.StartRoom;
        if (startRoom == null)
        {
            Debug.LogError("[InGameScene] Generated roomPCG has no start room.", this);
            return;
        }

        Vector3 spawnPosition = startRoom.GetPlayerSpawnPosition();
        if (playerPrefab != null)
        {
            playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(playerInstance.transform); //카메라 추적
        }
        else
        {
            Debug.LogWarning("[InGameScene] Player prefab is not assigned; dungeon generated only.", this);
        }
    }
}
