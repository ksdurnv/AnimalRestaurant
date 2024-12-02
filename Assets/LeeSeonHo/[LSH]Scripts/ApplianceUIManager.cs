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

    public GameObject rootUI;
    public GameObject appliancePanel;
    public Image applianceImage;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI upgradeInfoText;

    public Button hireBtn;
    public Button cleanBtn;
    public GameObject trashCan;

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
    Coroutine buildCoroutine;
    public bool useDescription = false;
    bool bHidden;
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
        cleanBtn.onClick.AddListener(() =>
        {
            gameInstance.GameIns.inputManager.bCleaningMode = !gameInstance.GameIns.inputManager.bCleaningMode;
        });
        appliancePanel.SetActive(false);
    }
    private void Start()
    {
        // gameInstance.GameIns.applianceUIManager = this;
     //   appliancePanel.SetActive(false); // 시작할 때는 패널 비활성화
    }

    Coroutine infoCoroutine;
    public void ShowApplianceInfo(FoodMachine foodMachine)
    {
        if (scheduleCoroutine == null)
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
            if (!bHidden)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GraphicRaycaster ggr2 = gameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                    PointerEventData ped2 = new PointerEventData(es);
                    ped2.position = Input.mousePosition;
                    List<RaycastResult> raycastResults2 = new List<RaycastResult>();
                    ggr2.Raycast(ped2, raycastResults2);
                    bool chck2 = false;
                    for (int i = 0; i < raycastResults2.Count; i++)
                    {
                        if (raycastResults2[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            Debug.Log("child");
                            chck2 = true;
                            break;
                        }
                    }



                    PointerEventData ped = new PointerEventData(es);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    gr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<ApplianceUIManager>() || raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            Debug.Log("Chid");
                            chck = true;
                            break;
                        }
                    }

                    Ray rayGround = Camera.main.ScreenPointToRay(Input.mousePosition);
                    bool checkGround = Physics.Raycast(rayGround, out RaycastHit hit3, Mathf.Infinity, 1);
                    if (!chck)
                    {
                        gameInstance.GameIns.inputManager.inputDisAble = false;
                        gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                        infoCoroutine = null;

                    }
                    else
                    {
                        gameInstance.GameIns.inputManager.inputDisAble = true;
                        gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    PointerEventData ped = new PointerEventData(es);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    gr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<ApplianceUIManager>() || raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            chck = true;
                            break;
                        }
                    }

                    if (!chck)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                        {
                            if (hit2.collider.GetComponentInParent<Animal>())
                            {
                                Employee newAnimal = hit2.collider.GetComponentInParent<Animal>().GetComponentInChildren<Employee>();

                                infoCoroutine = null;
                                float test1 = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test1 < 0.05f)
                                {
                                    ShowPenguinUpgradeInfo(newAnimal, true);
                                    gameInstance.GameIns.inputManager.inputDisAble = false;
                                    //   yield break;
                                    yield break;
                                }
                                // continue;
                            }
                        }

                        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray2, out RaycastHit hit3, Mathf.Infinity, 1 << 13))
                        {
                            if (hit3.collider.GetComponentInParent<FoodMachine>())
                            {
                                FoodMachine foodMachine = hit3.collider.GetComponentInParent<FoodMachine>();

                                infoCoroutine = null;
                                float test2 = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test2 < 0.05f)
                                {
                                    ShowApplianceInfo(foodMachine);
                                    gameInstance.GameIns.inputManager.inputDisAble = false;
                                    //   yield break;
                                    yield break;
                                }
                                // continue;
                            }

                        }
                        float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                        if (test < 0.05f)
                            break;
                    }
                }
            }
            yield return null;
        }

        Debug.Log("Aaa");
        UnlockHire(true);
        appliancePanel.SetActive(false);
        infoCoroutine = null;
       
    }
   
    public void ShowPenguinUpgradeInfo(Employee animal, bool justUpdate = false)
    {


        if (scheduleCoroutine == null)
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
                if (infoCoroutine != null) StopCoroutine(infoCoroutine);
                //  gameInstance.GameIns.inputManager.inputDisAble = true;
                
              
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
        if (useDescription)
        {

            feedingDescription.gameObject.SetActive(true);
            feedingDescription.text = "Click the floor for build a feeding box";
        }

        yield return null;
        //while (gameInstance.GameIns.inputManager.inputDisAble)
        while (true) 
        {
            float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
            gameInstance.GameIns.inputManager.inputDisAble = false;
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                {
                    if (!Physics.CheckBox(hit.point, new Vector3(1f, 1f, 1f), Quaternion.identity, 1 << 6 | 1 << 7))
                    {
                        //   float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                        if (test < 0.05f)
                        {
                            buildCoroutine = StartCoroutine(BuildPackageBox(hit.point));

                            yield break;
                        }
                    }
                }
            }
            yield return null;
        }
       // scheduleCoroutine = null;
    }

    IEnumerator BuildPackageBox(Vector3 endPosition)
    {
        gameInstance.GameIns.inputManager.inputDisAble = false;
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
       // while (gameInstance.GameIns.inputManager.inputDisAble)
        while(true)
        {
            
            if (currentBox != null && !bHidden)
            {
                if ((Input.GetMouseButton(0)) && falling == false || Input.GetMouseButtonDown(0))
                {
                    GraphicRaycaster ggr = gameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                    PointerEventData ped = new PointerEventData(es);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    ggr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            chck = true;
                            break;
                        }
                    }
                    if (!chck)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 12))
                        {
                            if (Physics.CheckBox(hit.point, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, LayerMask.GetMask("FeedBox")))
                            {
                                float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test < 0.05f)
                                {
                                    if (gameInstance.GameIns.restaurantManager.fishNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
                                    {
                                        if (currentBox.GetFish(true)) gameInstance.GameIns.restaurantManager.UseFish();
                                        falling = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Input.GetMouseButton(0) && falling == true)
                {
                    GraphicRaycaster ggr = gameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                    PointerEventData ped = new PointerEventData(es);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    ggr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            Debug.Log("child");
                            chck = true;
                            break;
                        }
                    }

                    if (!chck)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("FeedBox")))
                        {
                            if (gameInstance.GameIns.restaurantManager.fishNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
                            {
                                float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test < 0.05f)
                                {
                                    if (currentBox.GetFish(false)) gameInstance.GameIns.restaurantManager.UseFish();
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        GraphicRaycaster ggr = gameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                        PointerEventData ped = new PointerEventData(es);
                        ped.position = Input.mousePosition;
                        List<RaycastResult> raycastResults = new List<RaycastResult>();
                        ggr.Raycast(ped, raycastResults);
                        bool chck = false;
                        for (int i = 0; i < raycastResults.Count; i++)
                        {
                            if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                            {
                                Debug.Log("child");
                                chck = true;
                                break;
                            }
                        }

                        if (!chck)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("FeedBox")))
                            {
                                currentBox.StopFish();
                            }
                            else
                            {
                                float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test < 0.05f)
                                {
                                    if (currentBox.ClearFishes())
                                    {
                                        UnlockHire(true);
                                        DisableInput(false);
                                        // animalController.reward = null;
                                        //     gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                                    }
                                    else
                                    {
                                        UnlockHire(true);
                                        DisableInput(false);
                                        //   gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                                    }

                                    scheduleCoroutine = null;

                                    Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    if (Physics.Raycast(ray2, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                                    {
                                        if (hit2.collider.GetComponentInParent<Animal>().GetComponent<Employee>())
                                        {
                                            ShowPenguinUpgradeInfo(hit2.collider.GetComponentInParent<Animal>().GetComponent<Employee>(), false);
                                        }
                                    }
                                    Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    if (Physics.Raycast(ray3, out RaycastHit hit3, Mathf.Infinity, 1 << 13))
                                    {
                                        if (hit3.collider.GetComponentInParent<FoodMachine>())
                                        {
                                            FoodMachine foodMachine = hit3.collider.GetComponentInParent<FoodMachine>();

                                            infoCoroutine = null;

                                            ShowApplianceInfo(foodMachine);
                                            gameInstance.GameIns.inputManager.inputDisAble = false;
                                            //   yield break;
                                            yield break;

                                            // continue;
                                        }

                                    }
                                    yield break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentBox == null)
                {
                    feedingDescription.gameObject.SetActive(false);
                    yield break;
                }
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

    public void UIClearAll(bool visible)
    {

        Debug.Log(("UIClearAll"));
        bHidden = !visible;
        if (visible)
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("ApplianceUI"));
        }
        else
        {
            GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("ApplianceUI"));
        }
    }
}

