using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    public Transform[] viaPoints;

    public GameObject pathObject;

    Vector3 lastStartPos;
    Vector3 lastEndPos;

    List<Vector3> pathPositions = new List<Vector3>();
    List<GameObject> path = new List<GameObject>();

    void Start()
    {
        CreatePathPositions();
        VisualizePath();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            RandomizePositions();
            pathPositions.Clear();
            if (startPos.position != lastStartPos || endPos.position != lastEndPos)
            {
                CreatePathPositions();
            }
            
            if (pathPositions.Count > 0)
            {
                VisualizePath();
            }
        }
    }

    void RandomizePositions()
    {
        startPos.position = new Vector3(Random.Range(-5, 5), 0, 5);
        endPos.position = new Vector3(Random.Range(-5, 5), 0, -5);
    }

    void CreatePathPositions()
    {
        if (viaPoints.Length > 0)
        {
            Transform lastViaPoint = startPos;
            foreach (Transform viaPoint in viaPoints)
            {
                CreatePathPositionsBetween(lastViaPoint.position, viaPoint.position);
                lastViaPoint = viaPoint;
            }
            CreatePathPositionsBetween(lastViaPoint.position, endPos.position);
        }
        else
        {
            CreatePathPositionsBetween(startPos.position, endPos.position);
        }
    }

    void CreatePathPositionsBetween(Vector3 startPos, Vector3 endPos)
    {
        Vector3 pathPos = startPos;
        float distance = Vector3.Distance(startPos, endPos);
        while (distance > .5f)
        {
            print($"Current distance: {distance}");
            pathPositions.Add(pathPos);
            pathPos = Vector3.MoveTowards(pathPos, endPos, 1);
            distance = Vector3.Distance(pathPos, endPos);
        }
    }

    void VisualizePath()
    {
        ClearPath();
        foreach (Vector3 position in pathPositions)
            path.Add(Instantiate(pathObject, position, Quaternion.identity));
    }

    void ClearPath()
    {
        foreach (GameObject pathObject in path)
        {
            Destroy(pathObject);
        }
        path.Clear();
    }
}
