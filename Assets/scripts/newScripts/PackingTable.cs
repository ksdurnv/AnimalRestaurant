using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingTable : Counter
{
  //  public List<FoodStack> foodStack = new List<FoodStack>();
 //   public Transform[] stackPoints;
  //  public Transform workingSpot_SmallTables;
    public Transform packingTrans;
    public Transform endTrans;

    public FoodStack f1;
    public FoodStack f2;

    public FoodStack packageStack;

    public int packingNumber;
    public AnimalController employeeAssistant;

    // Start is called before the first frame update
    void Start()
    {
        f1 = new FoodStack();
        f1.type = FoodMachine.MachineType.BurgerMachine;
        
        foodStacks.Add(f1);

        f2 = new FoodStack();
        f2.type = FoodMachine.MachineType.CokeMachine;

        foodStacks.Add(f2);

        packageStack = new FoodStack();
        packageStack.type = FoodMachine.MachineType.PackingTable;

       // foodStack.Add(packageStack);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
