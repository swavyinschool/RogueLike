using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    public int maxEnemies; // New attribute for the maximum number of enemies
    List<Room> rooms = new List<Room>();

    // Sets the size of the dungeon
    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    // Sets the minimum and maximum room sizes
    public void SetRoomSize(int min, int max)
    {
        minRoomSize = min;
        maxRoomSize = max;
    }

    // Sets the maximum number of rooms
    public void SetMaxRooms(int max)
    {
        maxRooms = max;
    }

    // Sets the maximum number of enemies
    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }

    // Function to place enemies in a room
    private void PlaceEnemies(Room room, int maxEnemies)
    {
        // The number of enemies to spawn
        int num = Random.Range(0, maxEnemies + 1);

        for (int counter = 0; counter < num; counter++)
        {
            // The boundaries of the room are walls, so subtract and add 1 to avoid placing enemies on walls
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            // Create different enemies
            if (Random.value < 0.5f)
            {
                GameManager.Get.CreateActor("Wesp", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateActor("Wolf", new Vector2(x, y));
            }
        }
    }

    // Main function to generate the dungeon
    public void Generate()
    {
        rooms.Clear();

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            int roomX = Random.Range(0, width - roomWidth - 1);
            int roomY = Random.Range(0, height - roomHeight - 1);

            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            // If the room overlaps with another room, discard it
            if (room.Overlaps(rooms))
            {
                continue;
            }

            // Add tiles to make the room visible on the tilemap
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (!TrySetWallTile(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0));
                    }
                }
            }

            // Create a corridor between rooms
            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }

            rooms.Add(room);

            // Place enemies in the room
            PlaceEnemies(room, maxEnemies);
        }

        // Spawn the player in the first room
        var player = GameManager.Get.CreateActor("Player", rooms[0].Center());
    }

    // Function to try and set a wall tile
    private bool TrySetWallTile(Vector3Int pos)
    {
        // If this is a floor tile, it should not be a wall
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            // Otherwise, it can be a wall
            MapManager.Get.ObstacleMap.SetTile(pos, MapManager.Get.WallTile);
            return true;
        }
    }

    // Function to set a floor tile
    private void SetFloorTile(Vector3Int pos)
    {
        // This tile should be walkable, so remove every obstacle
        if (MapManager.Get.ObstacleMap.GetTile(pos))
        {
            MapManager.Get.ObstacleMap.SetTile(pos, null);
        }
        // Set the floor tile
        MapManager.Get.FloorMap.SetTile(pos, MapManager.Get.FloorTile);
    }

    // Function to create a tunnel between two rooms
    private void TunnelBetween(Room oldRoom, Room newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            // Move horizontally, then vertically
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            // Move vertically, then horizontally
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        // Generate the coordinates for this tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // Set the tiles for this tunnel
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (!TrySetWallTile(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }
}
