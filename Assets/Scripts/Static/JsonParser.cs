using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
public struct JsonParser
    {
        public static MovieInfo startInfo;

        public AnimatedLayer[] layers;
        public int highestLayerIndex;

        public static AnimatedLayer[] GetLayers(string name_)
        {
            string[] json = LoadFromFile(name_);
            AnimatedLayer[] animatedLayers = new AnimatedLayer[json.Length-1];
            startInfo = JsonUtility.FromJson<MovieInfo>(json[0]);
            for (int i = 1; i < json.Length; i++)
            {
                animatedLayers[i-1] = JsonUtility.FromJson<AnimatedLayer>(json[i]);
                
            }
            return animatedLayers;
        }

    private static string[] LoadFromFile(string fileName)
    {
        WWW wwwfile = new WWW(FileManager.getStreamAssetsPath() + fileName);
        
        List<string> strTemp = new List<string>();
        while (!wwwfile.isDone)
        {
            //strTemp.Add(reader.ReadLine());
        }
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
        File.WriteAllBytes(filepath, wwwfile.bytes);
        FileStream FS = File.OpenRead(filepath);

        StreamReader reader = new StreamReader(FS);
        while (!reader.EndOfStream)
        {
            strTemp.Add(reader.ReadLine());
        }
        reader.Close();

        return strTemp.ToArray();
    }

}


    public struct AnimatedLayer
    {
        public string id;
        public string refId;
        public float startTime;
        public int type;//13 - camera, 2 - png, 5 - fonts

        public string nm; 
        public int parent;
        public int ind;
        public GameObject gameObject;
        public Vector3 positionOffset;
        
        public Vector3 anchorPoint;
        public AnimatedProperties[] anchorSets;

        public Vector3 position;
        public AnimatedProperties[] positionSets;

        public Vector3 scale;
        public AnimatedProperties[] scaleSets;

        public float opacity;
        public AnimatedProperties[] opacitySets;
        public AnimatedProperties[] orientationSets;
    /*
    public Vector3 rotationEuler;
        public Quaternion rotation;
        public BodymovinAnimatedProperties[] rotationXSets;
        public BodymovinAnimatedProperties[] rotationYSets;
        public BodymovinAnimatedProperties[] rotationZSets;
    */
        public float inFrame;
        public float outFrame;
        public int blendMode;
    
    }

    [System.Serializable]
    public struct AnimatedProperties
    {
        public float t;
    /*
        public Vector3 ix;
        public Vector3 iy;
        public Vector3 ox;
        public Vector3 oy;

        public Vector2 i;
        public Vector2 o;
        public Vector3 ti;
        public Vector3 to;
    */
        public Vector3 s;
        public Vector3 e;
        public float sf;
        public float ef;
    }


    // Custom structures
    /*
    public struct BodyPoint
    {
        public Vector2 i;
        public Vector2 o;
        public Vector2 p;

        public BodyPoint(Vector2 point, Vector2 inPoint, Vector2 outPoint)
        {
            i = inPoint;
            o = outPoint;
            p = point;
        }
    }
    */
