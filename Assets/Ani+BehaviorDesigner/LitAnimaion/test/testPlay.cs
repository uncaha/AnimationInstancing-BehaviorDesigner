using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AniPlayable;
public class testPlay : MonoBehaviour
{
    public int count = 1000;
    public GameObject pfb;
    public GameObject animatorPfb;
    public bool useInstancing = true;
   // public AniPlayable.InstanceAnimation.AnimationInstancing animator; 
    // Start is called before the first frame update
    void Start()
    {
        GameObject usepfb = null;
        if(useInstancing)
        {
            usepfb = pfb;
        }
        else
        {
            usepfb = animatorPfb;
        }
        Vector3 tpos = Vector3.zero;
        int tstep = 1;
        int tmaxline = (int)System.Math.Sqrt((double)count);
        for (int i = 0,j = 0,k = 0; i < count; i++)
        {
            GameObject tobj = GameObject.Instantiate(usepfb);
            tobj.transform.position = new Vector3(j * tstep,0,k * tstep);
            j++;
            if(j >= tmaxline)
            {
                j = 0;
                k++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI() {
        // if(GUI.Button(new Rect(0,100,100f,30),"add key1"))
        // {
        //     animator.SetInt("ani1",2);
        // }
        // if(GUI.Button(new Rect(0,0,100f,30f),"add key2"))
        // {
        //     animator.SetBool("ani2",true);
        // }
        // if(GUI.Button(new Rect(0,50,100f,30),"add key3"))
        // {
        //      animator.SetFloat("ani3",3);
        // }

        // if(GUI.Button(new Rect(120,100,100f,30),"remove key1"))
        // {
        //     animator.SetInt("ani1",0);
        // }
        // if(GUI.Button(new Rect(120,0,100f,30f),"remove key2"))
        // {
        //     animator.SetBool("ani2",false);
        // }
        // if(GUI.Button(new Rect(120,50,100f,30),"remove key3"))
        // {
        //      animator.SetFloat("ani3",0);
        // }

    }
}
