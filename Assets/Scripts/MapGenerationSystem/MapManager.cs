using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class GridCell
{
    public int x, y;
    public bool hasRoom = false;
    public List<dir> connectedDir = new List<dir>();
    public GameObject roomObject = null;
    public string roomFeature = "";

    public bool isMainRoom = false;

    public GridCell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public enum dir
{
    U, B, L, R
}





public class MapManager : MonoBehaviour
{

    public static MapManager Instance { get; private set; }
    private Dictionary<string, List<GameObject>> roomTypePools;

    public int gridSize = 10;
    private GridCell[,] grid;
    [SerializeField] float spitChance = 0.2f;
    [SerializeField] int directionLength = 8;

    int currentDirectionRemain = 0;

    [SerializeField] public bool useSeed = false;
    [SerializeField] public int seed = 1153905347;
    private System.Random random;


    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (useSeed == false)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        else
        {
            if (seed == null)
            {
                seed = UnityEngine.Random.Range(0, int.MaxValue);
            }
        }
        random = new System.Random(seed);


        // All 15 different small room types
        roomTypePools = new Dictionary<string, List<GameObject>>();
        string[] roomTypes = {
            "U", "B", "L", "R",    "BU", "LU", "RU", "BL", "BR", "LR",
            "BLU", "BRU", "LRU", "BLR",     "BLRU"
        };
        foreach (var type in roomTypes)
        {
            roomTypePools[type] = new List<GameObject>();
        }

        // Load all the prefab from Assets/Prefabs/Rooms
        LoadRoomPrefabs();


        grid = new GridCell[gridSize, gridSize];
        InitializeGrid();
        GenerateMainArea();
        InstantiateRooms();



        //telepot the player to the Spawnroom at the begining of the game

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].isMainRoom)
                {
                    //PlayerController.Instance.TelepotPlayer(grid[x, y].roomObject.transform);
                }
            }
        }
    }


    void InitializeGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = new GridCell(x, y);
            }
        }
    }

    void GenerateMainArea()
    {
        int centerX = gridSize / 2;
        int centerY = gridSize / 2;
        grid[centerX, centerY].hasRoom = true;
        grid[centerX, centerY].isMainRoom = true;


        // Randomly select 3 directions to generate branches
        List <dir> directions = new List<dir>() { dir.U, dir.B, dir.L, dir.R };
        ShuffleList(directions);

        grid[centerX, centerY].roomFeature = "SPAWN";

        for (int i = 0; i < 3; i++)
        {
            currentDirectionRemain = directionLength;
            GenerateSingleRoom(centerX, centerY, directions[i]);
            grid[centerX, centerY].connectedDir.Remove(directions[i]);
        }

        if(grid[centerX, centerY+1].hasRoom) grid[centerX, centerY].connectedDir.Add(dir.U);
        if(grid[centerX, centerY-1].hasRoom) grid[centerX, centerY].connectedDir.Add(dir.B);
        if(grid[centerX-1, centerY].hasRoom) grid[centerX, centerY].connectedDir.Add(dir.L);
        if(grid[centerX+1, centerY].hasRoom) grid[centerX, centerY].connectedDir.Add(dir.R);

    }

    void GenerateSingleRoom(int x, int y, dir fromDir)
    {
        if (currentDirectionRemain <= 0)
        {

            grid[x, y].hasRoom = true;
            grid[x, y].connectedDir.Add(fromDir);
            grid[x, y].roomFeature = "DEADEND";
            //if no more room on this direction path


            return;
        } else if (currentDirectionRemain >= 1 )
        {

            List<dir> availableDirections = GetAvailableDirections(x, y);
            if (availableDirections.Count == 0)
            {
                //Debug.Log($"No available directions for room at ({x},{y})");

                if(!grid[x, y].hasRoom)
                {
                    grid[x, y].hasRoom = true;
                    grid[x, y].connectedDir.Add(fromDir);
                    grid[x, y].roomFeature = "DEADEND";
                }


                return;
            }

            int numDirections = (random.NextDouble() < (1 - spitChance)) ? 1 : Mathf.Min(2, availableDirections.Count);
            List<dir> selectedDirections = new List<dir>();
            ShuffleList(availableDirections);

            for (int i = 0; i < numDirections; i++)
            {
                selectedDirections.Add(availableDirections[i]);
            }

            currentDirectionRemain -= selectedDirections.Count;
            grid[x, y].hasRoom = true;
            grid[x, y].connectedDir.Add(fromDir);
            //Debug.Log($"Room created at ({x},{y}) with connections: {string.Join(",", selectedDirections)}");

            foreach (dir direction in selectedDirections)
            {
                grid[x, y].connectedDir.Add(direction);
                int newX = x;
                int newY = y;
                switch (direction)
                {
                    case dir.U: newY += 1; break;
                    case dir.B: newY -= 1; break;
                    case dir.L: newX -= 1; break;
                    case dir.R: newX += 1; break;
                }
                GenerateSingleRoom(newX, newY, GetOppositeDirection(direction));
            }
        }

    }

    List<dir> GetAvailableDirections(int x, int y)
    {
        List<dir> available = new List<dir>();
        if (y + 1 < gridSize && !grid[x, y + 1].hasRoom) available.Add(dir.U);
        if (y - 1 >= 0 && !grid[x, y - 1].hasRoom) available.Add(dir.B);
        if (x - 1 >= 0 && !grid[x - 1, y].hasRoom) available.Add(dir.L);
        if (x + 1 < gridSize && !grid[x + 1, y].hasRoom) available.Add(dir.R);
        return available;
    }

    dir GetOppositeDirection(dir direction)
    {
        switch (direction)
        {
            case dir.U: return dir.B;
            case dir.B: return dir.U;
            case dir.L: return dir.R;
            case dir.R: return dir.L;
            default: return dir.U; // Default case
        }
    }

    void InstantiateRooms()
    {
        int centerX = (int)gridSize/2 ;
        int centerY = (int)gridSize/2 ;

        int totalRoomsGenerated = 0;
        int totalDeadends = 0; 

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].hasRoom)
                {
                    string roomType = string.Join("", grid[x, y].connectedDir);
                    roomType = new string(roomType.Distinct().ToArray());
                    if (grid[x, y].roomFeature == "DEADEND")
                    {
                        totalDeadends++;
                    }
                    GameObject room = LoadRoomType(roomType);
                    Vector3 position = new Vector3((x - centerX) * 8.4f, (y-centerY) * 9.0f, 0);
                    grid[x, y].roomObject = Instantiate(room, position, Quaternion.identity);
                    grid[x, y].roomObject.transform.SetParent(transform);
                    grid[x, y].roomObject.name = $"{x}_{y}_{grid[x, y].roomFeature}_{roomType}";

                    totalRoomsGenerated++;
                    //Debug.Log($"Room of type {roomType} instantiated at ({x},{y})");
                }


            }
        }

        Debug.Log("Room generation complete. A total of: " + totalRoomsGenerated + " Rooms has been created.");
        Debug.Log("There's a total of: " + totalDeadends + " Dead end in the map.");
        Debug.Log("Map Generation is using seed£º" + seed);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randIndex = random.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }






    private void LoadRoomPrefabs()
    {
        string path = "Prefabs/Rooms";
        var roomFiles = Resources.LoadAll<GameObject>(path);

        int count = 0;
        foreach (var room in roomFiles)
        {
            string roomName = room.name;
            int underscoreIndex = roomName.IndexOf('_');

            // If contains _
            if (underscoreIndex == -1) continue;

            // exact all the character that is before _
            string Key = roomName.Substring(0, underscoreIndex);
            string typeKey = string.Concat(Key.OrderBy(c => c));

            // if valid, add it into the pool
            if (IsValidRoomType(typeKey))
            {
                roomTypePools[typeKey].Add(room);

                count++;
            }
        }
        Debug.Log("A total of " + count + " rooms has been loaded.");
    }

    private bool IsValidRoomType(string typeKey)
    {
        // making sure the character only contains U, B, L, R
        foreach (char c in typeKey)
        {
            if (!"UBLR".Contains(c))
                return false;
        }

        string sortedType = string.Concat(typeKey.OrderBy(c => c));

        return roomTypePools.ContainsKey(sortedType);
    }

    public GameObject LoadRoomType(string type)
    {
        //Returns a given room type, randomly selected.

        // Debug.Log("Try load room type: " + type);
        // Sort the characters
        string sortedType = string.Concat(type.OrderBy(c => c));

        // Check if the room type exist
        if (roomTypePools.ContainsKey(sortedType) && roomTypePools[sortedType].Count > 0)
        {
            var roomList = roomTypePools[sortedType];
            return roomList[random.Next(0, roomList.Count)];
        }
        else
        {
            Debug.LogWarning($"Room type '{sortedType}' Doesn't exist or there's no avaliable room for this type.");
            return null;
        }
    }
}
