using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
//?œê?

public struct FoodsAnimalsWant
{
    public bool burger, coke, coffee, donut;
    public int burgerNum, cokeNum, coffeeNum, donutNum;
    public AnimalSpawner.SpawnerType spawnerType;
}


public class AnimalSpawner : MonoBehaviour
{
   

    public AnimalManager animalManager;
    public int maxCustomer = 10;
    public enum SpawnerType
    {
        None = -1,
        FastFood = 0,
        DonutShop = 1,
        Delivery = 2,
        TakeOut = 3
    }
    public SpawnerType type;
    GameInstance instance = new GameInstance();
    // Start is called before the first frame update
    private void Awake()
    {
     
       
    }
    void Start()
    {
        instance.GameIns.workSpaceManager.spwaners.Add(this);


        if (animalManager.mode == AnimalManager.Mode.GameMode) StartCoroutine(Spawn());
    }

    public void RestartSpawner()
    {
        StartCoroutine(Spawn());
    }

    float coroutineTimer =0;
    float coroutineTimer2 =0;

    Customer deliveryCustomer;
  
    IEnumerator Spawn()
    {
        while (true)
        {
           
            WorkSpaceManager workSpaceManager = instance.GameIns.workSpaceManager;
            AnimalManager animalManager = instance.GameIns.animalManager;
            int n = 0;
            for(int i =0; i < animalManager.customerControllers.Count; i++)
            {
                if (animalManager.customerControllers[i].foodsAnimalsWant.spawnerType == type)
                {
                    n++;
                }
            }

            while (coroutineTimer <= 3f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
        

            // for(int i=0; i)
            if (n < maxCustomer)
            {
                switch (type)
                {



                    case SpawnerType.FastFood:
                        {
                            bool burger = false;
                            bool coke = false;

                            GetFoodUnlock(out burger, out coke, 0);

                            if (burger || coke)
                            {
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.burger = burger;
                                foodsAnimalsWant.coke = coke;
                                foodsAnimalsWant.spawnerType = SpawnerType.FastFood;
                                AnimalController ac = animalManager.SpawnCustomer(foodsAnimalsWant);
                              
                                ac.trans.position = transform.position;
                            }
                            break;
                        }
                    case SpawnerType.DonutShop:
                        {
                            bool coffee = false;
                            bool donut = false;

                            GetFoodUnlock(out coffee, out donut, 1);

                            if (coffee || donut)
                            {
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.coffee = coffee;
                                foodsAnimalsWant.donut = donut;
                                foodsAnimalsWant.spawnerType = SpawnerType.DonutShop;
                                AnimalController ac = animalManager.SpawnCustomer(foodsAnimalsWant);
                           
                                ac.trans.position = transform.position;
                            }
                            break;
                        }
                    case SpawnerType.TakeOut:
                        {
                            if (workSpaceManager.unlocks[2] > 0)
                            {
                                bool burger = false, coke = false, coffee = false, donut = false;
                                int rand = UnityEngine.Random.Range(0, 2);

                                if (rand == 0)
                                {
                                    int r = UnityEngine.Random.Range(0, 4);
                                    burger = r == 0 ? true : false; coke = r == 1 ? true : false; coffee = r == 2 ? true : false; donut = r == 3 ? true : false;
                                }
                                else
                                {
                                    bool[] b = new bool[4];
                                    int r = 0, rs = 0;
                                    while (r != rs)
                                    {
                                        r = UnityEngine.Random.Range(0, 4);
                                        rs = UnityEngine.Random.Range(0, 4);
                                    }
                                    b[r] = true;
                                    b[rs] = true;
                                    burger = b[0];
                                    coke = b[1];
                                    coffee = b[2];
                                    donut = b[3];
                                }
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.burger = burger;
                                foodsAnimalsWant.coke = coke;
                                foodsAnimalsWant.coffee = coffee;
                                foodsAnimalsWant.donut = donut;
                                foodsAnimalsWant.spawnerType = SpawnerType.TakeOut;
                                animalManager.SpawnCustomer(foodsAnimalsWant);

                            }
                            break;
                        }
                    case SpawnerType.Delivery:
                        {
                            if(deliveryCustomer != null)
                            {

                                while (deliveryCustomer.foodStacks[0].foodStack.Count != deliveryCustomer.foodStacks[0].needFoodNum)
                                {
                                    while(coroutineTimer2 <= 2f)
                                    {
                                        coroutineTimer2 += Time.deltaTime;
                                        yield return null;
                                    }
                                    coroutineTimer2 = 0;
                                }
                                instance.GameIns.animalManager.DespawnCustomer(deliveryCustomer, true);
                                deliveryCustomer = null;
                            }
                            else
                            {
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.burger = true;
                                foodsAnimalsWant.coke = true;
                                foodsAnimalsWant.spawnerType = SpawnerType.Delivery;
                               
                              
                                if (instance.GameIns.workSpaceManager.packingTables.Count == 2)
                                {

                                    deliveryCustomer = animalManager.SpawnCustomer(foodsAnimalsWant, true);
                                    for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                                    {
                                        //  Debug.Log(workSpaceManager.packingTables[i].counterType);
                                        if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery)
                                        {
                                            FoodStack foodStack = new FoodStack();
                                            foodStack.needFoodNum = 4;
                                            foodStack.type = FoodMachine.MachineType.PackingTable;
                                            deliveryCustomer.foodStacks.Add(foodStack);

                                            workSpaceManager.packingTables[i].customer = deliveryCustomer;
                                            deliveryCustomer.transform.position = new Vector3(workSpaceManager.packingTables[i].transform.position.x, 0, workSpaceManager.packingTables[i].transform.position.z - 10);

                                            instance.GameIns.uiManager.UpdateOrder(deliveryCustomer, Counter.CounterType.Delivery);
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            coroutineTimer = 0;
            //  yield return new WaitForSeconds(6);
        }
    }

    void GetFoodUnlock(out bool foodA, out bool foodB, int index)
    {
        foodA = false;
        foodB = false;
        WorkSpaceManager workSpaceManager = instance.GameIns.workSpaceManager;

        if (workSpaceManager.unlocks[index] == 1)
        {
            foodA = true;
        }
        if (workSpaceManager.unlocks[index] == 2)
        {
            int rand = UnityEngine.Random.Range(0, 3);

            if (rand == 0 || rand == 2) foodA = true;
            if (rand == 1 || rand == 2) foodB = true;

        }
    }
}
