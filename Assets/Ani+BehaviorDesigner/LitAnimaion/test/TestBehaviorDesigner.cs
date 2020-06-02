using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
public class TestBehaviorDesigner : MonoBehaviour
{
    public BehaviorTree tree;
    // Start is called before the first frame update
    void Start()
    {
        //var ttask = tree.FindTaskWithName("testtask");
        //tree.StartTaskCoroutine(ttask,"testmethod");
        tree.EnableBehavior();
        var ttask = tree.FindTaskWithName("testtask");
        Debug.Log(ttask);
        tree.StartTaskCoroutine(ttask,"testmethod");
    }
    public void testmethod()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
