using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SerialGenerator : MonoBehaviour
{
    private const float GoldenAngle = 2.39996322972865332f;
    private float divergenceAngleToShow;
    
    private int fibonacci_a;
    private int fibonacci_b;
    
    public GameObject Shape;
    public int n;
    public float c;
    public float DivergenceAngle = 137.5f; // in degrees
    public float DivergenceAngleChangeSpeed = 1f;
    public float DivergenceAngleChangeScale = 1f;
    public float DivergenceGrowTime = 5f;
    public float GenerateInterval = 0.1f;

    public bool LiveUpdate = false;
    public bool EnableCustomDivergenceAngle = false;
    public bool FibonacciColoring = false;
    public bool BasicColoring = false;
    public bool UpdateDivergenceAngle = false;
    public bool DivergenceGrowAnimation = false;

    public List<Color> Palette;
    
    private Vector2 GetPosition(int i)
    {
        var r = c * Mathf.Sqrt(i);
        var divergenceAngle = EnableCustomDivergenceAngle ? Mathf.Deg2Rad * DivergenceAngle : GoldenAngle;
        if (UpdateDivergenceAngle)
            divergenceAngle += Mathf.Sin(Time.time * DivergenceAngleChangeSpeed) * DivergenceAngleChangeScale;

        divergenceAngleToShow = divergenceAngle;
        var theta = i * divergenceAngle;
        return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
    }
    
    public void InstantiateShape(int i)
    {
        var shape = Instantiate(Shape, GetPosition(i), Quaternion.identity, this.transform);

        if (BasicColoring)
            shape.GetComponent<SpriteRenderer>().color = Palette[i % Palette.Count];

        if (FibonacciColoring && i == fibonacci_a)
        {
            shape.GetComponent<SpriteRenderer>().color = Color.red;
            shape.transform.localScale = Vector2.one * 0.15f;
            (fibonacci_a, fibonacci_b) = (fibonacci_a + fibonacci_b, fibonacci_a);
        }
    }
    
    public void Generate(bool animation = false)
    {
        Clear();
        (fibonacci_a, fibonacci_b) = (1, 1);
        
        if (animation)
        {
            StartCoroutine(GenerateCoroutine());
        }
        else
        {
            for (int i = 0; i < n; i++)
                InstantiateShape(i);
        }
    }
    
    private IEnumerator GenerateCoroutine()
    {
        for (int i = 0; i < n; i++)
        {
            InstantiateShape(i);
            yield return new WaitForSeconds(GenerateInterval);
        }
    }
    
    public void Clear()
    {
        for (int i = transform.childCount; i > 0; --i)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LiveUpdate)
        {
            Generate();
        }

        if (DivergenceGrowAnimation)
        {
            EnableCustomDivergenceAngle = true;
            DivergenceAngle = SmoothStep(0, GoldenAngle * Mathf.Rad2Deg, (Time.time - 1f) / DivergenceGrowTime);
        }
    }

    private float SmoothStep(float min, float max, float x) => Mathf.Clamp01(x * x * x * (x * (x * 6 - 15) + 10)) * (max - min) + min;

    private void OnGUI()
    {
        var style = new GUIStyle
        {
            fontSize = 64,
            normal = new GUIStyleState{textColor = Color.white},
        };
        
        GUILayout.Label($"Divergence Angle: {divergenceAngleToShow * Mathf.Rad2Deg}°", style);
    }
}
