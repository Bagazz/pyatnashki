using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public event System.Action<Cell, Vector3, Vector3> OnPositionChanged;



    [SerializeField]
    private Text text;

    private Vector3 start;
    private Vector3 end;
    private float startTime;
    private bool moving;


    private int number;
    
    public int Number
    {
        get { return number; }
        set { number = value; text.text = number.ToString(); }
    }

    public void Move(float x, float y)
    {
        start = transform.position;
        end = new Vector3(x, transform.position.y, y);
        startTime = Time.time;
        moving = true;
    }

    void Start()
    {
        
    }


    void Update()
    {
        if (moving)
        {
            float t = (Time.time - startTime) / 0.25f;
            transform.position = Vector3.Lerp(start, end, t);
            if (t >= 1f)
            {
                moving = false;
                OnPositionChanged?.Invoke(this, start, end);
            }
        }
    }
}
