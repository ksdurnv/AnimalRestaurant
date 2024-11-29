using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;

public class Employee : AnimalController
{
    public EmployeeState employeeState;
    public RewardingType rewardingType;

    public RewardsBox reward;
    public int maxWeight = 3;

    public List<Garbage> garbageList = new List<Garbage>();
    public GameObject garbage;

    private EmployeeData employeeData;
    public EmployeeData EmployeeData { get { return employeeData; } set { employeeData = value; if (ui != null) ui.UpdateLevel(employeeData.level); } }

    int exp;
    public int EXP
    {
        get { return exp; }
        set
        {
            if (value != 0)
            {
                int a = Mathf.Abs(exp - value);
                exp = value;
                if (ui != null) ui.UpdateEXP(a);
            }
            else
            {
                exp = 0;
                if (ui != null) ui.ClearEXP();
            }
        }
    }

    [HideInInspector]
    public bool spawning;

    // 직원 고용
    float x = 0;
    float z = 0;
    float length = 0;
    float elapsedTime = 0;
    Vector3 startPoint;
    Vector3 endPoint;
    Vector3 dir2;
    Vector3 controlVector;

 
    int step = 0;
    // Start is called before the first frame update
    void Start()
    {
      
        if (employeeData == null) employeeData = new EmployeeData(0, 3, 3, 4, 5);
    }
    // Update is called once per frame
    void Update()
    {

       
        if (headPoint != null)
        {
            for (int i = 0; i < foodStacks.Count; i++)
            {
                for (int j = 0; j < foodStacks[i].foodStack.Count; j++)
                {
                    foodStacks[i].foodStack[j].transform.position = headPoint.position + new Vector3(0, j * 0.7f, 0);
                }
            }
            for (int i = 0; i < garbageList.Count; i++)
            {
                garbageList[i].transform.position = headPoint.position + new Vector3(0, 0.5f * i, 0);
            }
        }
    }

    public void StartFalling(bool onlyFirst)
    {
        if (onlyFirst) CalculateFallingStrength();
        StartCoroutine(Falling());
    }

    void CalculateFallingStrength()
    {
        elapsedTime = 0f;

        x = 0;
        z = 0;
        length = 0;

        while (true)
        {
            x = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            z = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            length = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            //  length = Random.Range(-10,10);
            Vector3 test = new Vector3(instance.GameIns.inputManager.cameraRange.position.x, 0, instance.GameIns.inputManager.cameraRange.position.z);
            Vector3 n = new Vector3(x, 0, z).normalized;
            Vector3 ta = test + n * length;

            if (Physics.Raycast(ta + Vector3.up * 5, Vector3.down, 5))
            {
                Debug.DrawLine(ta + Vector3.up * 10, test + Vector3.down * 100f, Color.red, 100);

                if (Physics.CheckBox(ta, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                {

                    Debug.Log("blockObject");
                }
                else break;
            }
        }
        startPoint = trans.position; //start.position;

        endPoint = new Vector3(instance.GameIns.inputManager.cameraRange.position.x, 0, instance.GameIns.inputManager.cameraRange.position.z);
        dir2 = new Vector3(x, 0, z).normalized;

        endPoint += dir2 * length;
        controlVector = (startPoint + endPoint) / instance.GameIns.restaurantManager.weight + Vector3.up * instance.GameIns.restaurantManager.height;

        instance.GameIns.restaurantManager.flyingEndPoints.Add(endPoint);
    }

    IEnumerator Falling()
    {
        animator.SetInteger("state", 3);
     
        bool bstart = true;

        while (bstart)
        {

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / instance.GameIns.restaurantManager.duration);

            Vector3 origin = trans.position;
            // Vector3 targetLoc = Vector3.Lerp(startPoint, endPoint, t);// CalculateBezierPoint(t, startPoint, controlVector, endPoint);
            Vector3 targetLoc = CalculateBezierPoint(t, startPoint, controlVector, endPoint);

            Vector3 dir = targetLoc - origin;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Debug.DrawLine(trans.position, targetLoc, Color.red, 5f);
            trans.position = targetLoc;


            if (t >= 1.0f)
            {
               trans.position = endPoint;
                bstart = false;
            }

            yield return null;
        }
        yield return null;
        animator.SetInteger("state", 0);
        spawning = false;
        busy = false;

    }
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0; // 시작점
        point += 2 * u * t * p1; // 제어점
        point += tt * p2; // 끝점

        return point;
    }

    public void FindEmployeeWorks()
    {
        Vector3 target = new Vector3();
        WorkSpaceManager workSpaceManager = instance.GameIns.workSpaceManager;

        if (employeeState != EmployeeState.TrashCan)
        {
            bool isPackaged = false;
            int foodNum = 0;
            for (int i = 0; i < foodStacks.Count; i++)
            {
                foodNum += foodStacks[i].foodStack.Count;
                isPackaged = foodStacks[i].packaged;
            }

            if (foodNum > 0)
            {
                if (employeeState == EmployeeState.FoodMachine)
                {
                    if (!isPackaged)
                    {
                        //음식을 카운터로
                        List<FoodStack> stacks = new List<FoodStack>();
                        List<Counter> counterList = new List<Counter>();
                        for (int i = 0; i < workSpaceManager.counters.Count; i++)
                        {
                            for (int j = 0; j < foodStacks.Count; j++)
                            {
                                for (int k = 0; k < workSpaceManager.counters[i].foodStacks.Count; k++)
                                {
                                    if (foodStacks[j].type == workSpaceManager.counters[i].foodStacks[k].type)
                                    {
                                        counterList.Add(workSpaceManager.counters[i]);
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                        {
                            for (int j = 0; j < foodStacks.Count; j++)
                            {
                                for (int k = 0; k < workSpaceManager.packingTables[i].foodStacks.Count; k++)
                                {
                                    if (foodStacks[j].type == workSpaceManager.packingTables[i].foodStacks[k].type)
                                    {
                                        counterList.Add(workSpaceManager.packingTables[i]);
                                    }
                                }
                            }
                        }

                        counterList.Sort(delegate (Counter a, Counter b)
                        {

                            int aa = 0;
                            int bb = 0;
                            for (int i = 0; i < a.foodStacks.Count; i++)
                            {
                                for (int k = 0; k < foodStacks.Count; k++)
                                {
                                    if (a.foodStacks[i].type == foodStacks[k].type)
                                    {
                                        aa = i;

                                    }
                                }

                            }

                            for (int i = 0; i < b.foodStacks.Count; i++)
                            {
                                for (int k = 0; k < foodStacks.Count; k++)
                                {
                                    if (b.foodStacks[i].type == foodStacks[k].type)
                                    {
                                        bb = i;

                                    }
                                }

                            }
                            return (a.foodStacks[aa].foodStack.Count.CompareTo(b.foodStacks[bb].foodStack.Count));
                        });

                        if (counterList.Count > 0)
                        {
                            foreach (Counter counter in counterList)
                            {
                                if (counter.counterType == Counter.CounterType.Delivery || counter.counterType == Counter.CounterType.None)
                                {
                                    if (counter.counterType == Counter.CounterType.None)
                                    {

                                        
                                        PackingTable pt = (PackingTable)counter;

                                        for(int i=0; i< pt.workingSpot_SmallTables.Length; i++)
                                        { 
                                            for(int j=0; j<foodStacks.Count; j++)
                                            {
                                                if (pt.workingSpot_SmallTables[i].type == foodStacks[j].type)
                                                {
                                                    employeeState = EmployeeState.Counter;
                                                    target = pt.workingSpot_SmallTables[i].transform.position;
                                                    restartCoroutine += () => { Work(target, pt, 0); };
                                                    Work(target, pt, 0);
                                                }

                                            }
                                        }
                                        return;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < counter.workingSpot_SmallTables.Length; i++)
                                    {
                                        for (int j = 0; j < foodStacks.Count; j++)
                                        {
                                            if (counter.workingSpot_SmallTables[i].type == foodStacks[j].type)
                                            {
                                                employeeState = EmployeeState.Counter;
                                                target = counter.workingSpot_SmallTables[i].transform.position;
                                                restartCoroutine += () => { Work(target, counter, false); };
                                                Work(target, counter, false);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //기다린다
                        employeeState = EmployeeState.Wait;
                        restartCoroutine += () => { Work(); };
                        Work();
                        return;
                    }
                }
                else
                {

                }
            }
            else
            {
                if (reward != null)
                {
                    if (reward.foods.Count > 0)
                    {
                        restartCoroutine += () => { Reward(reward.transform.position); };
                        Reward(reward.transform.position);
                        return;
                    }
                }

                //음식을 서빙
                List<Counter> counterList = new List<Counter>();
                for (int i = 0; i < workSpaceManager.counters.Count; i++) counterList.Add(workSpaceManager.counters[i]);
                counterList.Sort(delegate (Counter a, Counter b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });
                if (counterList.Count > 0)
                {
                    foreach (Counter counter in counterList)
                    {
                        if (counter.customer && (counter.employee == null || counter.employee == this))
                        {
                            for (int i = 0; i < counter.customer.foodStacks.Count; i++)
                            {
                                for (int j = 0; j < counter.foodStacks.Count; j++)
                                {
                                    if (counter.customer.foodStacks[i].type == counter.foodStacks[j].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                                    {
                                        counter.employee = this;
                                        employeeState = EmployeeState.Serving;
                                        target = counter.workingSpot.position;
                                        restartCoroutine += () => { Work(target, counter, true); };
                                        Work(target, counter, true);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                //배달
                List<PackingTable> packingTables = new List<PackingTable>();
                for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                {
                    int d = i;
                    if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery && (workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this) && workSpaceManager.packingTables[i].customer != null)
                    {
                        if (workSpaceManager.packingTables[i].packageStack.foodStack.Count >= workSpaceManager.packingTables[i].customer.foodStacks[0].needFoodNum)
                        {
                            workSpaceManager.packingTables[i].employee = this;
                            restartCoroutine += () => { Work(workSpaceManager.packingTables[d].workingSpot.position, workSpaceManager.packingTables[d], 3); };
                            Work(workSpaceManager.packingTables[i].workingSpot.position, workSpaceManager.packingTables[i], 3);
                            return;
                        }
                    }

                }

                //테이블을 청소
                List<Table> tableList = new List<Table>();
                for (int i = 0; i < workSpaceManager.tables.Count; i++) tableList.Add(workSpaceManager.tables[i]);
                tableList.Sort(delegate (Table a, Table b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });

                if (tableList.Count > 0)
                {
                    foreach (Table table in tableList)
                    {
                        if (table.isDirty && !table.interacting && (table.employeeContoller == null || table.employeeContoller == this))
                        {
                            table.employeeContoller = this;
                            employeeState = EmployeeState.Table;
                            target = table.cleanSeat.position;
                            restartCoroutine += () => { Work(target, table); };
                            Work(target, table);
                            return;
                        }
                    }
                }

                // 손님 우선 조달

                List<FoodMachine> finalMachineList = new List<FoodMachine>();
                List<FoodStack> foodStacks = new List<FoodStack>();
                for (int i = 0; i < workSpaceManager.counters.Count; i++)
                {
                    for (int j = 0; j < workSpaceManager.counters[i].foodStacks.Count; j++) foodStacks.Add(workSpaceManager.counters[i].foodStacks[j]);
                }

                for (int i = 0; i < workSpaceManager.counters.Count; i++)
                {
                    if (workSpaceManager.counters[i].customer != null)
                    {
                        AnimalController customer = workSpaceManager.counters[i].customer;
                        //if(customer.food1 > customer.foodList.Count)
                        for (int j = 0; j < customer.foodStacks.Count; j++)
                        {
                            for (int k = 0; k < workSpaceManager.foodMachines.Count; k++)
                            {
                                if (customer.foodStacks[j].type == workSpaceManager.foodMachines[k].machineType && workSpaceManager.foodMachines[k].foodStack.foodStack.Count + customer.foodStacks[j].foodStack.Count >= customer.foodStacks[j].needFoodNum && (workSpaceManager.foodMachines[k].employee == null || workSpaceManager.foodMachines[k].employee == this) && customer.foodStacks[j].needFoodNum > customer.foodStacks[j].foodStack.Count)
                                {
                                    finalMachineList.Add(workSpaceManager.foodMachines[k]);
                                }
                            }

                            if (finalMachineList.Count > 0)
                            {
                                finalMachineList.Sort(delegate (FoodMachine a, FoodMachine b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((a.gameObject.transform.position - trans.position).magnitude); });
                                finalMachineList[0].employee = this;
                                employeeState = EmployeeState.FoodMachine;
                                target = finalMachineList[0].workingSpot.position;
                                restartCoroutine += () => { Work(target, finalMachineList[0]); };
                                Work(target, finalMachineList[0]);
                                return;
                            }
                        }
                    }
                }


                //포장된 음식을 배달 테이블로 이동
                for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                {
                    if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.None && workSpaceManager.packingTables[i].packageStack.foodStack.Count > 0 && (workSpaceManager.packingTables[i].employeeAssistant == null || workSpaceManager.packingTables[i].employeeAssistant == this))
                    {
                        for (int j = 0; i < workSpaceManager.packingTables.Count; j++)
                        {
                            if (i != j && workSpaceManager.packingTables[j].counterType == Counter.CounterType.Delivery)
                            {
                                workSpaceManager.packingTables[i].employeeAssistant = this;
                                employeeState = EmployeeState.FoodMachine;
                                target = workSpaceManager.packingTables[i].endTrans.position;
                                restartCoroutine += () => { Work(target, workSpaceManager.packingTables[i], 2); };
                                Work(target, workSpaceManager.packingTables[i], 2);
                                return;
                            }
                        }
                    }
                }



                //포장테이블 포장

                List<FoodStack> newFoodStacks = new List<FoodStack>();
                int n = 0;
                for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                {
                    if ((workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this) && workSpaceManager.packingTables[i].counterType == Counter.CounterType.None)
                    {
                        n += workSpaceManager.packingTables[i].packingNumber;
                        for (int j = 0; j < workSpaceManager.packingTables[i].foodStacks.Count; j++)
                        {
                            newFoodStacks.Add(workSpaceManager.packingTables[i].foodStacks[j]);
                        }
                    }
                }


                for (int i = 0; i < newFoodStacks.Count; i++) n += newFoodStacks[i].foodStack.Count;

                if (n >= 4)
                {


                    newFoodStacks.Sort(delegate (FoodStack a, FoodStack b) { return a.foodStack.Count.CompareTo(b.foodStack.Count); });


                    for (int j = 0; j < newFoodStacks.Count; j++)
                    {
                        for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                        {
                            for (int k = 0; k < workSpaceManager.packingTables[i].foodStacks.Count; k++)
                            {
                                if (newFoodStacks[j].type == workSpaceManager.packingTables[i].foodStacks[k].type && (workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this))
                                {
                                    workSpaceManager.packingTables[i].employee = this;
                                    employeeState = EmployeeState.FoodMachine;
                                    target = workSpaceManager.packingTables[i].packingTrans.position;

                                    restartCoroutine += () => { Work(target, workSpaceManager.packingTables[i], 1); };
                                    Work(target, workSpaceManager.packingTables[i], 1);

                                    return;
                                }
                            }
                        }
                    }
                }

                //일반 조달
                foodStacks.Clear();
                for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                {
                    for (int j = 0; j < workSpaceManager.packingTables[i].foodStacks.Count; j++)
                    {
                        foodStacks.Add(workSpaceManager.packingTables[i].foodStacks[j]);
                    }
                }

                for (int i = 0; i < workSpaceManager.counters.Count; i++)
                {
                    for (int j = 0; j < workSpaceManager.counters[i].foodStacks.Count; j++)
                    {
                        foodStacks.Add(workSpaceManager.counters[i].foodStacks[j]);
                    }
                }


                foodStacks.Sort(delegate (FoodStack a, FoodStack b) { return (a.foodStack.Count.CompareTo(b.foodStack.Count)); });

                List<FoodMachine> foodMachineList = new List<FoodMachine>();
                for (int i = 0; i < workSpaceManager.foodMachines.Count; i++) foodMachineList.Add(workSpaceManager.foodMachines[i]);

                finalMachineList.Clear();


                for (int i = 0; i < foodStacks.Count; i++)
                {
                    for (int j = 0; j < foodMachineList.Count; j++)
                    {
                        if (foodMachineList[j].machineType == foodStacks[i].type && foodMachineList[j].foodStack.foodStack.Count > 0 && (foodMachineList[j].employee == null || foodMachineList[j].employee == this))
                        {
                            finalMachineList.Add(foodMachineList[j]);
                        }
                    }
                    if (finalMachineList.Count > 0) break;
                }
                //Debug.Log(finalMachineList.Count);
                finalMachineList.Sort(delegate (FoodMachine a, FoodMachine b) { return a.foodStack.foodStack.Count.CompareTo((b.foodStack.foodStack.Count)); });
                finalMachineList.Reverse();
                if (finalMachineList.Count > 0)
                {
                    foreach (FoodMachine machine in finalMachineList)
                    {
                        if (machine.foodStack.foodStack.Count > 0 && (machine.employee == null || machine.employee == this))
                        {
                            machine.employee = this;
                            employeeState = EmployeeState.FoodMachine;
                            target = machine.workingSpot.position;
                            restartCoroutine += () => { Work(target, machine); };
                            Work(target, machine);
                            return;
                        }
                    }
                }

                //기다리기
                employeeState = EmployeeState.Wait;
                restartCoroutine += () => { Work(); };
                Work();
            }
        }
        else
        {
            //쓰레기를 버림
            List<TrashCan> trashCanList = new List<TrashCan>();
            for (int i = 0; i < workSpaceManager.trashCans.Count; i++) trashCanList.Add(workSpaceManager.trashCans[i]);
            trashCanList.Sort(delegate (TrashCan a, TrashCan b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });

            foreach (TrashCan c in trashCanList)
            {
                restartCoroutine += () => { Work(c.throwPos.position, c); };
                Work(c.throwPos.position, c);
                return;
            }
        }
    }

    public void Work()
    {
        int r = UnityEngine.Random.Range(0, 2);

        if (r == 0) StartCoroutine(EmployeeWait());
        else StartCoroutine(EmployeeWait_Patrol());
    }

    public void Work(Vector3 position, PackingTable packingTable, int i)
    {
        if(i == 3 && step != 0)
        {
            Coord temp = new Coord(1,1);
            StartCoroutine(EmployeeDeliveryFoods( temp, packingTable));
            return;
        }

        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;

            if (i == 0) StartCoroutine(EmployeePackingTable(coord, packingTable));
            else if (i == 1) StartCoroutine(EmployeeNowPacking(coord, packingTable));
            else if (i == 2) StartCoroutine(EmployeeGetPackagedFoods(coord, packingTable));
            else if (i == 3) StartCoroutine(EmployeeDeliveryFoods(coord, packingTable));

        }
        else
        {
            if (i == 1 || i == 3)
            {
                packingTable.employee = null;
                restartCoroutine = null;
            }
            StartCoroutine(EmployeeWait());
    
        }
    }

    public void Work(Vector3 position, FoodMachine foodMachine)
    {
        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;
            StartCoroutine(EmployeeFoodMachine(coord, foodMachine));
        }
        else
        {
            foodMachine.employee = null;
            restartCoroutine = null;
            StartCoroutine(EmployeeWait());
        }
    }

    public void Work(Vector3 position, Counter counter, bool serving)
    {
        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;

            if (!serving) StartCoroutine(EmployeeCounter(coord, counter));
            else StartCoroutine(EmployeeServing(coord, counter));
        }
        else
        {
            if (serving)
            {
                counter.employee = null;
                restartCoroutine = null;
            }
            else
            {
                restartCoroutine = null;
            }
            StartCoroutine(EmployeeWait());
        }
    }

    public void Work(Vector3 position, Table table)
    {
        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;
            StartCoroutine(EmployeeTable(coord, table));
        }
        else
        {
            table.employeeContoller = null;
            restartCoroutine = null;
            Debug.Log("Find Bug");
            StartCoroutine(EmployeeWait());
        }
    }

    public void Work(Vector3 position, TrashCan trash)
    {
        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;
            StartCoroutine(EmployeeTrashCan(coord, trash));
        }
        else
        {
            restartCoroutine = null;

            StartCoroutine(EmployeeWait());
        }

    }
   
    float coroutineTimer2 = 0;
    IEnumerator EmployeeWait()
    {
        animator.SetInteger("state", 2);
        while(coroutineTimer <= 1)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }
        coroutineTimer = 0;
        busy = false;
        restartCoroutine = null;
    }

    IEnumerator EmployeeWait_Patrol()
    {
        Vector3 target;
        while (true)
        {
            float x = UnityEngine.Random.Range(-1f, 1f);
            float y = UnityEngine.Random.Range(-1f, 1f);
            Vector3 v3 = new Vector3(x, 0, y).normalized;
            float speed = UnityEngine.Random.Range(employeeData.move_speed, employeeData.move_speed * 2);
            target = trans.position + v3 * speed;

            bool interruptCheck = Physics.CheckBox(target, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7);
            bool validCheck = Physics.CheckBox(target, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1);
            if (validCheck && !interruptCheck)
            {
                break;
            }
        }

        Coord coord = CalculateCoords(target);
        if (coord != null) {
            if (coord.c == 100 && coord.r == 100) coord = null;
            yield return StartCoroutine(AnimalMove(coord));
        }
       
        busy = false;
        restartCoroutine = null;
    }
    IEnumerator EmployeeFoodMachine(Coord coord, FoodMachine foodMachine)
    {
        yield return StartCoroutine(AnimalMove(coord));

        modelTrans.rotation = foodMachine.workingSpot.rotation;

        while (foodMachine.foodStack.foodStack.Count > 0)
        {
            while (coroutineTimer2 < employeeData.action_speed)
            {
                coroutineTimer2 += Time.deltaTime;
                yield return null;
            }
            coroutineTimer2 = 0;
            //yield return new WaitForSeconds(0.1f);
            int weight = 0;
            for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
            if (weight >= employeeData.max_holds) break;
            Food f = foodMachine.foodStack.foodStack[foodMachine.foodStack.foodStack.Count - 1];
            foodMachine.foodStack.foodStack.Remove(f);
            float r = UnityEngine.Random.Range(1, 2.5f);
            bool check = false;
            if (foodStacks.Count > 0)
            {
                for (int j = 0; j < foodStacks.Count; j++)
                {
                    if (foodStacks[j].type == f.parentType)
                    {
                        check = true;
                    }
                }
            }
            if (check == false)
            {
                FoodStack foodStack = new FoodStack();
                foodStack.type = f.parentType;
                foodStacks.Add(foodStack);
            }

            int n = 0;
            for (n = 0; n < foodStacks.Count; n++)
            {
                if (foodStacks[n].type == f.parentType)
                {

                    break;
                }
            }
            FoodStack fs = foodStacks[n];

            f.transform.DOJump(headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
            {
                fs.foodStack.Add(f);
               
                f.transform.position = headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0);
                //    foodList.Add(f);
                audioSource.Play();
            });
        }

        while(coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
     //   yield return new WaitForSeconds(0.5f);
        foodMachine.employee = null;
        busy = false;
        restartCoroutine = null;
    }
    IEnumerator EmployeeCounter(Coord coord, Counter counter)
    {
        yield return StartCoroutine(AnimalMove(coord));

        modelTrans.rotation = counter.transform.rotation;

        for (int i = 0; i < foodStacks.Count; i++)
        {
            for (int j = 0; j < counter.foodStacks.Count; j++)
            {
                if (foodStacks[i].type == counter.foodStacks[j].type)
                {
                    while (foodStacks[i].foodStack.Count > 0)
                    {
                        Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                        foodStacks[i].foodStack.Remove(f);
                        float r = UnityEngine.Random.Range(1, 2.5f);
                        f.transform.DOJump(counter.stackPoints[j].position + new Vector3(0, 0.7f * counter.foodStacks[j].foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f);
                        counter.foodStacks[j].foodStack.Add(f);
                        audioSource.Play();
                        yield return new WaitForSeconds(employeeData.action_speed);
                    }
                }
            }
        }

        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
            if (foodStacks[i].foodStack.Count == 0)
            {
                foodStacks.RemoveAt(i);
            }
        }
        while (coroutineTimer <= 0.3f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
        //yield return new WaitForSeconds(0.3f);
        busy = false;
        restartCoroutine = null;
    }
    IEnumerator EmployeePackingTable(Coord coord, PackingTable packingTable)
    {
        yield return StartCoroutine(AnimalMove(coord));

        modelTrans.rotation = packingTable.transform.rotation;

        for (int i = 0; i < foodStacks.Count; i++)
        {
            for (int j = 0; j < packingTable.foodStacks.Count; j++)
            {
                if (foodStacks[i].type == packingTable.foodStacks[j].type)
                {
                    while (foodStacks[i].foodStack.Count > 0)
                    {

                        Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                        foodStacks[i].foodStack.Remove(f);
                        float r = UnityEngine.Random.Range(1, 2.5f);
                        f.transform.DOJump(packingTable.stackPoints[j].position + new Vector3(0, 0.7f * packingTable.foodStacks[j].foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f);
                        packingTable.foodStacks[j].foodStack.Add(f);
                        audioSource.Play();
                        yield return new WaitForSeconds(employeeData.action_speed);

                        //Debug.Log(packingTable.foodStacks[j].foodStack.Count);
                    }
                }
            }
        }

        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
            if (foodStacks[i].foodStack.Count == 0)
            {
                foodStacks.RemoveAt(i);
            }
        }
        while (coroutineTimer <= 0.3f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
     //   yield return new WaitForSeconds(0.3f);
        busy = false;
        restartCoroutine = null;
    }

    IEnumerator EmployeeNowPacking(Coord coord, PackingTable packingTable)
    {
        yield return StartCoroutine(AnimalMove(coord));

        modelTrans.rotation = packingTable.packingTrans.rotation;

        HamburgerPackaging hamburgerPackaging = packingTable.GetComponent<HamburgerPackaging>();

        if (hamburgerPackaging.packingAction != null)
        {
            hamburgerPackaging.packingAction.Invoke();
            yield return new WaitForSeconds(1f);
        }
        int num = 0;
        foreach (FoodStack p in packingTable.foodStacks)
        {
            num += p.foodStack.Count;
        }

        List<FoodStack> list = new List<FoodStack>();
        while (num > 0)
        {
            for (int i = 0; i < packingTable.foodStacks.Count; i++)
            {
                list.Add(packingTable.foodStacks[i]);
            }
            list.Sort(delegate (FoodStack a, FoodStack b) { return a.foodStack.Count.CompareTo(b.foodStack.Count); });
            list.Reverse();
            Food f = list[0].foodStack[list[0].foodStack.Count - 1];

            list[0].foodStack.Remove(f);
            hamburgerPackaging.packingAction += () => { hamburgerPackaging.StartMove(f, false); };
            yield return hamburgerPackaging.MoveFood(f,true);
            yield return new WaitForSeconds(0.2f);
            num = 0;
            foreach (FoodStack p in packingTable.foodStacks)
            {
                num += p.foodStack.Count;
            }
        }


        /* while(packingTable.f1.foodStack.Count > 0)
         {
             hamburgerPackaging.MoveHamburger();
             yield return new WaitForSeconds(0.5f);
         }*/
        while (coroutineTimer <= 0.3f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
    //    yield return new WaitForSeconds(0.3f);
        packingTable.employee = null;
        restartCoroutine = null;
        busy = false;
    }
    IEnumerator EmployeeGetPackagedFoods(Coord coord, PackingTable packingTable)
    {
        if(step == 0) yield return StartCoroutine(AnimalMove(coord));
        if(step == 0) step = 1;

        if(step == 1)
        {
            modelTrans.rotation = packingTable.packingTrans.rotation;

            while (packingTable.packageStack.foodStack.Count > 0)
            {
                while (coroutineTimer <= employeeData.action_speed)
                {
                    coroutineTimer += Time.deltaTime;
                    yield return null;
                }
                coroutineTimer = 0;

                int weight = 0;
                for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
                if (weight >= employeeData.max_holds) break;

                Food f = packingTable.packageStack.foodStack[packingTable.packageStack.foodStack.Count - 1];
                packingTable.packageStack.foodStack.Remove(f);
                float r = UnityEngine.Random.Range(1, 2.5f);
                bool check = false;
                if (foodStacks.Count > 0)
                {
                    for (int j = 0; j < foodStacks.Count; j++)
                    {
                        if (foodStacks[j].type == f.parentType)
                        {
                            check = true;
                        }
                    }
                }
                if (check == false)
                {
                    FoodStack foodStack = new FoodStack();
                    foodStack.type = f.parentType;
                    foodStacks.Add(foodStack);
                }

                int n = 0;
                for (n = 0; n < foodStacks.Count; n++)
                {
                    if (foodStacks[n].type == f.parentType)
                    {

                        break;
                    }
                }

                FoodStack fs = foodStacks[n];
                f.transform.DOJump(headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                {
                    f.transform.position = headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0);
                    //    foodList.Add(f);
                    audioSource.Play();

                    fs.foodStack.Add(f);
                });

                // yield return new WaitForSeconds(0.1f);

            }
            step = 2;
        }

        if (step == 2)
        {
            GameInstance gameIns = new GameInstance();
            WorkSpaceManager workSpaceManager = gameIns.GameIns.workSpaceManager;

            for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
            {
                if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery)
                {

                    yield return StartCoroutine(AnimalMove(CalculateCoords(workSpaceManager.packingTables[i].workingSpot_SmallTables[0].transform.position)));
                    


                    for (int j = 0; j < foodStacks.Count; j++)
                    {
                        if (foodStacks[j].type == FoodMachine.MachineType.PackingTable)
                        {
                            while (foodStacks[j].foodStack.Count > 0)
                            {
                                while (coroutineTimer <= employeeData.action_speed)
                                {
                                    coroutineTimer += Time.deltaTime;
                                    yield return null;
                                }
                                coroutineTimer = 0;
                                float r = UnityEngine.Random.Range(1, 2.5f);
                                Food f = foodStacks[j].foodStack[foodStacks[j].foodStack.Count - 1];
                                foodStacks[j].foodStack.Remove(f);
                                workSpaceManager.packingTables[i].packageStack.foodStack.Add(f);
                                f.transform.DOJump(workSpaceManager.packingTables[i].smallTable.position + new Vector3(0, 0.7f * workSpaceManager.packingTables[i].packageStack.foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                                {
                                    f.transform.position = workSpaceManager.packingTables[i].smallTable.position + new Vector3(0, 0.7f * workSpaceManager.packingTables[i].packageStack.foodStack.Count, 0);
                                    //    foodList.Add(f);
                                    audioSource.Play();

                                   
                                });
                            }
                        }
                    }

                    for (int k = foodStacks.Count - 1; k >= 0; k--)
                    {
                        if (foodStacks[k].foodStack.Count == 0) foodStacks.RemoveAt(k);
                    }

                    //  yield return StartCoroutine(AnimalMovement(CalculateCoords()));
                    // yield return new WaitForSeconds(0.3f);
                    while (coroutineTimer <= 0.3f)
                    {
                        coroutineTimer += Time.deltaTime;
                        yield return null;
                    }

                    step = 0;
                    coroutineTimer = 0;
                    packingTable.employeeAssistant = null;
                    busy = false;
                    restartCoroutine = null;
                    break;
                }
            }
        }
    }
    float timerY = 0;
    float timerXZ = 0;
    float coroutineTimer3 = 0;
    float coroutineTimer4 = 0;
    float X = 0;
    float Z = 0;
    int foodA = -1;
    int foodNum = 0;
    IEnumerator EmployeeDeliveryFoods(Coord coord, PackingTable packingTable)
    {
        if(step == 0) yield return StartCoroutine(AnimalMove(coord));
        if (step == 0) step = 1;
        if (step == 1)
        {
            modelTrans.rotation = packingTable.workingSpot.rotation;
           
            while (packingTable.customer.foodStacks[0].needFoodNum > foodNum)
            {
                while (coroutineTimer <= employeeData.action_speed)
                {
                    coroutineTimer += Time.deltaTime;
                    yield return null;
                }
                coroutineTimer = 0;

                int weight = 0;
                for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
                if (weight >= employeeData.max_holds) break;

                Food food = packingTable.packageStack.foodStack[packingTable.packageStack.foodStack.Count - 1];
                packingTable.packageStack.foodStack.Remove(food);


                for (int i = 0; i < foodStacks.Count; i++)
                {
                    if (foodStacks[i].type == FoodMachine.MachineType.PackingTable)
                    {
                        foodA = i;
                    }
                }
                if (foodA == -1)
                {
                    FoodStack fs = new FoodStack();
                    fs.type = FoodMachine.MachineType.PackingTable;
                    foodStacks.Add(fs);
                    for (int i = 0; i < foodStacks.Count; i++)
                    {
                        if (foodStacks[i].type == FoodMachine.MachineType.PackingTable)
                        {
                            foodA = i;
                        }
                    }
                }

                float r = UnityEngine.Random.Range(1, 2.5f);
               
                foodNum++;
                food.transform.DOJump(headPoint.position + new Vector3(0, 0.7f * foodStacks[foodA].foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                {
                    foodStacks[0].foodStack.Add(food);
                    food.transform.position = headPoint.position + new Vector3(0, 0.7f * foodStacks[foodA].foodStack.Count, 0);
                    //    foodList.Add(f);
                    audioSource.Play();


                   

                });
            }
            step = 2;
            foodNum = 0;
        }
        //배달 준비 완료
  
        animator.SetInteger("state", 3);
        if (step == 2)
        {
            while (timerY <= 1.5f)
            {

                trans.Translate(Vector3.up * 6f * Time.deltaTime);

                timerY += Time.deltaTime;
                yield return null;
            }
            timerY = 0;
            step = 3;
        }

        if (step == 3)
        {
            while (coroutineTimer <= 0.4f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            step = 4;
        }
 
        if (step == 4)
        {
            while (timerXZ <= 3f)
            {
                trans.Translate(modelTrans.forward * 10f * Time.deltaTime);
                
                timerXZ += Time.deltaTime;
                yield return null;
            }
            timerXZ = 0;
            coroutineTimer = 0;
            step = 5;
        }

        if (step == 5)
        {
            while (foodStacks[foodA].foodStack.Count > 0)
            {

                Food f = foodStacks[foodA].foodStack[foodStacks[foodA].foodStack.Count - 1];
                foodStacks[foodA].foodStack.Remove(f);
                packingTable.customer.foodStacks[0].foodStack.Add(f);
            }

            packingTable.customer = null;
            packingTable.employee = null;

            while (coroutineTimer <= 5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            step = 6;
        }
        /*     for(int i= foodStacks.Count - 1; i <= 0; i--)
             {
                 if (foodStacks[i].foodStack.Count == 0)
                 {
                     foodStacks.RemoveAt(i);
                 }
             }*/

        bool check = false;
        if (step == 6)
        {
            while (!check)
            {
                X = UnityEngine.Random.Range(-18, 22);
                Z = UnityEngine.Random.Range(-22, 21);
                if (Physics.CheckBox(new Vector3(X, 0, Z), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                {
                    check = false;
                }
                else
                {
                    break;
                }

                yield return null;
            }
            step = 7;
        }

        Vector3 targetLoc = new Vector3(X, 0, Z);
        if (step == 7)
        {
            while (true)
            {
                Vector3 currnetLoc = new Vector3(trans.position.x, 0, trans.position.z);
                Vector3 dir = (targetLoc - currnetLoc).normalized;
                trans.Translate(dir * 7f * Time.deltaTime, Space.World);
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);

                if ((currnetLoc - targetLoc).magnitude < 0.5f) break;
                yield return null;
            }
            step = 8;
        }
        if (step == 8)
        {
            while (coroutineTimer <= 0.4f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            step = 9;
        }   
        //  yield return new WaitForSeconds(0.4f);
        if (step == 9)
        {
            while (true)
            {
                trans.position = new Vector3(trans.position.x, trans.position.y - 6f * Time.deltaTime, trans.position.z);

                if (trans.position.y < 0)
                {
                    trans.position = new Vector3(trans.position.x, 0, trans.position.z);

                    break;
                }

                yield return null;
            }
            step = 10;
        }
        animator.SetInteger("state", 0);
        if(step == 10)
        {
            while (coroutineTimer <= 0.5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            step = 0;
            restartCoroutine = null;
            busy = false;
            foodA = -1;
        }
    }

    IEnumerator EmployeeServing(Coord coord, Counter counter)
    {
        yield return StartCoroutine(AnimalMove(coord));

        modelTrans.rotation = counter.workingSpot.rotation;
        if (counter.customer)
        {
            for (int i = 0; i < counter.customer.foodStacks.Count; i++)
            {
                for (int j = 0; j < counter.foodStacks.Count; j++)
                {
                    if (counter.customer.foodStacks[i].type == counter.foodStacks[j].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                    {
                        while (counter.foodStacks[j].foodStack.Count > 0)
                        {
                            if (counter.customer.foodStacks[i].foodStack.Count >= counter.customer.foodStacks[i].needFoodNum) break;
                            Food f = counter.foodStacks[j].foodStack[counter.foodStacks[j].foodStack.Count - 1];
                            counter.foodStacks[j].foodStack.Remove(f);
                            float r = UnityEngine.Random.Range(1, 2.5f);
                            f.transform.DOJump(counter.customer.headPoint.position + new Vector3(0, 0.7f * counter.customer.VisualizingFoodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                            {
                                counter.customer.foodStacks[i].foodStack.Add(f);
                                counter.customer.VisualizingFoodStack.Add(f);
                                audioSource.Play();
                                instance.GameIns.uiManager.UpdateOrder(counter.customer, counter.counterType);
                            });

                            while (coroutineTimer2 <= employeeData.action_speed * 2)
                            {
                                coroutineTimer2 += Time.deltaTime;
                                yield return null;
                            }

                            coroutineTimer2 = 0;
                        }
                    }
                }
            }
        }
        while (coroutineTimer <= 1f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
        counter.employee = null;
        restartCoroutine = null;
        busy = false;
    }

    IEnumerator EmployeeTable(Coord coord, Table table)
    {

        yield return StartCoroutine(AnimalMove(coord, table));
        if (!table.interacting)
        {
            // Garbage go = table.garbageInstance;
            while (table.numberOfGarbage > 0)
            {
                float p = UnityEngine.Random.Range(1, 2.5f);
                Garbage go = table.garbageList[table.garbageList.Count - 1];//garbages.Pop();
                table.garbageList.Remove(go);
                table.numberOfGarbage--;
                go.transform.DOJump(headPoint.position + new Vector3(0, 0.5f * garbageList.Count, 0), p, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                {
                    audioSource.Play();
                    go.transform.position = headPoint.position + new Vector3(0, 0.5f * garbageList.Count, 0);
                    garbageList.Add(go);
                });
            }
            
            while(coroutineTimer <= 0.5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            table.isDirty = false;
            employeeState = EmployeeState.TrashCan;
            table.employeeContoller = null;
            busy = false;
            restartCoroutine = null;
        }
        else
        {
            Debug.Log("Interaction Error");
        }
    }

    IEnumerator EmployeeTrashCan(Coord coord, TrashCan trashCan)
    {
        yield return StartCoroutine(AnimalMove(coord));

        while (garbageList.Count > 0)
        {
            while (coroutineTimer2 <= employeeData.action_speed)
            {
                coroutineTimer2 += Time.deltaTime;
                yield return null;
            }
            coroutineTimer2 = 0;
            Garbage garbage = garbageList[garbageList.Count - 1];
            garbageList.Remove(garbage);
            float r = UnityEngine.Random.Range(1, 2.5f);
            garbage.transform.DOJump(trashCan.transform.position, r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
            {
                audioSource.Play();
                GarbageManager.ClearGarbage(garbage);
            });
        }
        while (coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
        employeeState = EmployeeState.Wait;
        restartCoroutine = null;
        busy = false;

    }

    void Reward(Vector3 position)
    {
        Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;

            StartCoroutine(EmployeeGetRewards(coord));
        }
        else
        {
            restartCoroutine = null;
            StartCoroutine(EmployeeWait());
        }

    }

    
    IEnumerator EmployeeGetRewards(Coord coord)
    {
        rewardingType = RewardingType.Walk;
        //이동
        yield return StartCoroutine(AnimalMove(coord, null, true));

        rewardingType = RewardingType.Eat;

        if(reward)
        {
            //행동
            while (reward.foods.Count > 0)
            {
                animator.SetInteger("state", 4);
                while (coroutineTimer <= 0.417f)
                {
                    coroutineTimer += Time.deltaTime;
                    yield return null;
                }

                Food go = reward.foods[reward.foods.Count - 1];

                reward.EatFish(go);
                Animal animal = GetComponentInParent<Animal>();
                EXP += 10;
                ui.GainExperience(); //경험치바 채워주기
                                     //yield return new WaitForSeconds(0.5f);
                if (EXP >= 100)
                {
                    EXP = 0;
                    //  instance.GameIns.restaurantManager.combineDatas.employeeData[id - 1].level++;
                    instance.GameIns.restaurantManager.UpgradePenguin(employeeData.level, false, this); //펭귄의 레벨 업 및 능력치 변경하기
                                                                                                        //Invoke("LevelUp", 0.5f);
                                                                                                        //   SaveLoadManager.EmployeeLevelSave(true);
                    while (coroutineTimer2 <= 0.5f)
                    {
                        coroutineTimer2 += Time.deltaTime;
                        yield return null;
                    }

                }
                else SaveLoadManager.EmployeeLevelSave(true);
                // yield return new WaitForSeconds(0.5f);
                animator.SetInteger("state", 0);

                while (coroutineTimer3 <= 0.2f)
                {
                    coroutineTimer3 += Time.deltaTime;
                    yield return null;
                }

                coroutineTimer3 = 0;
                coroutineTimer2 = 0;
                coroutineTimer = 0;
            }
        }

        reward.ClearFishes();   //상자 치우기
                                // instance.GameIns.inputManager.inputDisAble = false;

        reward = null;
        busy = false;
        restartCoroutine = null;
        rewardingType = RewardingType.None;
    }

    void LevelUp()
    {
        EXP = 0;
        Animal animal = GetComponentInParent<Animal>();
        Employee animalController = animal.GetComponentInChildren<Employee>();

        instance.GameIns.restaurantManager.UpgradePenguin(instance.GameIns.restaurantManager.combineDatas.employeeData[animalController.id - 1].level, false, animalController);
        SliderController sliderController = animal.GetComponentInChildren<SliderController>();
        // sliderController.
    }

    IEnumerator AnimalMove(Coord coord, Table table = null, bool rewards = false, bool bContinue = false)
    {
        if (coord != null)
        {
            List<Coord> coords = GetCalculatedList(coord);
            int i = 0;
            Vector3 currentNode = trans.position;
            while (i < coords.Count - 1)
            {
              
                animator.SetInteger("state", 1);
                bool bStopping = false;

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

                    if (table != null)
                    {
                        if (!table.isDirty || table.interacting)
                        {
                            bStopping = true;
                            break;
                        }
                    }

                    if (rewards)
                    {
                        if (reward)
                        {
                            float rr = instance.GameIns.calculatorScale.minY + coord.r * instance.GameIns.calculatorScale.distanceSize;
                            float cc = instance.GameIns.calculatorScale.minX + coord.c * instance.GameIns.calculatorScale.distanceSize;

                            float mag_r = Mathf.Abs(rr - modelTrans.position.z);
                            float mag_c = Mathf.Abs(cc - modelTrans.position.x);

                            if (mag_r <= 2f && mag_c <= 2f && mag_r + mag_c < 3)
                            {
                                bStopping = true;
                                break;
                            }
                            else if (magnitude <= 0.1f || newMagnitude < magnitude)
                            {
                                break;
                            }
                        }
                        else
                        {
                            bStopping = true;
                            break;
                        }
                    }

                    trans.Translate(dirs * Time.deltaTime * employeeData.move_speed, Space.World);
                    float angle = Mathf.Atan2(dirs.x, dirs.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    yield return null;
                }
                i++;


                if (bStopping)
                {
                    animator.SetInteger("state", 0);

                    while (coroutineTimer <= 0.2f)
                    {
                        coroutineTimer += Time.deltaTime;
                        yield return null;
                    }
                    coroutineTimer = 0;

                    employeeState = EmployeeState.Wait;
                    if (table) table.employeeContoller = null;
                    if (table) busy = false;
                    yield break;
                }
                if (!bContinue) animator.SetInteger("state", 0);
            }
        }
    }
}
