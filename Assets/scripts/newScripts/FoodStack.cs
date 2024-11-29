using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStack
{
    public FoodMachine.MachineType type;
    public List<Food> foodStack = new List<Food>();
    public int needFoodNum;
    public bool packaged;
}
