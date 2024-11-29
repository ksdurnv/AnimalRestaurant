using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.EventSystems; 

public class ApplianceUIManager : MonoBehaviour
{
    public TMP_Text text;
    int level = 0;

    public GameObject appliancePanel;
    public Image applianceImage;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI upgradeInfoText;

    public Button hireBtn;

    public Button upgradeButton;
    FoodMachine foodMachine;
    Employee animalController;
    public Sprite employeeIcon;
    public Sprite[] machines;
    private GameInstance gameInstance = new GameInstance();

    //  public Button feedExitButton;
    public TMP_Text feedingDescription;
    RewardsBox rewardsBox;

    public GameObject fishGO;
    Queue<GameObject> gameObjects = new Queue<GameObject>();
    List<GameObject> rewards = new List<GameObject>();

    public RewardsBox currentBox;
    public List<RewardsBox> rewardsBoxes = new List<RewardsBox>();
    GraphicRaycaster gr;
    EventSystem es;
    public bool useDescription = false;
  //  bool activeUpgrade;
    private void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
        es = GetComponent<EventSystem>();
      //  activeUpgrade = true;
       // rewardsBox = GetComponentInChildren<RewardsBox>();

        gameInstance.GameIns.applianceUIManager = this;
        upgradeButton.onClick.AddListener(() => {
            Upgrade();
        });

    

        hireBtn.onClick.AddListener(() =>
        {
             gameInstance.GameIns.restaurantManager.HireEmployee();
        });
    }
    private void Start()
    {
       // gameInstance.GameIns.applianceUIManager = this;
        appliancePanel.SetActive(false); // 시작할 때는 패널 비활성화
    }

    Coroutine infoCoroutine;
    public void ShowApplianceInfo(FoodMachine foodMachine)
    {
      //  if (infoCoroutine == null)
        {
            animalController = null;
            this.foodMachine = foodMachine;
            appliancePanel.SetActive(true);
            UnlockHire(false);
            if (infoCoroutine != null) StopCoroutine(infoCoroutine);
            infoCoroutine = StartCoroutine(InputInfos(false));
            applianceImage.sprite = machines[(int)foodMachine.machineType - 1];
            upgradeCostText.text = $"Upgrade Cost: {foodMachine.machineData.upgrade_cost}원";
            upgradeInfoText.text = $"Level: {foodMachine.machineData.level}\nSale Proceeds: {foodMachine.machineData.sale_proceeds}원\nProduction Speed: {foodMachine.machineData.food_production_speed}/s\nProduction Max Height: {foodMachine.machineData.food_production_max_height}개";
        }
    }


    IEnumerator InputInfos(bool onlyAnimal = true)
    {
        gameInstance.GameIns.inputManager.inputDisAble = true;
        yield return null;
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData ped = new PointerEventData(es);
                ped.position = Input.mousePosition;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                gr.Raycast(ped, raycastResults);
                bool chck = false;
                for (int i = 0; i < raycastResults.Count; i++)
                {
                    if (raycastResults[i].gameObject.GetComponentInParent<ApplianceUIManager>())
                    {
                        chck = true;
                    }
                }
                if(!chck)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                    {
                        if (hit2.collider.GetComponentInParent<Animal>())
                        {
                            Employee newAnimal = hit2.collider.GetComponentInParent<Animal>().GetComponentInChildren<Employee>();

                            infoCoroutine = null;
                            ShowPenguinUpgradeInfo(newAnimal, true);
                            yield break;
                        }
                    }
                    else if (Physics.Raycast(ray, out RaycastHit hit3, Mathf.Infinity, 1 << 13))
                    {
                        if(hit3.collider.GetComponent<FoodMachine>())
                        {
                            gameInstance.GameIns.inputManager.inputDisAble = false;
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                            infoCoroutine = null;
                            ShowApplianceInfo(hit3.collider.GetComponent<FoodMachine>());
                            yield break;
                        }
                        else
                        {
                            gameInstance.GameIns.inputManager.inputDisAble = false;
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                        }
                    }
                    else
                    {
                        gameInstance.GameIns.inputManager.inputDisAble = false;
                        gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                    }
                    break;
                }
            }

            yield return null;
        }
        UnlockHire(true);
        appliancePanel.SetActive(false);
        infoCoroutine = null;
       
    }
   
    public void ShowPenguinUpgradeInfo(Employee animal, bool justUpdate = false)
    {


      //  if (infoCoroutine == null)
        {
            bool bShow = false;
            if (!justUpdate) bShow = true;
            else if (appliancePanel.activeSelf) bShow = true;

            if (bShow)
            {
                animalController = animal;
                EmployeeData employeeData = animalController.EmployeeData;// gameInstance.GameIns.restaurantManager.currentEmployeeData;
                foodMachine = null;
                UnlockHire(false);
                appliancePanel.SetActive(true);
                if (infoCoroutine != null) StopCoroutine(infoCoroutine);
                infoCoroutine = StartCoroutine(InputInfos());

                applianceImage.sprite = employeeIcon;
                upgradeCostText.text = $"Upgrade Cost: {employeeData.upgrade_cost}원";
                upgradeInfoText.text = $"Level: {employeeData.level}\nspeed: {employeeData.move_speed}\naction speed: {employeeData.action_speed}/s\nProduction Max Height: {employeeData.max_holds}개";
            }
        }
    }

    void DisableInput(bool disable)
    {
      //  gameInstance.GameIns.inputManager
        gameInstance.GameIns.inputManager.inputDisAble = disable;
        feedingDescription.gameObject.SetActive(disable);
       // rewardsBox.ClearFishes();
       // rewardsBox.gameObject.SetActive(disable);
    }


    public void HideApplianceInfo()
    {
        appliancePanel.SetActive(false);
       // UnlockHire(true);
        if(infoCoroutine != null)
        {
            StopCoroutine(infoCoroutine);
            infoCoroutine = null;
        }
    }

    public void UpdateLevel(int level)
    {
        this.level = level;
        //  text.text = $"Level {level}";
        Invoke("Level", 1f);
    }

    void Level()
    {
        text.text = $"Level {level}";
    }


    void Upgrade()
    {
        if (foodMachine) gameInstance.GameIns.restaurantManager.UpgradeFoodMachine(foodMachine);
        else
        {
            if(gameInstance.GameIns.restaurantManager.playerData.fishesNum > 0)
            Feed();
        }    
    }

    Coroutine scheduleCoroutine;
    bool buildBoxCoroutine;
    void Feed()
    {
    //    if (scheduleCoroutine == null && buildBoxCoroutine == false)
        {
            if (animalController.rewardingType == RewardingType.None && rewardsBoxes.Count < 3)
            {
                gameInstance.GameIns.inputManager.inputDisAble = true;
                if(useDescription)
                {
                    
                    feedingDescription.gameObject.SetActive(true);
                    feedingDescription.text = "Click the floor for build a feeding box";
                }
              
                HideApplianceInfo();
               
                scheduleCoroutine = StartCoroutine(EmployeeScheduleWork());
            }
            else
            {
                Debug.Log("boxCount " + rewardsBoxes.Count + " " + animalController.rewardingType);
            }
        }
    }

    IEnumerator EmployeeScheduleWork()
    {
        while (gameInstance.GameIns.inputManager.inputDisAble)
        {
           
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                {   
                    if(Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Default")))
                    {
                        if (hit2.collider.GetComponentInParent<FoodMachine>())
                        {
                            gameInstance.GameIns.inputManager.inputDisAble = false;
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                            feedingDescription.gameObject.SetActive(false);
                            ShowApplianceInfo(hit2.collider.GetComponentInParent<FoodMachine>());

                            yield break;
                        }
                    }
                    if (Physics.Raycast(ray, out RaycastHit hit3, Mathf.Infinity, LayerMask.GetMask("Animal")))
                    {
                        if (hit3.collider.GetComponentInParent<Animal>().GetComponent<Employee>())
                        {

                            gameInstance.GameIns.inputManager.inputDisAble = false;
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                            feedingDescription.gameObject.SetActive(false);
                            ShowPenguinUpgradeInfo(hit3.collider.GetComponentInParent<Animal>().GetComponent<Employee>());

                            yield break;
                        }
                    }
                    if (!Physics.CheckBox(hit.point, new Vector3(0.5f, 0.5f,0.5f), Quaternion.identity, 1 << 6 | 1<< 7))
                    {
                        StartCoroutine(BuildPackageBox(hit.point));

                        yield break;
                    }
                }
            }
            yield return null;
        }
        scheduleCoroutine = null;
    }

    IEnumerator BuildPackageBox(Vector3 endPosition)
    {
        RewardsBox rewardsBox = FoodManager.GetRewardsBox();
        rewardsBoxes.Add(rewardsBox);
        animalController.reward = rewardsBox;
        currentBox = rewardsBox;

        buildBoxCoroutine = true;
        Vector3 startPosition = new Vector3(endPosition.x, 5, endPosition.z);
        currentBox.gameObject.SetActive(true);
        currentBox.transform.localScale = new Vector3(2f,2f,2f);

        currentBox.transform.position = startPosition;

        if (useDescription) feedingDescription.text = "Click on the food box and feed it";
        while (true)
        {
            currentBox.transform.Translate(Vector3.down * 30f * Time.deltaTime, Space.World);
            if(currentBox.transform.position.y < 0)
            {
                currentBox.transform.position = new Vector3(currentBox.transform.position.x, 0, currentBox.transform.position.z);
                break;
            }
            yield return null;
        }

        while (true)
        {
            currentBox.transform.localScale += new Vector3(1, 1, 1) * 10 * Time.deltaTime;

            if (currentBox.transform.localScale.x > 2.5f)
            {
                break;
            }

            yield return null;
        }

        while (true)
        {
            currentBox.transform.localScale -= new Vector3(1, 1, 1) * 5 * Time.deltaTime;

            if (currentBox.transform.localScale.x < 2f)
            {
                currentBox.transform.localScale = new Vector3(2, 2, 2);
                break;
            }
            yield return null;
        }
       
        bool falling = false;
        while (gameInstance.GameIns.inputManager.inputDisAble)
        {
            if (currentBox != null)
            {
                if ((Input.GetMouseButton(0)) && falling == false || Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,1 << 12))
                    {
                        if (Physics.CheckBox(hit.point, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, LayerMask.GetMask("FeedBox")))
                        {
                            if (gameInstance.GameIns.restaurantManager.fishNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
                            {
                                if (currentBox.GetFish(true)) gameInstance.GameIns.restaurantManager.UseFish();
                                falling = true;
                            }
                            else
                            {
                                if (currentBox.ClearFishes())
                                {
                                    DisableInput(false);
                                    UnlockHire(true);
                                    animalController.reward = null;
                                    gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                                   
                                }
                                else
                                {
                          
                                    UnlockHire(true);
                                    DisableInput(false);
                                    gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                                }
                                yield break;
                            }
                        }
                    }
                    else
                    {
                        if (currentBox.ClearFishes())
                        {
                            animalController.reward = null;
                            UnlockHire(true);
                            DisableInput(false);
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                        }
                        else
                        {
                        
                            UnlockHire(true);
                            DisableInput(false);
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                         
                        }
                        if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                        {
                            if (hit2.collider.GetComponentInParent<Animal>())
                            {
                                Employee newAnimal = hit2.collider.GetComponentInParent<Animal>().GetComponentInChildren<Employee>();
                                currentBox = null;
                                ShowPenguinUpgradeInfo(newAnimal);

                            }
                        }

                        yield break;
                    }
                }
                else if (Input.GetMouseButton(0) && falling == true)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("FeedBox")))
                    {
                        if (gameInstance.GameIns.restaurantManager.fishNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
                        {
                            if (currentBox.GetFish(false)) gameInstance.GameIns.restaurantManager.UseFish();
                        }
                        else
                        {
                            if (currentBox.ClearFishes())
                            {
                                UnlockHire(true);
                                DisableInput(false);
                                animalController.reward = null;
                                gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                            }
                            else
                            {
                                UnlockHire(true);
                                DisableInput(false);
                                gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                            }
                            yield break;
                        }
                    }
                    else
                    {
                        if (currentBox.ClearFishes())
                        {
                            UnlockHire(true);
                            DisableInput(false);
                            animalController.reward = null;
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                        }
                        else
                        {
                            UnlockHire(true);
                            DisableInput(false);
                            gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                        }
                        if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                        {
                            if (hit2.collider.GetComponentInParent<Animal>())
                            {
                                Employee newAnimal = hit2.collider.GetComponentInParent<Animal>().GetComponentInChildren<Employee>();

                                ShowPenguinUpgradeInfo(newAnimal);

                            }
                            yield break;
                        }
                    }
                }
                else
                {
                    currentBox.StopFish();
                }
            }
            else
            {
                yield break;
                //  Debug.Log("Bug");
                //   DisableInput(false);
            }
            yield return null;
        }

        buildBoxCoroutine = false;
    }

    public void UnlockHire(bool unlock)
    {
        if (unlock)
        {
            RestaurantManager restaurantManager = gameInstance.GameIns.restaurantManager;
            int num = restaurantManager.playerData.employeeNum;
            if (num < 8 && restaurantManager.employeeHire[num] <= restaurantManager.GetRestaurantValue())
            {
                hireBtn.gameObject.SetActive(unlock);
            }
        }
        else
        {
            hireBtn.gameObject.SetActive(unlock);
        }
    }

    public void UIClearAll()
    {
        animalController = null;
        foodMachine = null;
        hireBtn.gameObject.SetActive(false);

        appliancePanel.SetActive(false);
        StopAllCoroutines();
    }
}

