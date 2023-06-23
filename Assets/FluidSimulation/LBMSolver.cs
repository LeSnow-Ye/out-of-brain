using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

enum GirdType
{
    Wall,
    Space,
    ZouHeUp,
    ZouHeDown,
    ZouHeRight,
    ZouHeLeft,
}

public enum RenderMode
{
    Vorticity,
    Density,
    Velocity,
    Speed,
}

public class LBMSolver : MonoBehaviour
{
    [Header("LBM Solver")]
    public ComputeShader LBMSolverCs;
    public Material FluidSimulationMat;
    public int GridWidth = 512;
    public int GridHeight = 512;
    public float Tau = 1f;

    public int TargetFrameRate;

    public float V = 1f;
    public float U = 1f;
    public float RhoIn = 1f;
    public float RhoOut = 1f;

    public bool ShowDebugInfo = false;
    
    [Header("Render")]
    public RenderMode RenderMode = RenderMode.Speed;
    public float ParticleSize = 0.2f;
    public Material ParticleMaterial;
    public Texture GradientTexture;
    public int ParticleNum = 200;
    public int ParticleRefreshSpeed = 2;
    public float RenderIntensity = 0.5f;
    
    
    private int _step = 0;
    
    private int _phase1KernelId;
    private int _phase2KernelId;

    // LBM Buffers
    private ComputeBuffer _maskBuffer;
    private int[] _maskData;
    private ComputeBuffer _velocityBuffer;
    private Vector2[] _velocityData;
    private ComputeBuffer _populationsBuffer;
    private float[] _populationsData;
    private ComputeBuffer _densityBuffer;
    private float[] _densityData;
    private ComputeBuffer _debugBuffer;
    private float[] _debugData;
    
    // Render
    private RenderTexture _resultTexture;

    // Particles
    private Mesh _mesh;
    private ComputeBuffer _particlesBuffer;
    private Vector2[] _particlesData;
    private ComputeBuffer _argsBuffer;
    
    void InitMaskBuffer()
    {
        int dataLength = GridWidth * GridHeight;
        _maskData = new int[dataLength];
        for (int x = 1; x < GridWidth - 1; x++)
        {
            // _maskData[Pos2Index(x, 0)] = (int)GirdType.ZouHeDown;
            // _maskData[Pos2Index(x, GridHeight - 1)] = (int)GirdType.ZouHeUp;
            for (int y = 1; y < GridHeight - 1; y++)
            {
                _maskData[Pos2Index(0, y)] = (int)GirdType.ZouHeLeft;
                _maskData[Pos2Index(GridWidth - 1, y)] = (int)GirdType.ZouHeRight;

                /*if (new Vector2(x - GridWidth / 4.5f,y - GridHeight/2).magnitude <= GridHeight / 10f)
                {
                    continue;
                }*/
                
                _maskData[Pos2Index(x, y)] = (int)GirdType.Space;
            }
        }
        
        _maskBuffer = new ComputeBuffer(dataLength, sizeof(int));
        _maskBuffer.SetData(_maskData);
        LBMSolverCs.SetBuffer(_phase1KernelId, "Mask", _maskBuffer);
        LBMSolverCs.SetBuffer(_phase2KernelId, "Mask", _maskBuffer);
    }
    
    void InitVelocityBuffer()
    {
        int dataLength = GridWidth * GridHeight;
        _velocityData = new Vector2[dataLength];


        /*for (int i = 0; i < dataLength; i++)
        {
            if (_maskData[i] == 1)
            {
                _velocityData[i] = InitSpeed;
            }
        }*/

        _velocityBuffer = new ComputeBuffer(dataLength, 2 * 4);
        _velocityBuffer.SetData(_velocityData);
        LBMSolverCs.SetBuffer(_phase1KernelId, "Velocity", _velocityBuffer);
        LBMSolverCs.SetBuffer(_phase2KernelId, "Velocity", _velocityBuffer);
    }
    
    void InitPopulationsBuffer()
    {
        int dataLength = GridWidth * GridHeight * 9 * 2;
        _populationsData = new float[dataLength];
        _populationsBuffer = new ComputeBuffer(dataLength, 4);
        _populationsBuffer.SetData(_populationsData);
        LBMSolverCs.SetBuffer(_phase1KernelId, "Populations", _populationsBuffer);
        LBMSolverCs.SetBuffer(_phase2KernelId, "Populations", _populationsBuffer);
    }
    
    void InitDebugBuffer()
    {
        int dataLength = 10;
        _debugData = new float[dataLength];
        _debugBuffer = new ComputeBuffer(dataLength, 4);
        _debugBuffer.SetData(_debugData);
        LBMSolverCs.SetBuffer(_phase1KernelId, "DebugBuffer", _debugBuffer);
        LBMSolverCs.SetBuffer(_phase2KernelId, "DebugBuffer", _debugBuffer);
    }
        
    void InitDensityBuffer()
    {
        int dataLength = GridWidth * GridHeight;
        _densityData = new float[dataLength];
        for (int i = 0; i < dataLength; i++)
            _densityData[i] = 1f;

        _densityBuffer = new ComputeBuffer(dataLength, 4);
        _densityBuffer.SetData(_densityData);
        LBMSolverCs.SetBuffer(_phase1KernelId, "Density", _densityBuffer);
        LBMSolverCs.SetBuffer(_phase2KernelId, "Density", _densityBuffer);
    }

        
    void InitMesh()
    {
        _mesh = new Mesh();
        var offset = new Vector3(0.5f, 0.5f) * ParticleSize;
        var vertices = new []
        {
            new Vector3(0, 0, 0) - offset,
            new Vector3(ParticleSize, 0, 0) - offset,
            new Vector3(0, ParticleSize, 0) - offset,
            new Vector3(ParticleSize, ParticleSize, 0) - offset
        };
        _mesh.vertices = vertices;

        var triangles = new int[]
        {
            0, 2, 1,
            2, 3, 1
        };
        _mesh.triangles = triangles;

        var normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        _mesh.normals = normals;

        var uv = new Vector2[4]
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        };
        _mesh.uv = uv;
    }
    
    void InitParticles()
    {
        _particlesData = new Vector2[ParticleNum];
        for (int i = 0; i < ParticleNum; i++)
        {
            int x, y;
            do
            {
                x = Random.Range(0, GridWidth);
                y = Random.Range(0, GridHeight);
            } while (_maskData[Pos2Index(x, y)] != 1);
            _particlesData[i] = new Vector2(x * 16.0f / GridWidth - 8, y * 4.0f / GridHeight - 2);
        }
        
        _particlesBuffer = new ComputeBuffer(ParticleNum, 8);
        _particlesBuffer.SetData(_particlesData);
        LBMSolverCs.SetBuffer(_phase2KernelId, "Particles", _particlesBuffer);
        ParticleMaterial.SetBuffer("Particles", _particlesBuffer);
        
        uint[] args = new uint[5];
        args[0] = _mesh.GetIndexCount(0);
        args[1] = (uint)ParticleNum;
        args[2] = _mesh.GetIndexStart(0);
        args[3] = _mesh.GetBaseVertex(0);
        _argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        _argsBuffer.SetData(args);
    }

    void InitResultTexture()
    {
        _resultTexture = new RenderTexture(GridWidth,  GridHeight, 16) { enableRandomWrite = true };
        _resultTexture.Create();
        LBMSolverCs.SetTexture(_phase2KernelId, "Result", _resultTexture);
        FluidSimulationMat.mainTexture = _resultTexture;
    }

    void InitBuffers()
    {
        InitResultTexture();
        InitMaskBuffer();
        InitVelocityBuffer();
        InitPopulationsBuffer();
        InitDensityBuffer();
        InitDebugBuffer();
    }

    void PrintDebugInfo()
    {
        _debugBuffer.GetData(_debugData);
        Debug.Log("Step " + _step);        

        Debug.Log(_debugData[0].ToString());        
        Debug.Log(_debugData[1].ToString());        
        Debug.Log("density " + _debugData[2]);
        Debug.Log("Density " + _debugData[5]);
        Debug.Log("Density1 " + _debugData[3]);
        // Debug.Log("p_eq " + _debugData[4]);
    }
    
    void SolverPrepare()
    {
        _phase1KernelId = LBMSolverCs.FindKernel("CSPhase1");
        _phase2KernelId = LBMSolverCs.FindKernel("CSPhase2");
        LBMSolverCs.SetInt("GridWidth", GridWidth);
        LBMSolverCs.SetInt("GridHeight", GridHeight);
        LBMSolverCs.SetTexture(_phase2KernelId, "GradientTexture", GradientTexture);
    }

    void SolveAStep()
    {
        LBMSolverCs.SetFloat("Tau", Tau);
        LBMSolverCs.SetFloat("V", V);
        LBMSolverCs.SetFloat("U", U);
        LBMSolverCs.SetFloat("RhoIn", RhoIn);
        LBMSolverCs.SetFloat("RhoOut", RhoOut);
        LBMSolverCs.SetFloat("RenderMode", (int)RenderMode);
        LBMSolverCs.SetFloat("RenderIntensity", RenderIntensity);
        LBMSolverCs.SetInt("Step", _step++ % 2);
        LBMSolverCs.Dispatch(_phase1KernelId, GridWidth / 8, GridHeight / 8, 1);
        LBMSolverCs.Dispatch(_phase2KernelId, GridWidth / 8, GridHeight / 8, 1);
        
        if (ShowDebugInfo)
            PrintDebugInfo();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SolverPrepare();
        InitBuffers();
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = TargetFrameRate;
        SolveAStep();

    }

    private void OnDestroy()
    {
        ReleaseBuffers();
    }

    void ReleaseBuffers()
    {
        _velocityBuffer?.Release();
        _maskBuffer?.Release();
        _densityBuffer?.Release();
        _populationsBuffer?.Release();
        _debugBuffer?.Release();
        _particlesBuffer?.Release();
        _argsBuffer?.Release();
    }
    
    int Pos2Index(int x, int y) => y * GridWidth + x;
}
