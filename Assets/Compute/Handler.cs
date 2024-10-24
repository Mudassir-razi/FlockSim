using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handler : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture TrailTexture;

    [Range(8, 2048)]
    public int resX = 256;   //resolution of the map
    [Range (8, 2048)]
    public int resY = 256;
    [Range(1, 20)]
    public float x_max = 6;
    [Range(1, 20)]
    public float y_max = 6;
    [Range(16, 10000)]
    public int agentCount;
    [Range(0.0001f, 0.01f)]
    public float speed;
    [Range(0.00001f, 0.0004f)]
    public float decayRate;
    [Range(0, 1)]
    public int randomize;


    //stuff
    Agent[] agents;
    ComputeBuffer agentBuffer;
    Color[] pallete;

    private void Start()
    {
        Init();
    }

    private void CreatePallete()
    {
        pallete = new Color[5];
        float hue = Random.Range(0, 1.0f);
        float r1 = Random.Range(0, 0.5f);
        pallete[0] = new Color(hue - (r1 / 90.0f), Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        pallete[1] = new Color(hue - r1, Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        pallete[2] = new Color(hue , Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        pallete[3] = new Color(hue + r1 , Random.Range(0, 0.5f), Random.Range(0, 0.5f));
        pallete[4] = new Color(hue + (r1 / 90.0f), Random.Range(0, 0.5f), Random.Range(0, 0.5f));
    }

    private void Init()
    {
        //initializint agents with random position and direction
        agents = new Agent[agentCount];
        CreatePallete();
        //Debug.Log(pallete[0]);

        for (int i = 0; i < agentCount; i++)
        {
            Agent agent = new Agent();
            agent.col = pallete[Random.Range(0, 5)];
            agent.position = new Vector2(Random.Range(-x_max, x_max), Random.Range(-y_max, y_max));
            agents[i] = agent;
        }

        //creating buffer
        agentBuffer = new ComputeBuffer(agentCount, sizeof(float) * 6);
        agentBuffer.SetData(agents);


        //creating textures
        TrailTexture = new RenderTexture(resX, resY, 24);
        TrailTexture.filterMode = FilterMode.Trilinear;
        TrailTexture.enableRandomWrite = true;
        TrailTexture.Create();

        //feeding shader informations
        shader.SetFloat("timeStep", speed);
        shader.SetFloat("mapSizeX", resX);
        shader.SetFloat("mapSizeY", resY);
        shader.SetFloat("x_max", x_max);
        shader.SetFloat("y_max", y_max);

        shader.SetFloat("rMax", Random.Range(0, 100));
        shader.SetFloat("gMax", Random.Range(0, 200));
        shader.SetFloat("bMax", Random.Range(0, 200));

        shader.SetFloat("k", Random.Range(0.5f, 0.8f));
        shader.SetFloat("decayRate", decayRate);
        shader.SetFloat("omega1", Random.Range(0.2f, 5.0f));
        shader.SetFloat("omega2", Random.Range(0.5f, 5.0f));
        shader.SetBuffer(0, "agents", agentBuffer);

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shader.SetTexture(0, "TrailMap", TrailTexture);
        shader.SetTexture(1, "TrailMap", TrailTexture);

        shader.Dispatch(0, agentCount / 16, 1, 1);
        shader.Dispatch(1, resX / 8, resY / 8, 1);

        Graphics.Blit(TrailTexture, destination);
    }

    public void Changed()
    {
        agentBuffer.Dispose();
        Init();
    }

    public struct Agent
    {
        public Vector2 position;
        public Color col;
    }
}
