using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

public static class Tools
{
    /*Position to number
     *  0
     * 3 1
     *  2
     */

    /* opp
     *  2
     * 1 3
     *  0
     */

    public enum Direction
    {
        Top,
        Right,
        Bottom,
        Left
    }
}

public enum NodeState
{
    Available,
    Current,
    Completed,
    Start,
    Final
}

public class Room : MonoBehaviour
{

    public List<GameObject> Walls = new List<GameObject>();
    public TextMeshProUGUI textPro;
    public SpriteRenderer floor;
    public NodeState state;


    //open = true then open wall
    // open = false then close wall
    public void modifyWall(Tools.Direction direction, bool open)
    {
        if (Walls[(int)direction].activeSelf == open)
            Walls[(int)direction].SetActive(!open);
        else Debug.LogError("Tried to modify room " + this.name + " but the wall in direction " + direction + " is already modified");
    }

    public void setGridPosition(Point p, int gridIndex)
    {
        textPro.text = p.ToString() + " " + System.Environment.NewLine + gridIndex;
    }

    public void SetState(NodeState state)
    {
        this.state = state;
        switch (state)
        {
            case NodeState.Available:
                floor.color = Color.white;
                break;
            case NodeState.Current:
                floor.color = Color.yellow;
                break;
            case NodeState.Completed:
                floor.color = Color.blue;
                break;
            case NodeState.Start:
                floor.color = Color.green;
                break;
            case NodeState.Final:
                floor.color = Color.red;
                break;
        }
    }

    public int numberOfWalls()
    {
        int closedWalls = 0;

        foreach (GameObject wall in Walls)
            if (wall.activeSelf) closedWalls++;

        return closedWalls;
    }

    public GameObject getWall(Tools.Direction direction)
    {
        return Walls[(int)direction];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
