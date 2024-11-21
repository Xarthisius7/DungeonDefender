using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.AI;
using NavMeshPlus.Components;


public class GridCell
{
    public int x, y;
    public bool hasRoom = false;
    public List<dir> connectedDir = new List<dir>();
    public GameObject roomObject = null;
    public string roomFeature = "";
    public int Level = 1;

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
    private Dictionary<string, List<GameObject>> TreasureRoomTypePools;
    private Dictionary<string, List<GameObject>> HighDiffRoomTypePools;
    private Dictionary<string, List<GameObject>> TrapRoomTypePools;
    private Dictionary<string, List<GameObject>> FianlExitRoomTypePools;
    private Dictionary<string, List<GameObject>> MainRoomTypePools;

    public int gridSize = 10;
    private GridCell[,] grid;
    [SerializeField] float spitChance = 0.2f;
    [SerializeField] int directionLength = 8;
    [SerializeField] int AreaIncreasement = 3;

    int currentDirectionRemain = 0;

    [SerializeField] public bool useSeed = false;
    [SerializeField] public int seed = 1153905347;
    private System.Random random;

    [SerializeField] MinimapGenerator miniGen;
    [SerializeField] Transform MapGenerationTransform;

    public int StartRoomX = 0;
    public int StartRoomY = 0;

    public NavMeshSurface NavMesh;

    public GridCell[,] CreateMap()
    {
        //Generate the entire map, and return the girds to GameController.
        GenerateMainArea();

        //Assign each area's room type
        AssignRoomsTypes(1);
        AssignRoomsTypes(2);
        AssignRoomsTypes(3);

        //actually create the room
        InstantiateRooms();

        // Build our Navmesh
        NavMesh.BuildNavMesh();

        return grid;
    }

    public GameObject[,] MakeMiniMap()
    {
        //generate minimap
        return miniGen.GenerateMinimap(grid, gridSize);
    }


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
        TreasureRoomTypePools = new Dictionary<string, List<GameObject>>();
        HighDiffRoomTypePools = new Dictionary<string, List<GameObject>>();
        TrapRoomTypePools = new Dictionary<string, List<GameObject>>();
        FianlExitRoomTypePools = new Dictionary<string, List<GameObject>>();
        MainRoomTypePools = new Dictionary<string, List<GameObject>>();

        string[] roomTypes = {
            "U", "B", "L", "R",    "BU", "LU", "RU", "BL", "BR", "LR",
            "BLU", "BRU", "LRU", "BLR",     "BLRU"
        };
        foreach (var type in roomTypes)
        {
            roomTypePools[type] = new List<GameObject>();
            TreasureRoomTypePools[type] = new List<GameObject>();
            HighDiffRoomTypePools[type] = new List<GameObject>();
            TrapRoomTypePools[type] = new List<GameObject>();
            FianlExitRoomTypePools[type] = new List<GameObject>();
            MainRoomTypePools[type] = new List<GameObject>();
        }

        // Load all the prefab from Assets/Prefabs/Rooms
        LoadRoomPrefabs();


        grid = new GridCell[gridSize, gridSize];
        InitializeGrid();


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
        grid[centerX, centerY].roomFeature = "BASEROOM";

        //set the center room's x and y, so game controller knows.
        StartRoomX = centerX; StartRoomY = centerY;

        // Generating the First Area.
        // Randomly select 3 directions to generate branches
        List <dir> directions = new List<dir>() { dir.U, dir.B, dir.L, dir.R };
        ShuffleList(directions);


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



        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].hasRoom)
                {
                    grid[x, y].Level = 3;
                }
            }
        }



        // Generating the 2nd Area.

        GridCell area2Start = GetNextStart(centerX, centerY,3);

        directionLength += AreaIncreasement;

        for (int i = 0; i < 4; i++)
        {
            currentDirectionRemain = directionLength;
            GenerateSingleRoom(area2Start.x, area2Start.y, directions[i]);
        }




        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].hasRoom && grid[x, y].Level!=3)
                {
                    grid[x, y].Level = 2;
                }
            }
        }
        dir invalideDir = StringToDir(area2Start.roomFeature);

        area2Start.roomFeature = "BASEROOM";

        // Generating the 3rd Area.

        GridCell area3Start = LocateFinalArea(area2Start.x, area2Start.x, 2, invalideDir);
        area3Start.roomFeature = "BASEROOM";

        directionLength += AreaIncreasement;

        for (int i = 0; i < 4; i++)
        {
            currentDirectionRemain = directionLength;
            GenerateSingleRoom(area3Start.x, area3Start.y, directions[i]);
        }


    }



    public void AssignRoomsTypes(int targetType)
    {
        List<GridCell> RList = new List<GridCell>();


        //Assign room types to Deadends before actually genearting them.


        // Iterate through all rooms and add the rooms that meet the criteria to the RList
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].hasRoom && grid[x, y].Level == targetType && grid[x, y].roomFeature.Equals("DEADEND"))
                {
                    RList.Add(grid[x, y]);
                }
            }
        }

        //Assign the Final exit to area 3
        // If targetType is 1, randomly select one room from the list and set it as "FINALEXIT"
        if (targetType == 1 && RList.Count > 0)
        {
            int finalExitIndex = random.Next(RList.Count);
            RList[finalExitIndex].roomFeature = "FINALEXIT";
            RList.RemoveAt(finalExitIndex);
        }

        // Assign 1 room features in order: "TREASURE", "HIGHDIFF", "TRAP"
        string[] roomFeatures = { "TREASURE", "HIGHDIFF", "TRAP" };
        int featureIndex = 0;

        while (RList.Count > 0 && featureIndex < roomFeatures.Length)
        {
            int index = random.Next(RList.Count);
            RList[index].roomFeature = roomFeatures[featureIndex];
            RList.RemoveAt(index);
            featureIndex++;
        }

        // Randomly assign the remaining rooms with "TREASURE", "HIGHDIFF", "TRAP"
        while (RList.Count > 0)
        {
            int index = random.Next(RList.Count);
            string randomFeature = roomFeatures[random.Next(roomFeatures.Length)];
            RList[index].roomFeature = randomFeature;
            RList.RemoveAt(index);
        }
    }







    public GridCell GetNextStart(int centerx, int centery, int targetLevel)
    {
        GridCell farthestRoom = null;
        float maxDistance = -1f;
        dir chosenDir = dir.U;

        // Traverse through all cells in the grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GridCell cell = grid[x, y];
                if (cell.hasRoom && cell.roomFeature == "DEADEND" && cell.Level == targetLevel)
                {
                    float distance = Mathf.Sqrt(Mathf.Pow(x - centerx, 2) + Mathf.Pow(y - centery, 2));

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestRoom = cell;

                        // Determine relative direction from centerx, centery
                        if (x <= centerx && y <= centery)
                        {
                            // Bottom Left
                            chosenDir = (centerx - x > centery - y) ? dir.L : (centerx - x < centery - y) ? dir.B : (Random.value > 0.5f ? dir.L : dir.B);
                        }
                        else if (x <= centerx && y >= centery)
                        {
                            // Top Left
                            chosenDir = (centerx - x > y - centery) ? dir.L : (centerx - x < y - centery) ? dir.U : (Random.value > 0.5f ? dir.L : dir.U);
                        }
                        else if (x >= centerx && y <= centery)
                        {
                            // Bottom Right
                            chosenDir = (x - centerx > centery - y) ? dir.R : (x - centerx < centery - y) ? dir.B : (Random.value > 0.5f ? dir.R : dir.B);
                        }
                        else if (x >= centerx && y >= centery)
                        {
                            // Top Right
                            chosenDir = (x - centerx > y - centery) ? dir.R : (x - centerx < y - centery) ? dir.U : (Random.value > 0.5f ? dir.R : dir.U);
                        }
                    }
                }
            }
        }

        if (farthestRoom != null)
        {
            farthestRoom.roomFeature = chosenDir.ToString();
        }

        return farthestRoom;
    }

    public GridCell LocateFinalArea(int centerx, int centery, int targetLevel, dir UnavailableDir)
    {
        GridCell farthestRoom = null;
        float maxDistance = -1f;

        // Traverse through all cells in the grid
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GridCell cell = grid[x, y];
                if (cell.hasRoom && cell.roomFeature == "DEADEND" && cell.Level == targetLevel)
                {
                    // Determine the direction from centerx, centery to x, y
                    if (!IsDirectionUnavailable(centerx, centery, x, y, UnavailableDir))
                    {
                        float distance = Mathf.Sqrt(Mathf.Pow(x - centerx, 2) + Mathf.Pow(y - centery, 2));

                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            farthestRoom = cell;
                        }
                    }
                }
            }
        }

        return farthestRoom;
    }

    private bool IsDirectionUnavailable(int centerx, int centery, int x, int y, dir unavailableDir)
    {
        switch (unavailableDir)
        {
            case dir.R:
                return x > centerx;
            case dir.L:
                return x > centerx;
            case dir.U:
                return y < centery;
            case dir.B:
                return y > centery;
            default:
                return false;
        }
    }

    public dir StringToDir(string direction)
    {
        return (dir)System.Enum.Parse(typeof(dir), direction);
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
        int totalTreasureRoomsGenerated = 0;
        int totalHighDIffRoomsGenerated = 0;
        int totalTrapRoomsGenerated = 0;
        int totalDeadends = 0; 

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].hasRoom)
                {
                    string roomType = string.Join("", grid[x, y].connectedDir);
                    roomType = new string(roomType.Distinct().ToArray());

                    GameObject room = null;
                    if (grid[x, y].roomFeature == "")
                    {
                        //Common Rooms
                        room = LoadRoomType(roomType, "common");
                    }
                    else if (grid[x, y].roomFeature == "BASEROOM")
                    {
                        room = LoadRoomType(roomType, "BASEROOM");
                    }
                    else if (grid[x, y].roomFeature == "FINALEXIT")
                    {
                        room = LoadRoomType(roomType, "FINALEXIT");
                        totalDeadends++;
                    }
                    else if (grid[x, y].roomFeature == "TREASURE")
                    {
                        room = LoadRoomType(roomType, "TREASURE");
                        totalDeadends++;
                        totalTreasureRoomsGenerated++;
                    }
                    else if (grid[x, y].roomFeature == "HIGHDIFF")
                    {
                        room = LoadRoomType(roomType, "HIGHDIFF");
                        totalDeadends++;
                        totalHighDIffRoomsGenerated++;
                    }
                    else if (grid[x, y].roomFeature == "TRAP")
                    {
                        room = LoadRoomType(roomType, "TRAP");
                        totalDeadends++;
                        totalTrapRoomsGenerated++;
                    }
                    else
                    {
                        room = LoadRoomType(roomType, "common");
                    }



                    if (room != null)
                    {
                        Vector3 position = new Vector3((x - centerX) * 8.0f, (y - centerY) * 9.0f, 0);
                        grid[x, y].roomObject = Instantiate(room, position, Quaternion.identity);
                        grid[x, y].roomObject.transform.SetParent(MapGenerationTransform);
                        grid[x, y].roomObject.name = $"{x}_{y}_{grid[x, y].roomFeature}_{roomType}";

                        totalRoomsGenerated++;
                    }
                }


            }
        }

        Debug.Log("Room generation complete. A total of: " + totalRoomsGenerated + " Rooms has been created.");
        Debug.Log("There's a total of: " + totalDeadends + " Dead end in the map.");
        Debug.Log("There's a total of: " + totalTreasureRoomsGenerated + " Treasure room in the map.");
        Debug.Log("There's a total of: " + totalHighDIffRoomsGenerated + " High Difficulty room in the map.");
        Debug.Log("There's a total of: " + totalTrapRoomsGenerated + " Trap room  in the map.");
        Debug.Log("Map Generation is using seed��" + seed);
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

            // If does not contain _
            if (underscoreIndex == -1) continue;

            // Extract all characters that are before _
            string Key = roomName.Substring(0, underscoreIndex);
            string typeKey = string.Concat(Key.OrderBy(c => c));

            // Determine which pool to add the room to based on the prefix
            if (roomName.Contains("S_P1_"))
            {
                if (IsValidRoomType(typeKey))
                {
                    TreasureRoomTypePools[typeKey].Add(room);
                    count++;
                }
            }
            else if (roomName.Contains("S_P2_"))
            {
                if (IsValidRoomType(typeKey))
                {
                    HighDiffRoomTypePools[typeKey].Add(room);
                    count++;
                }
            }
            else if (roomName.Contains("S_P3_"))
            {
                if (IsValidRoomType(typeKey))
                {
                    TrapRoomTypePools[typeKey].Add(room);
                    count++;
                }
            }
            else if (roomName.Contains("S_P4_"))
            {
                if (IsValidRoomType(typeKey))
                {
                    FianlExitRoomTypePools[typeKey].Add(room);
                    count++;
                }
            }
            else if (roomName.Contains("S_P5_"))
            {
                if (IsValidRoomType(typeKey))
                {
                    MainRoomTypePools[typeKey].Add(room);
                    count++;
                }
            }
            else
            {
                // If not matching any specific prefix, add it to roomTypePools
                if (IsValidRoomType(typeKey))
                {
                    roomTypePools[typeKey].Add(room);
                    count++;
                }
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

    public GameObject LoadRoomType(string type, string targetPool)
    {
        //Returns a given room type from give pool, randomly selected.

        string sortedType = string.Concat(type.OrderBy(c => c));
        List<GameObject> roomList = null;

        switch (targetPool.ToLower())
        {
            case "common":
                if (roomTypePools.ContainsKey(sortedType))
                    roomList = roomTypePools[sortedType];
                break;
            case "treasure":
                if (TreasureRoomTypePools.ContainsKey(sortedType))
                    roomList = TreasureRoomTypePools[sortedType];
                break;
            case "highdiff":
                if (HighDiffRoomTypePools.ContainsKey(sortedType))
                    roomList = HighDiffRoomTypePools[sortedType];
                break;
            case "trap":
                if (TrapRoomTypePools.ContainsKey(sortedType))
                    roomList = TrapRoomTypePools[sortedType];
                break;
            case "finalexit":
                if (FianlExitRoomTypePools.ContainsKey(sortedType))
                    roomList = FianlExitRoomTypePools[sortedType];
                break;
            case "baseroom":
                if (MainRoomTypePools.ContainsKey(sortedType))
                    roomList = MainRoomTypePools[sortedType];
                break;
            default:
                if (roomTypePools.ContainsKey(sortedType))
                    roomList = roomTypePools[sortedType];
                break;
        }

        // Check if the room list is available and has at least one room.
        if (roomList != null && roomList.Count > 0)
        {
            return roomList[random.Next(0, roomList.Count)];
        }
        else
        {
            Debug.LogWarning($"Room type '{sortedType}' in {targetPool} Room Pool doesn't exist or there's no available room for this type.");
            return null;
        }
    }
}
