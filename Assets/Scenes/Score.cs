using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Text

public class Score : MonoBehaviour
{
    public Text CatText;

    static public int Cat;
    //public static Score instance;

    // void Awake()
    // {
    //     instance=this;
    // }
    
    void Start()
    {
        Cat = 0;
        //FixedUpdateScore();
    }

    public void Update()
    {
        CatText.text = "Score : " + Cat;
    }
}
