using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoreGeneratio : MonoBehaviour
{
    [Header("ISLAND GENERATION SETTINGS------------")]
    [Header("Number:")]
    [SerializeField] private Vector2Int numRange = new Vector2Int(4, 8); private int minNum; private int maxNum;

    [Space(2)]
    [Header("Size:")]
    [SerializeField] private Vector2 sizeRange = new Vector2(1f, 5f); private float minSize; private float maxSize;
    
    [Space(2)]
    [Header("Spread:")]
    [SerializeField] private float spread = 3f;

    [Space(7)]

    [Header("PREFABS----------------------")]
    [SerializeField] private GameObject testing;
    
    //-------------------------------------------------------------------not accessible---------------------------------------------------------------
    private List<List<Vector2>> squares = new List<List<Vector2>>();


    //================================================methods==================================================================================
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        minNum = numRange.x;
        maxNum = numRange.y;

        minSize = sizeRange.x;
        maxSize = sizeRange.y;

        GenerateSquares();
        PlaceInstancesOnPoints();
    }

    private void GenerateSquares()
    {
        //-----------making first-------------------
        float size = minSize + (maxSize - minSize) * 0.75f;
        float halfDiagonal = size / Mathf.Sqrt(2);
        List<Vector2> firstSquare = new List<Vector2> {new Vector2(-halfDiagonal, halfDiagonal), new Vector2(halfDiagonal, halfDiagonal), new Vector2(halfDiagonal, -halfDiagonal), new Vector2( -halfDiagonal, -halfDiagonal)};
        squares.Add(firstSquare);


        int currentNum = Random.Range(minNum, maxNum);

        for (int i = 0; i < currentNum; i++)
        {
            Vector2 center = Vector2.zero + new Vector2(Random.Range(-size, size), Random.Range(-size, size));
            float currentSize = Random.Range(minSize, maxSize);

            halfDiagonal = currentSize / Mathf.Sqrt(2);
            List<Vector2> currentSquare = new List<Vector2> {center + new Vector2(-halfDiagonal, halfDiagonal) , center + new Vector2(halfDiagonal, halfDiagonal), center + new Vector2(halfDiagonal, -halfDiagonal), center + new Vector2( -halfDiagonal, -halfDiagonal)};
            
            float randomAngle = Random.Range(0f, 360f);    
            squares.Add(RotateSquarePoints(randomAngle, currentSquare));
        }

        //--------creating mesh---------------------
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        Mesh mesh = new Mesh();

        int index = 0;
        for (int i = 0; i < squares.Count; i++)
        {
            vertices.Add(new Vector3(squares[i][0].x, squares[i][0].y, 0f));
            vertices.Add(new Vector3(squares[i][1].x, squares[i][1].y, 0f));
            vertices.Add(new Vector3(squares[i][2].x, squares[i][2].y, 0f));
            vertices.Add(new Vector3(squares[i][3].x, squares[i][3].y, 0f));

            tris.AddRange(new List<int> { i * 4 + 0, i * 4 + 1, i * 4 + 2});
            tris.AddRange(new List<int> { i * 4 + 0, i * 4 + 2, i * 4 + 3});
            uvs.AddRange(new List<Vector2> { new Vector2(0, 1), new Vector2 (1, 1), new Vector2(1, 0), new Vector2(0, 0)});
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void PlaceInstancesOnPoints()
    {
        foreach (List<Vector2> current in squares)
        {
            Color _color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            foreach (Vector2 position in current)
            {
                GameObject newObj = Instantiate(testing, new Vector3(position.x, position.y, 1f), Quaternion.identity);
                newObj.GetComponent<SpriteRenderer>().color = _color;
                newObj.transform.localScale = new Vector3(2, 2, 2);
            }
        }
    }


    List<Vector2> RotateSquarePoints(float angleDegrees, List<Vector2> _points)
    {
        // Convert the angle to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Precompute sin and cos of the angle
        float cosTheta = Mathf.Cos(angleRadians);
        float sinTheta = Mathf.Sin(angleRadians);

        // Rotation matrix application
        for (int i = 0; i < _points.Count; i++)
        {
            Vector2 point = _points[i];

            // Apply rotation
            float xNew = cosTheta * point.x - sinTheta * point.y;
            float yNew = sinTheta * point.x + cosTheta * point.y;

            // Update the point with the new coordinates
            _points[i] = new Vector2(xNew, yNew);
        }

        return _points;
    }

}
