using UnityEngine;
using System;

public class Goal : MonoBehaviour
{
    
    // The mouse object which is supposed to get to the goal
    [SerializeField]
    public GameObject mouse = null;
    
    // The radius in which the goal triggers
    [SerializeField]
    public float goalRadius = 1.0f;
    
    void Start()
    {
        // check if mouse object was properly set
        if (this.mouse == null)
            throw new NullReferenceException("The mouse object is null.");
    }
    
    void Update()
    {
        // check if the goal was reached
        if (Vector3.Distance(this.transform.position, this.mouse.transform.position) <= this.goalRadius)
            this.OnGoal();
    }

    // confetti

    // This method is called when the goal is reached
    void OnGoal()
    {
        Debug.Log("GOAL REACHED!!!!!");
    }
    
}
