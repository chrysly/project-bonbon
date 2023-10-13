using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshGather : MonoBehaviour
{
    [SerializeField]
    private GameObject thing;
    
    public class Pair{
        public Renderer ren;
        public MeshFilter mf;
        public Pair(Renderer ren, MeshFilter mf) {
            this.ren = ren;
            this.mf = mf;
        }
    }
    void Start()
    {
        List<Pair> array = Collect(thing);
        print("I'M ALIVE");
    }

    public List<Pair> Collect(GameObject obj) {
        List<Pair> collected = new List<Pair>();
        Renderer[] rendererArray = obj.GetComponentsInChildren<Renderer>(true);
        foreach(Renderer ren in rendererArray) {
            collected.Add(new Pair(ren, ren.GetComponent<MeshFilter>()));
        }
        return collected;
    }
}
