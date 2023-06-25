using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public float LineWidth;
    public Material LineMaterial;
    public int CapVertices = 5;
    public int CornerVertices = 5;

    private LineRenderer _lineRenderer;
    
    public void Draw(Vector3[] positions)
    {
        if (_lineRenderer is null) Init();

        _lineRenderer.positionCount = positions.Length;
        _lineRenderer.SetPositions(positions);
    }

    public void DrawNewPosition(Vector3 position)
    {
        if (_lineRenderer is null) Init();

        _lineRenderer.positionCount += 1;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, position);
    }

    public void Init()
    {
        _lineRenderer ??= gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = LineMaterial;
        _lineRenderer.widthMultiplier = LineWidth;
        _lineRenderer.numCapVertices = CapVertices;
        _lineRenderer.numCornerVertices = CornerVertices;
    }
    
    public void Clear()
    {
        if (_lineRenderer is null) return;

        _lineRenderer.positionCount = 0;
    }
    
    public void DrawFunction(Func<float, float> function, float xMin, float xMax, int sampleNum)
    {
        var positions = new Vector3[sampleNum];
        float stepLength = (xMax - xMin) / (sampleNum - 1);
        for (int i = 0; i < sampleNum; i++)
            positions[i] = new Vector3(xMin + i * stepLength, function(xMin + i * stepLength));
        Draw(positions);
    }    
    
    public void DrawFunctionAnimated(Func<float, float> function, float xMin, float xMax, int sampleNum, float animationTime = 1f)
    {
        if (animationTime < 0)
            throw new ArgumentException("animationTime must be non-negative");
        StartCoroutine(DrawFunctionCoroutine(function, xMin, xMax, sampleNum, animationTime));
    }

    private IEnumerator DrawFunctionCoroutine(Func<float, float> function, float xMin, float xMax, int sampleNum,
        float animationTime= 1f)
    {
        float stepLength = (xMax - xMin) / (sampleNum - 1);
        for (int i = 0; i < sampleNum; i++)
        {
            DrawNewPosition(new Vector3(xMin + i * stepLength, function(xMin + i * stepLength)));
            yield return new WaitForSeconds(animationTime / (sampleNum - 1));
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator Test()
    {
        DrawFunction(x => Mathf.Sin(x), -2 * Mathf.PI, 2 * Mathf.PI,100);
        yield return new WaitForSeconds(2f);
        Clear();
        DrawFunctionAnimated(x => Mathf.Cos(x), -2 * Mathf.PI, 2 * Mathf.PI,100);
        yield return new WaitForSeconds(3f);
        Clear();
        DrawFunctionAnimated(x => Mathf.Log(x + 5.01f), -5, 5,300, 2f);
        yield return new WaitForSeconds(3f);
        Clear();
        DrawFunctionAnimated(x => Mathf.Sin(1.4f * x) + 2 * Mathf.Cos(3 * x), -5, 5,300, 2f);

    }
}
