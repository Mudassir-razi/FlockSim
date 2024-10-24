using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lines : MonoBehaviour
{
    public ComputeShader lineShader;
    public RenderTexture tex;

    //simulation parameters
    [Range(8, 2048)]
    public int resX = 256;   //resolution of the map
    [Range(8, 2048)]
    public int resY = 256;
    [Range(16, 500)]
    public int agentCount = 32;
    [Range(0, 1.0f)]
    public float timeStep = 0.001f;
    [Range(0, 80)]
    public float maxSpeed = 100;
    [Range(0, 3)]
    public float steerForce = 0.5f;
    [Range(1, 5)]
    public int sensorSize;
    [Range(0, 90)]
    public float sensorAngle;
    [Range(2, 10)]
    public float sensorOffset;
    [Range(0, 0.2f)]
    public float evaporateRate = 100;
    [Range(0, 50.10f)]
    public float randomness;
    [Range(1, 5)]
    public int groups = 2;
    [Range(0, 1000)]
    public float spawnRadius = 100.0f;

    Agent[] agents;
    ComputeBuffer agentBuffer;

    Color[] RawPallete;

    void Start()
    {
        Init();
    }

    private void CreatePallete()
    {
        RawPallete = new Color[5];
        float hue = Random.Range(0, 1.0f);
        float r1 = Random.Range(0, 0.5f);
        RawPallete[0] = new Color(hue - (r1 / 90.0f), Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        RawPallete[1] = new Color(hue - r1, Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        RawPallete[2] = new Color(hue, Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        RawPallete[3] = new Color(hue + r1, Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        RawPallete[4] = new Color(hue + (r1 / 90.0f), Random.Range(0, 0.5f), Random.Range(0, 0.5f));
    }

    void Init()
    {
        agents = new Agent[agentCount];
        CreatePallete();

        for (int i = 0; i < agentCount; i++)
        {
            Agent agent = new Agent();
            Vector2 screenCenter = new Vector2(resX / 2, resY / 2);
            int right = Random.Range(0, 2);
        
            int agentGroup = Random.Range(0, groups);
            agent.pos = Random.insideUnitCircle * spawnRadius + screenCenter;
            agent.vel = new Vector2(right , 1-right) * Mathf.Sign(Random.Range(-3.0f, 3.0f)) * maxSpeed;
            agent.group = RawPallete[agentGroup];
            agents[i] = agent;
        }

        //creating agent buffer
        agentBuffer = new ComputeBuffer(agentCount, sizeof(float) * 8);
        agentBuffer.SetData(agents);


        //creating textures
        tex = new RenderTexture(resX, resY, 24);
        tex.filterMode = FilterMode.Bilinear;
        tex.enableRandomWrite = true;
        tex.Create();

        //feeding shader info
        lineShader.SetFloat("timeStep", timeStep);
        lineShader.SetFloat("maxSpeed", maxSpeed);
        lineShader.SetFloat("steerForce", steerForce);
        lineShader.SetFloat("sensorOffset", sensorOffset);
        lineShader.SetFloat("sensorAngle", sensorAngle * Mathf.Deg2Rad);
        lineShader.SetFloat("steerForce", steerForce);
        lineShader.SetFloat("xMax", resX);
        lineShader.SetFloat("yMax", resY);
        lineShader.SetFloat("randomness", randomness);
        lineShader.SetInt("sensorSize", sensorSize);
        lineShader.SetFloat("evaporateRate", evaporateRate);
        lineShader.SetBuffer(0, "agents", agentBuffer);
    }

    public void Restart()
    {
        agentBuffer.Dispose();
        Init();
    }

    public void Change()
    {
        lineShader.SetFloat("timeStep", timeStep);
        lineShader.SetFloat("maxSpeed", maxSpeed);
        lineShader.SetFloat("steerForce", steerForce);
        lineShader.SetFloat("sensorOffset", sensorOffset);
        lineShader.SetFloat("sensorAngle", sensorAngle * Mathf.Deg2Rad);
        lineShader.SetFloat("steerForce", steerForce);
        lineShader.SetFloat("randomness", randomness);
        lineShader.SetInt("sensorSize", sensorSize);
        lineShader.SetFloat("evaporateRate", evaporateRate);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        lineShader.SetTexture(0, "RenderTex", tex);
        lineShader.SetTexture(1, "RenderTex", tex);

        lineShader.Dispatch(0, agentCount / 8, 1, 1);
        lineShader.Dispatch(1, resX / 8, resY / 8, 1);

        Graphics.Blit(tex, destination);
    }


    public struct Agent
    {
        public Vector2 pos;
        public Vector2 vel;
        public Color group;
    }
}
