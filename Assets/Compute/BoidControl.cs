using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidControl : MonoBehaviour
{
    public ComputeShader boidShader;
    public RenderTexture tex;

    //simulation parameters
    [Range(8, 2048)]
    public int resX = 256;   //resolution of the map
    [Range(8, 2048)]
    public int resY = 256;
    [Range(1, 2048)]
    public float x_max = 6;
    [Range(1, 2048)]
    public float y_max = 6;
    [Range(16, 10000)]
    public int agentCount = 32;
    [Range(0, 0.01f)]
    public float timeStep = 0.001f;

    [Range(0, 1000)]
    public float separationDist = 25.0f;
    [Range(0, 1000)]
    public float maxSpeed = 2;
    [Range(0, 1000000)]
    public float maxForce = 2;

    [Range(0, 1)]
    public float separation = 0.1f;
    [Range(0, 1)]
    public float cohesion = 0.1f;
    [Range(0, 1)]
    public float alignment = 0.5f;
    [Range(0, 0.05f)]
    public float decay;
    [Range(0, 1)]
    public float blur;
    [Range(0, 1.0f)]
    public float alpha;
    [Range(1, 5)]
    public int group = 2;

    //stuff
    Agent[] agents;
    Colors[] pallete;
    Color[] RawPallete;
    ComputeBuffer agentBuffer;
    ComputeBuffer colorBuffer;
    

    void Start()
    {
        Init();   
    }

    private void CreatePallete()
    { 
        RawPallete = new Color[5];
        float hue = Random.Range(0, 1.0f);
        float r1 = Random.Range(0, 0.5f);

        RawPallete[0]= new Color(hue - (r1 / 90.0f), Random.Range(0, 0.5f), Random.Range(0, 0.5f), alpha);
        RawPallete[1] = new Color(hue - r1, Random.Range(0, 0.5f), Random.Range(0, 0.5f), alpha);
        RawPallete[2] = new Color(hue, Random.Range(0, 0.5f), Random.Range(0, 0.5f), alpha);
        RawPallete[3] = new Color(hue + r1, Random.Range(0, 0.5f), Random.Range(0, 0.5f), alpha);
        RawPallete[4] = new Color(hue + (r1 / 90.0f), Random.Range(0, 0.5f), Random.Range(0, 0.5f), alpha);
    }

    private void Init()
    {
        agents = new Agent[agentCount];
        pallete = new Colors[group];

        CreatePallete();

        for(int i = 0;i < group;i++)
        {
            Colors temp = new Colors();
            temp.pixel = RawPallete[i];
            pallete[i] = temp;
        }

        for (int i = 0; i < agentCount; i++)
        {
            Agent agent = new Agent();
            agent.pos = new Vector2(Random.Range(0, x_max), Random.Range(0, y_max));
            agent.vel = Random.insideUnitCircle * maxSpeed;
            agent.force = new Vector2(0, 0);
            agent.group = Random.Range(0, group);
            agents[i] = agent;
        }
        Debug.Log(agents[10].group);
        
        //creating agent buffer
        agentBuffer = new ComputeBuffer(agentCount, sizeof(float) * 7);
        agentBuffer.SetData(agents);

        //creating color pallete texture
        colorBuffer = new ComputeBuffer(group, sizeof(float) * 4);
        colorBuffer.SetData(pallete);

        //creating textures
        tex = new RenderTexture(resX, resY, 24);
        tex.filterMode = FilterMode.Bilinear;
        tex.enableRandomWrite = true;  
        tex.Create();

        //feeding shader info
        boidShader.SetFloat("timeStep", timeStep);
        boidShader.SetFloat("cohesion", cohesion);
        boidShader.SetFloat("agentCount", agentCount);
        boidShader.SetFloat("separation", separation);
        boidShader.SetFloat("alignment", alignment);
        boidShader.SetFloat("maxSpeed", maxSpeed);
        boidShader.SetFloat("maxForce", maxForce);
        boidShader.SetFloat("SeparationDist", separationDist);
        boidShader.SetFloat("mapSizeX", resX);
        boidShader.SetFloat("mapSizeY", resY);
        boidShader.SetFloat("x_max", x_max);
        boidShader.SetFloat("y_max", y_max);
        boidShader.SetFloat("decayRate", decay);
        boidShader.SetFloat("blur", blur);
        boidShader.SetFloat("colorMix", group);
        boidShader.SetBuffer(0, "agents", agentBuffer);
        boidShader.SetBuffer(0, "colorPallete", colorBuffer);

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        boidShader.SetTexture(0, "RenderTex", tex);
        boidShader.SetTexture(1, "RenderTex", tex);


        boidShader.Dispatch(0, agentCount / 16, 1, 1);
        boidShader.Dispatch(1, resX/8, resY/8, 1);
        Graphics.Blit(tex, destination);
    }

    public void Changed()
    {
        agentBuffer.Dispose();
        colorBuffer.Dispose();
        Init();
    }

    public struct Agent
    {
        public Vector2 pos;
        public Vector2 vel;
        public Vector2 force;
        public int group;
    }

    public struct Colors
    {
        public Color pixel;
    }
}
