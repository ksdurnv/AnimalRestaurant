using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using static SaveLoadManager;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;
using CryingSnow.FastFoodRush;

public class RestaurantManager : MonoBehaviour
{
    public int[] employeeHire = { 0, 500, 1000, 1500, 2000, 2500, 3000, 3500 };
    
    public GameObject[] levels;
    public NextTarget[] levelGuides;
    NextTargetData[] nextTargetDatas = new NextTargetData[100];
    GameInstance gameInstance = new GameInstance();
    public AudioSource purchase;
    public int level = 0;
    public bool allLevelUp;
    public int fishNum = 100;
    public bool employable;
    public int restaurantValue = 0;

    public PlayerData playerData = new PlayerData(0, 0f, 0,0, null);
   
    public CombineDatas combineDatas;

    public List<EmployeeData> employeeDatas = new List<EmployeeData>();
    public EmployeeData currentEmployeeData;
    public Dictionary<FoodMachine.MachineType, List<MachineData>> upgradeMachineDic = new Dictionary<FoodMachine.MachineType, List<MachineData>>();

    public List<Vector3> flyingEndPoints = new List<Vector3>();

    // Start is called before the first frame update
    private void Awake()
    {
       
        var burgerupgrade_resources = Resources.Load<TextAsset>("burger_upgrade_table");

        // List<MachineData> burgerdata = JsonConvert.DeserializeObject<List<MachineData>>(burgerupgrade_resources.text);
        List<MachineData> burgerdata = JsonConvert.DeserializeObject<List<MachineData>>(burgerupgrade_resources.text);
        upgradeMachineDic.Add(FoodMachine.MachineType.BurgerMachine, burgerdata);

        var cokeupgrade_resources = Resources.Load<TextAsset>("coke_upgrade_table");
        List<MachineData> cokedata = JsonConvert.DeserializeObject<List<MachineData>>(cokeupgrade_resources.text);
        upgradeMachineDic.Add(FoodMachine.MachineType.CokeMachine, cokedata);

        var coffeeupgrade_resources = Resources.Load<TextAsset>("coffee_upgrade_table");
        List<MachineData> coffeedata = JsonConvert.DeserializeObject<List<MachineData>>(coffeeupgrade_resources.text);
        upgradeMachineDic.Add(FoodMachine.MachineType.CoffeeMachine, coffeedata);

        var donutupgrade_resources = Resources.Load<TextAsset>("donut_upgrade_table");
        List<MachineData> donutdata = JsonConvert.DeserializeObject<List<MachineData>>(donutupgrade_resources.text);
        upgradeMachineDic.Add(FoodMachine.MachineType.DonutMachine, donutdata);

        var employeeupgrade_resources = Resources.Load<TextAsset>("employee_upgrade_table");
        employeeDatas = JsonConvert.DeserializeObject<List<EmployeeData>>(employeeupgrade_resources.text);

        GameInstance instance = new GameInstance();
        instance.GameIns.restaurantManager = this;
        combineDatas = SaveLoadManager.LoadGame();
     //   combineDatas.playerData.money = 100000;
        playerData = combineDatas.playerData;
     
       
       // UpgradePenguin(combineDatas.playerData.level, true, null);

    
    }
    void Start()
    {
       if(gameInstance.GameIns.uiManager !=null) gameInstance.GameIns.uiManager.UpdateMoneyText(playerData.money);
        /*  for (int i = 0; i < 100; i++)
          {
              nextTargetDatas[i] = new NextTargetData();
              nextTargetDatas[i].Price = 500;
          }

          for (int i = 0; i < levelGuides.Length; i++)
          {
              levelGuides[i].money = nextTargetDatas[i].Price;
          }*/
        if (allLevelUp)
        {
            for (int i = 0; i < levelGuides.Length; i++)
            {
                //  LevelUp();
            }
        }

        LoadRestaurant();

        playerData.money = 100000;
        playerData.fishesNum = 1000;
    }

    public void LevelUp(bool load = false)
    {
        //   if (saveLoadManager.isLoading == false)
        {
            if (!load)
            {
                if (playerData.money >= levelGuides[level].money)
                {
                    playerData.money -= levelGuides[level].money;
                }
                else return;
                gameInstance.GameIns.uiManager.UpdateMoneyText(playerData.money);
                purchase.Play();
            }
        }

        combineDatas.playData[level].unlock = true;
        WorkSpaceManager workSpaceManager = gameInstance.GameIns.workSpaceManager;
        if (levelGuides[level].type == FoodMachine.MachineType.BurgerMachine && workSpaceManager.unlocks[0] == 0) workSpaceManager.unlocks[0]++;
        if (levelGuides[level].type == FoodMachine.MachineType.CokeMachine && workSpaceManager.unlocks[0] == 1) workSpaceManager.unlocks[0]++;
        if (levelGuides[level].type == FoodMachine.MachineType.CoffeeMachine && workSpaceManager.unlocks[1] == 0) workSpaceManager.unlocks[1]++;
        if (levelGuides[level].type == FoodMachine.MachineType.DonutMachine && workSpaceManager.unlocks[1] == 1) workSpaceManager.unlocks[1]++;

        switch (levelGuides[level].workSpaceType)
        {
            case NextTarget.WorkSpaceType.Counter:
                {
                    workSpaceManager.counters.Add(levels[level].gameObject.GetComponent<Counter>());
                    break;
                }
            case NextTarget.WorkSpaceType.Table:
                {
                    workSpaceManager.tables.Add(levels[level].gameObject.GetComponent<Table>());
                    break;
                }
            case NextTarget.WorkSpaceType.FoodMachine:
                {
                    if (levels[level].gameObject.TryGetComponent<FoodMachine>(out FoodMachine foodMachine))
                    {
                        workSpaceManager.foodMachines.Add(foodMachine);
                        foodMachine.playData = combineDatas.playData[level];
                       

                        for (int i = 0; i < combineDatas.levelData.Count; i++)
                        {
                            if (combineDatas.levelData[i].id == combineDatas.playData[level].id)
                            {
                                foodMachine.level = combineDatas.levelData[i].level;

                                List<MachineData> md = upgradeMachineDic[foodMachine.machineType];
                                foodMachine.machineData = md[foodMachine.level - 1];
                                //     UpgradeMachine(foodMachine);
                                break;
                            }
                        }
                    }
                    break;
                }
        }

        levelGuides[level].gameObject.SetActive(false);
        

        GameObject expandObject = levels[level++];
        expandObject.SetActive(true);

        if (expandObject.GetComponent<PackingTable>())
            workSpaceManager.packingTables.Add(expandObject.GetComponent<PackingTable>());

        if (expandObject.TryGetComponent<ScaleUp>(out ScaleUp scaleUp)) scaleUp.ObjectEnabled(load);
        if (expandObject.TryGetComponent<TableScaleUp>(out TableScaleUp tableScaleUp))
        {
            tableScaleUp.ObjectEnabled(load);
        }
        if (levelGuides.Length > level) levelGuides[level].gameObject.SetActive(true);
        MoveCalculator.CheckArea(gameInstance.GameIns.calculatorScale);

        if (!load)
        {
            if ((playerData.employeeNum < 8 && employeeHire[playerData.employeeNum] <= GetRestaurantValue()))
            {
                gameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }

            SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
            SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_UPGRADE);

            for (int i = 0; i < gameInstance.GameIns.animalManager.employeeControllers.Count; i++)
            {
                if (gameInstance.GameIns.animalManager.employeeControllers[i].restartCoroutine != null)
                {
                    gameInstance.GameIns.animalManager.employeeControllers[i].RestartAction();
                }
            }
            for (int i = 0; i < gameInstance.GameIns.animalManager.customerControllers.Count; i++)
            {
                if (gameInstance.GameIns.animalManager.customerControllers[i].restartCoroutine != null)
                {
                    gameInstance.GameIns.animalManager.customerControllers[i].RestartAction();
                }
            }
        }
    }

    void LoadRestaurant()
    {
        for (int i = 0; i < combineDatas.playData.Count; i++)
        {
            if (combineDatas.playData[i].unlock)
            {
                LevelUp(true);
            }
        }

        Invoke("LoadEmployees", 0.1f);
    }

    void LoadEmployees()
    {
        // yield return new WaitForSeconds(0.5f);
        //직원 로드
        for (int i = 0; i < combineDatas.playerData.employeeNum; i++)
        {
            Employee animal = gameInstance.GameIns.animalManager.SpawnEmployee();
          //  animal.employeeData = employeeDatas[i];
            bool check = false;
            float X = 0;
            float Z = 0;
            while (!check)
            {
                X = Random.Range(-18, 22);
                Z = Random.Range(-22, 21);
                if (Physics.CheckBox(new Vector3(X, 0, Z), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                {
                    check = false;
                }
                else
                {
                    check = true;
                }
            }

            animal.trans.position = new Vector3(X, 0, Z);
            animal.EXP = combineDatas.employeeData[animal.id - 1].exp;
            animal.EmployeeData = employeeDatas[combineDatas.employeeData[animal.id - 1].level - 1];
            animal.ui.UpdateLevel(animal.EmployeeData.level);
        }
        if ((playerData.employeeNum < 8 && employeeHire[playerData.employeeNum] <= GetRestaurantValue()))
        {
            gameInstance.GameIns.applianceUIManager.UnlockHire(true);
        }
        else  gameInstance.GameIns.applianceUIManager.UnlockHire(false);
    }


    public void UpgradeMachine(FoodMachine foodMachine)
    {
        if (foodMachine.machineData.upgrade_cost <= playerData.money)
        {
            playerData.money -= foodMachine.machineData.upgrade_cost;
            List<MachineData> machineUpgrades = upgradeMachineDic[foodMachine.machineType];
            foodMachine.machineData = machineUpgrades[foodMachine.level - 1];

            if ((playerData.employeeNum < 8 && employeeHire[playerData.employeeNum] <= GetRestaurantValue()))
            {
                gameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }
            else
            {
                gameInstance.GameIns.applianceUIManager.UnlockHire(false);
            }

         //   SaveLoadManager.Save(SaveState.)
        }
    }

    public void HireEmployee()
    {
        if (playerData.employeeNum < 8 && employeeHire[playerData.employeeNum] <= GetRestaurantValue())
        { 
            playerData.employeeNum++;
       
            SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
            EmployeeNum();
        }
    }

    public void EmployeeNum()
    {
        Employee animal = gameInstance.GameIns.animalManager.SpawnEmployee();
    
        animal.EmployeeData = employeeDatas[combineDatas.employeeData[animal.id - 1].level - 1];
        //   Vector3 targetPos = gameInstance.GameIns.inputManager.cameraRange.position;
        animal.busy = true;
        animal.trans.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // animal.transform.DOMove(new Vector3(targetPos.x,0,targetPos.z), 2f);

        if ((playerData.employeeNum < 8 && employeeHire[playerData.employeeNum] <= GetRestaurantValue()))
        {
            gameInstance.GameIns.applianceUIManager.UnlockHire(true);
        }
        else
        {
            gameInstance.GameIns.applianceUIManager.UnlockHire(false);
        }
        //StartCoroutine(movePenguin(animal));

        animal.spawning = true;
        animal.StartFalling(true);
    }


    public void UseFish()
    {
        playerData.fishesNum--;
        SaveLoadManager.PlayerStateSave();
    }


    public Transform start;
    public Transform end;
    [Range(1f, 100f)]
    public float duration;
    [Range(1f, 40f)]
    public float height;
    [Range(1f, 4f)]
    public float weight;


    public void testCode()
    {
        float x = 0;
        float z = 0;
        float length = 0;
        for (int i = 0; i < 100; i++)
        {
            x = Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            z = Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            length = Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            Vector3 test = new Vector3(gameInstance.GameIns.inputManager.cameraRange.position.x, 0, gameInstance.GameIns.inputManager.cameraRange.position.z);
            Vector3 n = new Vector3(x, 0, z).normalized;
            Vector3 ta = test + n * length;

            if (Physics.Raycast(ta + Vector3.up * 5, Vector3.down, 10))
            {
                Debug.DrawLine(ta + Vector3.up * 10, ta + Vector3.down * 100f, Color.red, 100);


            }

        }
    }

    IEnumerator movePenguin(AnimalController animal)
    {
      //  testCode();
        animal.animator.SetInteger("state", 3);
        Vector3 startPoint = animal.trans.position; //start.position;

        float x = 0;
        float z = 0;
        float length = 0;
        while (true)
        {
            x = Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            z = Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            length = Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            //  length = Random.Range(-10,10);
            Vector3 test = new Vector3(gameInstance.GameIns.inputManager.cameraRange.position.x, 0, gameInstance.GameIns.inputManager.cameraRange.position.z);
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
            yield return null;
        }


        Vector3 endPoint = new Vector3(gameInstance.GameIns.inputManager.cameraRange.position.x, 0, gameInstance.GameIns.inputManager.cameraRange.position.z);
        Vector3 dir2 = new Vector3(x, 0, z).normalized;

        endPoint += dir2 * length;
        bool bstart = true;
        Vector3 controlVector = (startPoint + endPoint) / weight + Vector3.up * height;

        float elapsedTime = 0f;

        while (bstart)
        {

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            Vector3 origin = animal.trans.position;
            // Vector3 targetLoc = Vector3.Lerp(startPoint, endPoint, t);// CalculateBezierPoint(t, startPoint, controlVector, endPoint);
            Vector3 targetLoc = CalculateBezierPoint(t, startPoint, controlVector, endPoint);

            Vector3 dir = targetLoc - origin;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            animal.modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Debug.DrawLine(animal.trans.position, targetLoc, Color.red, 5f);
            animal.trans.position = targetLoc;


            if (t >= 1.0f)
            {
                animal.trans.position = endPoint;
                bstart = false;
            }

            yield return null;
        }

        animal.animator.SetInteger("state", 0);
        animal.busy = false;

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

    public void UpgradePenguin(int level, bool isLoad, Employee animalController)
    {
        if (employeeDatas.Count > level)
        {
            EmployeeData nextEmployeeData = employeeDatas[level];
       //     currentEmployeeData = employeeDatas[level];
            playerData.level = level;
        
            if (!isLoad)
            {
                gameInstance.GameIns.animalManager.UpdateEmployeeUpgrade(animalController);

                playerData.money -= employeeDatas[level - 1].upgrade_cost;
                animalController.EmployeeData = nextEmployeeData;
                //gameInstance.GameIns.applianceUIManager.ShowPenguinUpgradeInfo(animalController, true);
               // combineDatas.employeeData[]
                SaveLoadManager.EmployeeLevelSave(true);
           
            }
            else
            {
                gameInstance.GameIns.animalManager.UpdateEmployeeUpgrade(animalController);
            }
        }
    }

    public void UpgradeFoodMachine(FoodMachine foodMachine)
    {
        if (playerData.money >= foodMachine.machineData.upgrade_cost)
        {
            playerData.money -= foodMachine.machineData.upgrade_cost;
            gameInstance.GameIns.uiManager.UpdateMoneyText(playerData.money); 
            FoodMachine.MachineType type = foodMachine.machineType;
            MachineData d = foodMachine.machineData;

            List<MachineData> machineDatas = upgradeMachineDic[type];

            int currentLevel = d.level;


            MachineData nextLevel = machineDatas[d.level];

            foodMachine.machineData = nextLevel;

            gameInstance.GameIns.applianceUIManager.ShowApplianceInfo(foodMachine);

            SaveLoadManager.Save(SaveState.ONLY_SAVE_UPGRADE);
            SaveLoadManager.Save(SaveState.ONLY_SAVE_PLAYERDATA);
        }
    }

    public float GetRestaurantValue()
    {
        WorkSpaceManager workSpaceManager = gameInstance.GameIns.workSpaceManager;
        float val = 0;
        for(int i=0; i< combineDatas.playData.Count; i++)
        {
            if (combineDatas.playData[i].unlock)
            {
                int type = combineDatas.playData[i].type;
                switch(type)
                {
                    case 0:
                        val += 50;
                        break;
                    case 1:
                        val += 100;
                        break;
                    case 2:
                        val += 150;
                        break;
                    case 3:
                        val += 200;
                        break;
                }
            }
        }
        for(int i=0; i < workSpaceManager.foodMachines.Count; i++)
        {
            FoodMachine.MachineType machineType = workSpaceManager.foodMachines[i].machineType;
            List<MachineData> machindb = upgradeMachineDic[machineType];

            int proceeds = machindb[workSpaceManager.foodMachines[i].mData.level - 1].sale_proceeds;
            int maxheight = machindb[workSpaceManager.foodMachines[i].mData.level - 1].food_production_max_height;
            float speed = machindb[workSpaceManager.foodMachines[i].mData.level - 1].food_production_speed;

            val += proceeds * (maxheight - speed);
        }

        for(int i=0; i< gameInstance.GameIns.animalManager.employeeControllers.Count; i++)
        {
            Employee newAnimalController = gameInstance.GameIns.animalManager.employeeControllers[i];
            float action_speed = newAnimalController.EmployeeData.action_speed;
            float move_speed = newAnimalController.EmployeeData.move_speed;
            float max_holds = newAnimalController.EmployeeData.max_holds;
            val += (move_speed - action_speed) * (Mathf.Sqrt(max_holds) / 2) * 100;
        }

        val += 50000;
        return val;
    }
}
