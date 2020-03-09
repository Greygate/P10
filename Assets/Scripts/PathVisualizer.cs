using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public GameObject pathVisualPrefab;

    List<Vector3> pathPositions = new List<Vector3>();
    List<GameObject> path = new List<GameObject>();

    public void CreatePath(Vector3[] pathPositions)
    {
        if (pathPositions.Length < 2)
            return;

        if (pathPositions.Length > 2)
        {
            Vector3 lastViaPoint = pathPositions[0];
            foreach (Vector3 viaPoint in pathPositions)
            {
                CreatePathPositionsBetween(lastViaPoint, viaPoint);
                lastViaPoint = viaPoint;
            }
        }
        else
            CreatePathPositionsBetween(pathPositions[0], pathPositions[1]);

        VisualizePath();
    }

    void CreatePathPositionsBetween(Vector3 startPos, Vector3 endPos)
    {
        Vector3 pathPos = startPos;
        float distance = Vector3.Distance(startPos, endPos);
        while (distance > .5f)
        {
            pathPositions.Add(pathPos);
            pathPos = Vector3.MoveTowards(pathPos, endPos, 1);
            distance = Vector3.Distance(pathPos, endPos);
        }
    }

    void VisualizePath()
    {
        ClearPath();
        foreach (Vector3 position in pathPositions)
            path.Add(Instantiate(pathVisualPrefab, position, Quaternion.identity));
    }

    void ClearPath()
    {
        foreach (GameObject pathObject in path)
            Destroy(pathObject);

        path.Clear();
    }
}
