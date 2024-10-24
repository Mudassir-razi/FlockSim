using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomataController : MonoBehaviour
{
    // Start is called before the first frame update

    public ComputeShader Automatan;

    [Range(0, 2000)]
    public int xMax;
    [Range(0, 2000)]
    public int yMax;
    [Range(0, 1.0f)]
    public float fill;
    [Range(0,10)]
    public int randomize;
    public bool enableTime;
    [Range(0.1f, 1.0f)]
    public float timeStep = 0.5f;

    [Header("Filter")]
    public Vector3[] convFilter;
   

    RenderTexture tex;
    RenderTexture filter;
    Texture2D _filter;
    float timePassed;

    void Start()
    {
        Init();   
    }

    void Init()
    {
        //creating textures
        tex = new RenderTexture(xMax, yMax, 24);
        tex.filterMode = FilterMode.Point;
        tex.enableRandomWrite = true;
        tex.Create();

        //creating filter
        _filter = new Texture2D(3, 3);
        _filter.filterMode = FilterMode.Point;
        for(int i = 0;i < 3;i++)
        {
            for(int j = 0;j < 3;j++)
            {
                Color c = new Color(1, 1, 1) * convFilter[i][j];
                _filter.SetPixel(i, j, c);
            }
        }
        _filter.Apply();

        filter = new RenderTexture(3, 3, 24);
        filter.filterMode = FilterMode.Point;
        filter.enableRandomWrite=true;
        Graphics.Blit(_filter, filter);
        filter.Create();

        Automatan.SetFloat("fill", fill);
        Automatan.SetInt("xMax", xMax);
        Automatan.SetInt("yMax", yMax);
        Automatan.SetInt("randomize", (int)Random.Range(1, 10));

        Automatan.SetTexture(1, "RenderTex", tex);
        Automatan.SetTexture(1, "Filter", filter);
        Automatan.Dispatch(1, xMax / 8, yMax / 8, 1);

        timePassed = 0.0f;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (enableTime)
        {
            if (timePassed > timeStep)
            {
                timePassed = 0.0f;
                Automatan.SetTexture(0, "RenderTex", tex);
                Automatan.SetTexture(0, "Filter", filter);
                Automatan.Dispatch(0, xMax / 8, yMax / 8, 1);
                Graphics.Blit(tex, destination);
            }
            timePassed += Time.deltaTime;
        }
        else
        {
            Automatan.SetTexture(0, "RenderTex", tex);
            Automatan.SetTexture(0, "Filter", filter);
            Automatan.Dispatch(0, xMax / 8, yMax / 8, 1);
            Graphics.Blit(tex, destination);
        }
        
    }


    public void Changed()
    {
        Init();
    }
}
