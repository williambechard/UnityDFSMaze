using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public int sizeX;
    public int sizeY;
    public float Spacing;
    public float waitTime;
    public GameObject RoomPREFAB;

    List<Room> grid = new List<Room>();

    public List<Room> currentPath = new();
    public List<Room> completedPath = new();


    // Start is called before the first frame update
    void Start() => generateGrid();


    void generateGrid()
    {
        //clear grid
        grid.Clear();
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                GameObject room = Instantiate<GameObject>(RoomPREFAB);
                room.name = "Room[" + x + "][" + y + "]";
                room.transform.position = new Vector3(0, 0, 0);
                room.transform.parent = this.transform;
                room.transform.localPosition = new Vector3(Spacing * x, Spacing * -y, 0);
                room.GetComponent<Room>().setGridPosition(new Point(x, y), grid.Count);
                grid.Add(room.GetComponent<Room>());
            }
        }
        DFS();
    }

    private void DFS()
    {
        StartCoroutine(DFS_Slow());
    }

    public bool roomAvailable(int Index)
    {
        if (Index != -1) return grid[Index].state != NodeState.Current && grid[Index].state != NodeState.Completed;
        else return false;
    }

    public Room randomAdjacentUnvistedRoom(int Coord, Room room)
    {
        Debug.Log("Found Index = " + Coord);
        Vector2 position = GetItemLocation(Coord);
        int x = (int)position.x;
        int y = (int)position.y;
        Debug.Log("Point position " + x + "," + y);
        List<Tools.Direction> AvailableDirections = new List<Tools.Direction>();

        bool topAvailable = roomAvailable(GetItemIndex(x, y - 1));
        bool rightAvailable = roomAvailable(GetItemIndex(x + 1, y));
        bool bottomAvailable = roomAvailable(GetItemIndex(x, y + 1));
        bool leftAvailable = roomAvailable(GetItemIndex(x - 1, y));

        if (topAvailable) AvailableDirections.Add(Tools.Direction.Top);
        if (rightAvailable) AvailableDirections.Add(Tools.Direction.Right);
        if (bottomAvailable) AvailableDirections.Add(Tools.Direction.Bottom);
        if (leftAvailable) AvailableDirections.Add(Tools.Direction.Left);



        if (AvailableDirections.Count > 0)
        {
            Tools.Direction targetDirection = AvailableDirections[UnityEngine.Random.Range(0, AvailableDirections.Count)];
            int targetIndex = -1;
            switch (targetDirection)
            {
                case Tools.Direction.Top:
                    targetIndex = GetItemIndex(x, y - 1);
                    break;
                case Tools.Direction.Right:
                    targetIndex = GetItemIndex(x + 1, y);
                    break;
                case Tools.Direction.Bottom:
                    targetIndex = GetItemIndex(x, y + 1);
                    break;
                case Tools.Direction.Left:
                    targetIndex = GetItemIndex(x - 1, y);
                    break;

            }

            if (targetIndex != -1)
            {
                //5. break the wall between the room and the Adjacent Room
                room.modifyWall(targetDirection, true);
                Room adjacentRoom = grid[targetIndex];
                Tools.Direction oppDirection;
                if (targetDirection == Tools.Direction.Top || targetDirection == Tools.Direction.Right)
                    oppDirection = targetDirection + 2;
                else oppDirection = targetDirection - 2;

                adjacentRoom.modifyWall(oppDirection, true);

                return adjacentRoom;
            }
        }
        return null;
    }

    IEnumerator DFS_Slow()
    {
        //get starting room
        int index = UnityEngine.Random.Range(0, grid.Count);

        Room r = grid[index];
        currentPath.Add(r);

        r.SetState(NodeState.Current);

        int startIndex = index;

        while (currentPath.Count > 0)
        {
            //check rooms next to current one
            //4. randomly check adjacent room that hasnt been visited (if any exists)
            Room AdjacentRoom = randomAdjacentUnvistedRoom(grid.IndexOf(r), r);
            if (AdjacentRoom != null)
            {
                currentPath.Add(AdjacentRoom);
                AdjacentRoom.SetState(NodeState.Current);
                r = AdjacentRoom;
            }
            else
            {
                if (currentPath.Count > 0)
                {
                    r = currentPath[currentPath.Count - 1];
                    r.SetState(NodeState.Completed);
                    currentPath.RemoveAt(currentPath.Count - 1);
                }

            }
            yield return new WaitForSeconds(waitTime);
        }

        grid[startIndex].SetState(NodeState.Start);

        //determine final room as corner of maze farthest from starting cell
        List<float> largestDistance = new();

        largestDistance.Add(Vector2.Distance(grid[startIndex].transform.position, grid[0].transform.position));
        largestDistance.Add(Vector2.Distance(grid[startIndex].transform.position, grid[GetItemIndex(sizeX - 1, 0)].transform.position));
        largestDistance.Add(Vector2.Distance(grid[startIndex].transform.position, grid[GetItemIndex(sizeX - 1, sizeY - 1)].transform.position));
        largestDistance.Add(Vector2.Distance(grid[startIndex].transform.position, grid[GetItemIndex(0, sizeY - 1)].transform.position));

        float largestvalue = 0;
        int largestIndex = 0;


        for (int i = 0; i < largestDistance.Count; i++)
        {
            if (largestDistance[i] > largestvalue)
            {
                largestvalue = largestDistance[i];
                largestIndex = i;
            }
        }

        grid[largestIndex].SetState(NodeState.Final);
    }


    public int GetItemIndex(int x, int y)
    {
        if (x < 0 || x >= sizeX)
            return -1;
        else if (y < 0 || y >= sizeY)
            return -1;

        return (y * sizeX) + x;
    }
    public Vector2 GetItemLocation(int index)
    {
        if (index < 0 || index >= grid.Count)
            throw new IndexOutOfRangeException("Index is out of range");
        return new Vector2(index % sizeX, Mathf.CeilToInt(index / sizeX));
    }
}
