using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class Customer : AnimalController
{
    public AnimalType animalType;
    public CustomerState customerState;

    public FoodsAnimalsWant foodsAnimalsWant;
    int currentWaypointIndex;
    bool hasMoney;
    float eatingTimer;
    //public Action restartCoroutine;

    private float speed;
    private float eatSpeed;
    private int minOrder;
    private int maxOrder;
    private void Start()
    {
        
    }
    public void Setup(FoodsAnimalsWant foodsAnimals)
    {
        hasMoney = true;
        currentWaypointIndex = 9;
        if (foodsAnimals.spawnerType == AnimalSpawner.SpawnerType.Delivery)
        {
            GameInstance instance = new GameInstance();
            WorkSpaceManager workSpaceManager = instance.GameIns.workSpaceManager;
            for (int i = 0; i < workSpaceManager.counters.Count; i++)
            {
                if (workSpaceManager.counters[i].counterType == Counter.CounterType.Delivery)
                {
                    workSpaceManager.counters[i].customer = this;
                    transform.position = workSpaceManager.counters[i].transform.position;
                    FoodStack foodStack = new FoodStack();
                    foodStack.needFoodNum = 8;
                    foodStack.type = FoodMachine.MachineType.PackingTable;
                    foodStacks.Add(foodStack);
                    instance.uiManager.UpdateOrder(this, Counter.CounterType.Delivery);
                }
            }
        }
        else
        {
            foodsAnimalsWant = foodsAnimals;           

            if (foodsAnimals.burger)
            {
                minOrder = gameObject.GetComponentInParent<Animal>().minOrder;
                maxOrder = gameObject.GetComponentInParent<Animal>().maxOrder;

                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = UnityEngine.Random.Range(minOrder, maxOrder);
                //foodStack.needFoodNum = 6;
                foodStack.type = FoodMachine.MachineType.BurgerMachine;
                foodStacks.Add(foodStack);
            }
            if (foodsAnimals.coke)
            {
                minOrder = gameObject.GetComponentInParent<Animal>().minOrder;
                maxOrder = gameObject.GetComponentInParent<Animal>().maxOrder;

                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = UnityEngine.Random.Range(minOrder, maxOrder);
                //foodStack.needFoodNum = 5;
                foodStack.type = FoodMachine.MachineType.CokeMachine;
                foodStacks.Add(foodStack);
            }
            if (foodsAnimals.coffee)
            {
                minOrder = gameObject.GetComponentInParent<Animal>().minOrder;
                maxOrder = gameObject.GetComponentInParent<Animal>().maxOrder;

                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = UnityEngine.Random.Range(minOrder, maxOrder);
                foodStack.type = FoodMachine.MachineType.CoffeeMachine;
                foodStacks.Add(foodStack);
            }
            if (foodsAnimals.donut)
            {
                minOrder = gameObject.GetComponentInParent<Animal>().minOrder;
                maxOrder = gameObject.GetComponentInParent<Animal>().maxOrder;

                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = UnityEngine.Random.Range(minOrder, maxOrder);
                foodStack.type = FoodMachine.MachineType.DonutMachine;
                foodStacks.Add(foodStack);
            }
            //   Debug.Log(foodsAnimalsWant.spawnerType);
        }
    }

    public List<Food> VisualizingFoodStack = new List<Food>();

    private void Update()
    {
        if (headPoint != null)
        {
            //for (int i = 0; i < foodStacks.Count; i++)
            //{
            //    for (int j = 0; j < foodStacks[i].foodStack.Count; j++)
            //    {
            //        foodStacks[i].foodStack[j].transform.position = headPoint.position + new Vector3(0, j * 0.7f, 0);
            //    }
            //}

            for(int i=0; i<VisualizingFoodStack.Count;i++)
            {
                VisualizingFoodStack[i].transform.position = headPoint.position + new Vector3(0, i * 0.7f, 0);
            }
        }
    }

    public void FindCustomerActions()
    {
        Vector3 target = new Vector3();
        WorkSpaceManager workSpaceManager = instance.GameIns.workSpaceManager;
        if (customerState != CustomerState.Table)
        {
            if (customerState == CustomerState.Walk)
            {
                busy = true;
                //         customerState = CustomerState.Counter;
           
                restartCoroutine += () => { CustomerPlayAction(workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType].queuePoints, workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType]); };
                CustomerPlayAction(workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType].queuePoints, workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType]);
                return;
            }
            else if (customerState == CustomerState.Counter)
            {
                List<Table> tableList = new List<Table>();
                for (int i = 0; i < workSpaceManager.tables.Count; i++) tableList.Add(workSpaceManager.tables[i]);
                tableList.Sort(delegate (Table a, Table b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });
                if (tableList.Count > 0)
                {

                    foreach (Table t in tableList)
                    {
                        if (t.isDirty == false)
                        {
                            for (int i = 0; i < t.seats.Length; i++)
                            {
                                if (t.seats[i].customer == null || t.seats[i].customer == this)
                                {
                                    busy = true;
                               //     customerState = CustomerState.Table;
                                    t.seats[i].customer = this;
                                    target = t.seats[i].transform.position;
                                    int capture = i;
                                    restartCoroutine += () => { CustomerPlayAction(target, t, capture); };
                                    CustomerPlayAction(target, t, i);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            busy = true;
            restartCoroutine += () => { CustomerPlayAction(); };
            CustomerPlayAction();
        }
        else
        {
            List<GameObject> endList = new List<GameObject>();
            for (int i = 0; i < workSpaceManager.endPoint.Count; i++) endList.Add(workSpaceManager.endPoint[i]);
            endList.Sort(delegate (GameObject a, GameObject b) { return (a.transform.position - trans.position).magnitude.CompareTo((b.transform.position - trans.position).magnitude); });
            busy = true;
            restartCoroutine += () => { CustomerPlayAction(endList[0].transform.position); };
            CustomerPlayAction(endList[0].transform.position);
        }
    }

    public void CustomerPlayAction()
    {
        StartCoroutine(CustomerWait());
    }
    public void CustomerPlayAction(QueuePoint[] position, Counter counter)
    {
        List<Table> tableList = new List<Table>();
        //   for (int i = 0; i < instance.GameIns.workSpaceManager.tables.Count; i++) tableList.Add(instance.GameIns.workSpaceManager.tables[i]);
        StartCoroutine(CustomerWalkToCounter(position, counter));
    }

    public void CustomerPlayAction(Vector3 position, Table table, int index)
    {
        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;
            StartCoroutine(CustomerWalkToTable(coord, table, index));
        }
        else
        {
            table.seats[index].customer = null;
            restartCoroutine = null;
            StartCoroutine(CustomerWait());
        }
    }

    public void CustomerPlayAction(Vector3 position)
    {
        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;
            StartCoroutine(CustomerGoHome(coord));
        }
        else
        {
            restartCoroutine = null;
            StartCoroutine(CustomerWait());
        }
    }

    IEnumerator CustomerWait()
    {
        while (coroutineTimer <= 1f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }
        coroutineTimer = 0f;
        busy = false;
        restartCoroutine = null;
    }

    IEnumerator CustomerWalkToCounter(QueuePoint[] position, Counter counter)
    {
        int i = currentWaypointIndex;//position.Length - 1;

        while (i >= 0)
        {
            Coord coord;
            while (true)
            {
                if (position[i].controller == null || position[i].controller == this)
                {
                    if (i + 1 < position.Length) position[i + 1].controller = null;
                    position[i].controller = this;

                    coord = CalculateCoords(position[i].gameObject.transform.position);

                    if (coord.r != 100 && coord.c != 100)
                    {
                        yield return StartCoroutine(AnimalMove(coord));
                    }
                    else
                    {
                    
                        yield return null;
                    }
                    break;
                }
                yield return null;
                animator.SetInteger("state", 0);
            }

            if (i == 0)
            {
                counter.customer = this;
                animator.SetInteger("state", 0);
                instance.GameIns.uiManager.UpdateOrder(this, counter.counterType);
            }
            i--;
            currentWaypointIndex = i;
            yield return null;
        }


        while (true)
        {
            //주문량을 충족함

            bool check = true;
            for (int j = 0; j < foodStacks.Count; j++)
            {
                if (foodStacks[j].needFoodNum != foodStacks[j].foodStack.Count)
                {
                    check = false;
                    break;
                }
            }
            if (check) break;
            yield return null;
        }

        float foodPrices = 0;
        int tipNum = 0;
        for (int j = 0; j < foodStacks.Count; j++)
        {
            for (int k = 0; k < foodStacks[j].foodStack.Count; k++)
            {
                foodPrices += foodStacks[j].foodStack[k].foodPrice;
                int tip = UnityEngine.Random.Range(1, 11);
                if (tip == 1) tipNum++;
            }
        }

        if(hasMoney)
        {
            instance.GameIns.restaurantManager.playerData.money += foodPrices;
            instance.GameIns.restaurantManager.playerData.fishesNum += tipNum;
            instance.GameIns.uiManager.UpdateMoneyText(instance.GameIns.restaurantManager.playerData.money);

            SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
            hasMoney = false;
        }
     
        List<Table> tables = instance.GameIns.workSpaceManager.tables;

        while (true)
        {
            while(coroutineTimer <= 0.5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0f;

            foreach (Table t in tables)
            {
                if (t.isDirty == false)
                {
                    for (int j = 0; j < t.seats.Length; j++)
                    {
                        if ((t.seats[j].customer == null || t.seats[j].customer == this))
                        {
                            counter.customer = null;
                            position[0].controller = null;
                            customerState = CustomerState.Counter;
                            busy = false;
                            restartCoroutine = null;
                            yield break;
                        }
                    }
                }
            }
           
        }
    }

    IEnumerator CustomerWalkToTable(Coord coord, Table table, int index)
    {
        yield return StartCoroutine(AnimalMove(coord));

        modelTrans.rotation = table.seats[index].transform.rotation;

        //for (int i = 0; i < foodStacks.Count; i++)
        //{
        //    while (foodStacks[i].foodStack.Count > 0)
        //    {
        //        Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];

        //        foodStacks[i].foodStack.Remove(f);
        //        float r = UnityEngine.Random.Range(1, 2.5f);

        //        if (table.foodStacks.Count > 0)
        //        {
        //            bool check = false;
        //            for (int j = 0; j < table.foodStacks.Count; j++)
        //            {
        //                if (table.foodStacks[j].type == foodStacks[i].type)
        //                {
        //                    check = true;
        //                }
        //            }
        //            if (check == false)
        //            {
        //                FoodStack foodStack = new FoodStack();
        //                foodStack.type = foodStacks[i].type;
        //                table.foodStacks.Add(foodStack);
        //            }
        //        }
        //        else
        //        {
        //            FoodStack foodStack = new FoodStack();
        //            foodStack.type = foodStacks[i].type;
        //            table.foodStacks.Add(foodStack);
        //        }
        //        for (int j = 0; j < table.foodStacks.Count; j++)
        //        {
        //            if (table.foodStacks[j].type == foodStacks[i].type)
        //            {
        //                f.transform.DOJump(table.transform.position + new Vector3(0, 0.7f * table.foodStacks[j].foodStack.Count, 0), r, 1, 0.2f);
        //                table.foodStacks[j].foodStack.Add(f);
        //                audioSource.Play();
        //            }
        //        }

        //        yield return new WaitForSeconds(0.1f);
        //    }
        //}

        while(VisualizingFoodStack.Count > 0)
        {
            Food f = VisualizingFoodStack[VisualizingFoodStack.Count - 1];
            VisualizingFoodStack.Remove(f);
            for(int i =0; i<foodStacks.Count; i++)
            {
                if (foodStacks[i].type == f.parentType)
                {
                    foodStacks[i].foodStack.Remove(f);
                    break;
                }
            }

            float r = UnityEngine.Random.Range(1, 2.5f);

            f.transform.DOJump(table.transform.position + new Vector3(0, 0.7f + table.foodStacks[0].foodStack.Count, 0), r, 1, 0.2f);
            table.foodStacks[0].foodStack.Add(f);
            audioSource.Play();

            yield return new WaitForSeconds(0.1f);
        }


        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
            if (foodStacks[i].foodStack.Count == 0)
            {
                foodStacks.RemoveAt(i);
            }
        }

        for (int i = 0; i < table.foodStacks.Count; i++)
        {

            while (table.foodStacks.Count > i)
            {

                if (table.foodStacks[i].foodStack.Count <= 0) break;

                while (true)
                {
                    eatingTimer += Time.deltaTime;

                    eatSpeed = gameObject.GetComponentInParent<Animal>().eatSpeed;

                    if (eatingTimer >= eatSpeed)
                    {
                       
                        break;
                    }

                    yield return null;
                }

                data.animal_eating_speed = 0.417f;
                animator.SetInteger("state", 2);

                GameObject particle = ParticleManager.CreateParticle();
                particle.gameObject.transform.position = mousePoint.position;
                particle.GetComponent<ParticleSystem>().Play();
                restartCoroutine += () => { ParticleManager.ClearParticle(particle);  };
               yield return new WaitForSeconds(data.animal_eating_speed);
             
                if (table.foodStacks.Count <= i) break;
                int foodIndex = table.foodStacks[i].foodStack.Count - 1;
                if (foodIndex < 0) break;
                Food f = table.foodStacks[i].foodStack[foodIndex];
                table.foodStacks[i].foodStack.Remove(f);
                table.numberOfGarbage++;
                FoodManager.EatFood(f);

                ParticleManager.ClearParticle(particle);
                animator.SetInteger("state", 0);
                eatingTimer = 0;
                //  yield return new WaitForSeconds(5f);
            }
        }


        //for (int i = table.foodStacks.Count - 1; i >= 0; i--)
        //{
        //    if (table.foodStacks[i].foodStack.Count == 0)
        //    {
        //        table.foodStacks.RemoveAt(i);
        //    }
        //}
        animator.SetInteger("state", 0);

        while(coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;

        //GameObject go = Instantiate(table.garbage);
        ////////쓰레기를 표현 할 에셋 추가, Garbage클래스 오브젝트 풀링////////
        table.seats[index].customer = null;

        bool bCustomerExist = true;
        for (int i = 0; i < table.seats.Length; i++)
        {
            if (table.seats[i].customer != null) bCustomerExist = false;
        }
        if (bCustomerExist)
        {
            table.numberOfGarbage = table.numberOfGarbage > 10 ? 10 : table.numberOfGarbage;
           // int n = table.numberOfGarbage > 10 ? 10 : table.numberOfGarbage;
            table.isDirty = true;

            for (int i = 0; i < table.numberOfGarbage; i++)
            {
                Garbage go = GarbageManager.CreateGarbage();
                go.transform.SetParent(table.trashPlate.transform);
                table.garbageList.Add(go);
                float x = UnityEngine.Random.Range(-1f, 1f);
                float z = UnityEngine.Random.Range(-1f, 1f);

                go.transform.position = table.up.position + new Vector3(x, 0, z);
            }

        }
        //////////////////////////////////////////////////////////////////////
        customerState = CustomerState.Table;
        busy = false;
        restartCoroutine = null;
    }

    IEnumerator CustomerGoHome(Coord coord)
    {
        if(coord != null) yield return StartCoroutine(AnimalMove(coord));
        restartCoroutine = null;
        instance.GameIns.animalManager.DespawnCustomer(this);
    }

  

    IEnumerator AnimalMove(Coord coord, bool continuous = false)
    {
        if (coord != null)
        {
            animator.SetInteger("state", 1);
            
            List<Coord> coords = GetCalculatedList(coord);
            int i = 0;
            Vector3 currentNode = trans.position;
            while (i < coords.Count)
            {
                Vector3 target = AnimalMovement(coord, ref i, coords);

                Vector3 dirs = (target - trans.position).normalized;
                float newMagnitude = (target - trans.position).magnitude;
                while (true)
                {
                    float magnitude = (target - trans.position).magnitude;
                    if (magnitude < 0.1f)
                    {
                        trans.position = target;
                        break;
                    }

                    speed = gameObject.GetComponentInParent<Animal>().speed;

                    trans.Translate(dirs * Time.deltaTime * speed, Space.World);
                    float angle = Mathf.Atan2(dirs.x, dirs.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    yield return null;
                }
                i++;

            }
            if (!continuous) animator.SetInteger("state", 0);
        }
    }

}
