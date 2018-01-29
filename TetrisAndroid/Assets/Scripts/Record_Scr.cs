using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record_Scr  {

    public string Name { get; set; }
    public int Points { get; set; }
    public Record_Scr(string name, int points)
    {
        Name = name;
        Points = points;
    }
}
