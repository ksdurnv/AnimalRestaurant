using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData 
{
    public int id;
    public int level;
    public int exp;
    public LevelData(int id, int level, int exp)
    {
        this.id = id;
        this.level = level;
        this.exp = exp;
    }
}
