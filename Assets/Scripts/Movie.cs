using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class Movie : MonoBehaviour
{
    public static Movie movie;
    public Transform SceneRoot;
    public Text subtitleText;
    public Canvas canvas;
    public Camera movieCamera;
    private string imgPath = "Movies/Images/";
    private float frameRate, frames;
    //private Vector2 canvasSize = new Vector2(10594, 5959);
    //private Dictionary<string, Sprite> PreloadedImages;
    private AnimatedLayer[] layersByIndex;
    private AnimatedLayer[] laysers;
    // Start is called before the first frame update
    private void Awake()
    {
        movie = this;
    }
    void Start()
    {
        DisableElements(false);
        PreloadLayers();
    }
    
    private void PreloadLayers()
    {
        layersByIndex = new AnimatedLayer[30];
        for(int i = 0; i < layersByIndex.Length; i++)
        {
            layersByIndex[i].gameObject = new GameObject();
            layersByIndex[i].gameObject.AddComponent<SpriteRenderer>();
            layersByIndex[i].gameObject.transform.parent = SceneRoot;
            layersByIndex[i].gameObject.transform.localPosition = Vector3.zero;
        }
        laysers = JsonParser.GetLayers(1.ToString() + "_");
    }

    public void PlayAnim(int num_)
    {
        Debug.Log("Play");
        MovieStopLevelStart(true);
        frameRate = JsonParser.startInfo.fr;
        Debug.Log("frameRate: " + frameRate);
        frames = JsonParser.startInfo.op;
        Debug.Log("frames: " + frames); 
        SetLayers(laysers);
        StartCoroutine(animateCoroutine());
    }

    public void MovieStopLevelStart(bool isMovieStart)
    {
        DisableElements(isMovieStart);
        Debug.Log("Movie stop");
        if (!isMovieStart)
        {
            StopAllCoroutines();
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true; 
            GameplayController.controller.StartLevel();
            
        }
        else
        {
            //Screen.autorotateToPortrait = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }
    private void DisableElements(bool isEnable)
    {
        movieCamera.gameObject.SetActive(isEnable);
        canvas.gameObject.SetActive(isEnable);
    }

    private void ClearLayers()
    {
        if (layersByIndex == null)
            return;
        for (int l = 0; l < SceneRoot.childCount; l++)
        {
            Destroy(SceneRoot.GetChild(l).gameObject);
        }
        layersByIndex = new AnimatedLayer[0];
    }

    private void SetLayers(AnimatedLayer[] layers)
    {
        //layersByIndex = new BodymovinLayer[layers.Length+1];
        for(int l = 0; l < layers.Length; l++)
        {
             
            
            if (layers[l].type == 2)//png
            {
                
                GameObject objectGO = layersByIndex[layers[l].ind - 1].gameObject;
                objectGO.transform.SetParent(SceneRoot, true);
                layersByIndex[layers[l].ind - 1].gameObject.layer = 12;

                layersByIndex[layers[l].ind - 1] = layers[l];
                layersByIndex[layers[l].ind - 1].gameObject = objectGO;
                //layers[l].gameObject = objectGO;
                SpriteRenderer spriteRenderer = objectGO.GetComponent<SpriteRenderer>();
                string imgNM = layers[l].nm.Remove(layers[l].nm.Length - 4);
                objectGO.name = imgNM;
                string pathSprite = imgPath + imgNM;
                spriteRenderer.sortingOrder = -layers[l].ind;//Обратная сортировка слоёв
                spriteRenderer.sprite = Resources.Load<Sprite>(pathSprite);
                spriteRenderer.drawMode = SpriteDrawMode.Sliced;
                spriteRenderer.size = new Vector2(layers[l].anchorPoint.x * 2f, Mathf.Abs(layers[l].anchorPoint.y) * 2f); //canvasSize;

                spriteRenderer.transform.localPosition = layers[l].position;
                float posZ = spriteRenderer.transform.position.z;
                if (Mathf.Abs(posZ) != 0 && Mathf.Abs(posZ) < 4f)
                    spriteRenderer.transform.position += new Vector3(0, 0, 1.5f* posZ/ Mathf.Abs(posZ));
                spriteRenderer.transform.localScale = layers[l].scale * .01f;
                spriteRenderer.color = new Color(1, 1, 1, layers[l].opacity * .01f);
                
                //StartCoroutine(timeCoroutine(layers[l]));
            }
            
            else if(layers[l].type == 5)//text
            {
                GameObject objectGO = layersByIndex[layers[l].ind - 1].gameObject;
                objectGO.name = layers[l].nm;
                layersByIndex[layers[l].ind - 1] = layers[l];
                layersByIndex[layers[l].ind - 1].gameObject = objectGO;
            }
            
            else if (layers[l].type == 13)//Camera
            {
                AnimatedLayer currLayer = layersByIndex[layers[l].ind - 1];
                GameObject objectGO = currLayer.gameObject;
                layersByIndex[layers[l].ind - 1] = layers[l];
                layersByIndex[layers[l].ind - 1].gameObject = objectGO;
                movieCamera.gameObject.transform.parent = currLayer.gameObject.transform;
                movieCamera.transform.localPosition = Vector3.zero;
                currLayer.gameObject.transform.localPosition = layers[l].position - layers[l].anchorPoint;
                objectGO.name = layers[l].nm;
                //BodymovinLayer camLayer = layers[l];
            }
            if(layers[l].type == 3)//NULL
            {
                AnimatedLayer currLayer = layersByIndex[layers[l].ind - 1];
                GameObject objectGO = currLayer.gameObject;
                layersByIndex[layers[l].ind - 1] = layers[l];
                layersByIndex[layers[l].ind - 1].gameObject = objectGO;

                currLayer.gameObject.transform.localPosition = layers[l].position - layers[l].anchorPoint;
                objectGO.name = layers[l].nm;
            }
        }
        //Set parents
        for (int l = 0; l < layersByIndex.Length; l++)
        {
            int p = layersByIndex[l].parent-1;
            GameObject currLayer = layersByIndex[l].gameObject;
            
            if (p < 0)
            {
                layersByIndex[l].gameObject.transform.parent = SceneRoot;

                continue;
            }
            GameObject parentLayer = layersByIndex[p].gameObject;
            currLayer.transform.parent = parentLayer.transform;
            currLayer.transform.localPosition = layersByIndex[l].position - layersByIndex[l].anchorPoint;
            currLayer.transform.localScale = layersByIndex[l].scale * .01f;
        }

    }

    IEnumerator animateCoroutine()
    {
        int timer = 0;
        while (++timer < frames)
        {
            //Debug.Log("timer: " + timer + " frameRate * frames: " + frameRate * frames);

            for (int l = 0; l < layersByIndex.Length; l++)
            {
                
                AnimatedLayer layerToMove = layersByIndex[l];
                if (layerToMove.type == 0 || layerToMove.positionSets == null)
                    continue;
                if (layerToMove.positionSets.Length > 0)
                {
                    Vector3 nextPos = DeltaPos(timer, layerToMove.positionSets, false);
                    layerToMove.gameObject.transform.localPosition = nextPos;
                    if (layerToMove.type == 13)//camera
                        layerToMove.gameObject.transform.localPosition -= layerToMove.anchorPoint;
                }


                if (layerToMove.type == 13)//camera
                {
                    /*
                    if (layerToMove.anchorSets.Length > 100)
                    {
                        Vector3 lookAtPoint = DeltaPos(timer, layerToMove.anchorSets, false);
                        Transform camTS = layerToMove.gameObject.transform;
                        camTS.LookAt(lookAtPoint, Vector3.up);
                        Debug.Log("lookAtPoint: " + lookAtPoint);
                    }
                    */
                    if ( layerToMove.orientationSets.Length > 0)
                    {
                        Vector3 orientAngles = DeltaPos(timer, layerToMove.orientationSets, true);
                        if(Screen.orientation == ScreenOrientation.Portrait)
                            layerToMove.gameObject.transform.localEulerAngles = orientAngles;
                        else
                            layerToMove.gameObject.transform.localEulerAngles = Vector3.zero;

                    }
                }

                if (layerToMove.opacitySets.Length > 0)
                {
                    float currOpacity = GetOpacity(timer, layerToMove.opacitySets) * .01f;
                    
                    if(layerToMove.type == 2 && currOpacity >= 0)//PNG
                    {
                        SpriteRenderer sprite = layerToMove.gameObject.GetComponent<SpriteRenderer>();
                        sprite.color = new Color(1, 1, 1, currOpacity);
                    }
                    if(layerToMove.type == 5 && currOpacity > 0)//TEXT
                    {
                        subtitleText.text = layerToMove.nm;
                        Color txtCol = subtitleText.color;
                        if (currOpacity <= .1f)
                            currOpacity = 0;
                        subtitleText.color = new Color(txtCol.r, txtCol.g, txtCol.b, currOpacity);
                    }
                }
                //Debug.DrawLine(camTS.position, lookAtPoint);


            }

            yield return new WaitForSeconds(1f / frameRate);
        }
        MovieStopLevelStart(false);
    }

    private float GetOpacity(int timer_, AnimatedProperties[] animProperties)
    {
        AnimatedProperties prevProp = animProperties[0], nextProp = new AnimatedProperties();
        for (int p = 0; p < animProperties.Length; p++)
        {
            if (animProperties[p].t <= timer_)
            {
                prevProp = animProperties[p];
            }
        }
        for (int p = 0; p < animProperties.Length; p++)
        {
            if (animProperties[p].t > timer_ || p == animProperties.Length - 1)
            {
                nextProp = animProperties[p];
                break;
            }
        }
        float timerMultiplier = (timer_ - prevProp.t) / (nextProp.t - prevProp.t);
        if (nextProp.t == prevProp.t)
        {
            timerMultiplier = 0;
        }
        return prevProp.sf + (nextProp.sf - prevProp.sf) * timerMultiplier;
    }

    private Vector3 DeltaPos(int timer_, AnimatedProperties[] animProperties, bool orientation_)
    {
        AnimatedProperties prevProp = animProperties[0], nextProp = new AnimatedProperties();
        for (int p = 0; p < animProperties.Length; p++)
        {
            if (animProperties[p].t <= timer_)
            {
                prevProp = animProperties[p];
            }
        }
        for (int p = 0; p < animProperties.Length; p++)
        {
            if (animProperties[p].t > timer_ || p == animProperties.Length-1)
            {
                nextProp = animProperties[p];
                break;
            }
        }
        float timerMultiplier = (timer_ - prevProp.t) / (nextProp.t - prevProp.t);
        if (timerMultiplier == Mathf.Infinity || timerMultiplier == Mathf.NegativeInfinity)
        {
            timerMultiplier = 1;
        }
        if(nextProp.t == prevProp.t)
        {
            timerMultiplier = 0;
        }

        Vector3 deltaV3 = prevProp.s + (nextProp.s - prevProp.s) * timerMultiplier;
        if(orientation_)//For camera
        {
            float x1, x2, y1, y2, z1, z2;
            x1 = prevProp.s.x < 0 ? prevProp.s.x + 360f : prevProp.s.x;
            x2 = nextProp.s.x < 0 ? nextProp.s.x + 360f : nextProp.s.x;
            float absX = Mathf.DeltaAngle(x1, x2);

            y1 = prevProp.s.y < 0 ? prevProp.s.y + 360f : prevProp.s.y;
            y2 = nextProp.s.y < 0 ? nextProp.s.y + 360f : nextProp.s.y;
            float absY = Mathf.DeltaAngle(y1, y2);

            z1 = prevProp.s.z < 0 ? prevProp.s.z + 360f : prevProp.s.z;
            z2 = nextProp.s.z < 0 ? nextProp.s.z + 360f : nextProp.s.z;
            float absZ = Mathf.DeltaAngle(z1, z2);

            Vector3 fromV3 = prevProp.s + new Vector3(absX, absY, absZ) * timerMultiplier;

            deltaV3 = fromV3;
        }
        
        return deltaV3;
    }
}

[System.Serializable]
public struct MovieInfo
{
    public string v;
    public float fr;
    public float ip;
    public float op;
    public int width;
    public int height;
    public string name;

    public MovieInfo(string name, string v, float fr, float ip, float op, int width, int height)
    {
        this.v = v;
        this.fr = fr;
        this.ip = ip;
        this.op = op;
        this.width = width;
        this.height = height;
        this.name = name;
    }
}
