
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Counter;
//using TMPro.EditorUtilities;
public class Counter : MonoBehaviour
{
    public enum CounterType
    {
        None,
        FastFood,
        Donuts,
        Delivery,
        TakeOut
    }

    public Employee employee;
    public Customer customer;
    public UIManager uiManager;
    public Customer Customer
    {
        get { return customer; }
        set { customer = value;}
    }

    public Transform workingSpot;
    public WorkingSpot[] workingSpot_SmallTables;

    public Transform[] stackPoints;
    public Transform smallTable;
    public Transform smallTable2;
    public QueuePoint[] queuePoints;

    public FoodMachine[] foodMachines; 
    public List<FoodStack> foodStacks = new List<FoodStack>();
    public CounterType counterType;
    public MoneyPile moneyPile;
    // Start is called before the first frame update
    void Start()
    {
        switch (counterType)
        {

            case CounterType.None:
                break;
            default:
                {
                    if (counterType == CounterType.FastFood || counterType == CounterType.TakeOut)
                    {
                        FoodStack burgerStack = new FoodStack();
                        burgerStack.type = FoodMachine.MachineType.BurgerMachine;
                        foodStacks.Add(burgerStack);
                        FoodStack cokeStack = new FoodStack();
                        cokeStack.type = FoodMachine.MachineType.CokeMachine;
                        foodStacks.Add(cokeStack);
                    }

                    if (counterType == CounterType.Donuts || counterType == CounterType.TakeOut)
                    {
                        FoodStack CoffeeStack = new FoodStack();
                        CoffeeStack.type = FoodMachine.MachineType.CoffeeMachine;
                        foodStacks.Add(CoffeeStack);
                        FoodStack DonutStack = new FoodStack();
                        DonutStack.type = FoodMachine.MachineType.DonutMachine;
                        foodStacks.Add(DonutStack);
                    }
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
