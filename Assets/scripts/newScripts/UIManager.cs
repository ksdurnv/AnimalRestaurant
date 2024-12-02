using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public TMP_Text moneyText;
    GameInstance instance = new GameInstance();
  
    public Order[] order;

    public Button animalGuideButton;
    public Panel panel;
    public Button goRestaurant;
    public Button goDraw;
    public Button drawBtn;
    public Button drawSpeedUpBtn;
    public GameObject animalGuide;
    public Image fadeImage;
    bool bGuideOn = false;

    private float cameraSize;

    // Start is called before the first frame update
    private void Awake()
    {
        panel = GetComponentInChildren<Panel>();
        instance.GameIns.uiManager = this;
    }
    void Start()
    {
        animalGuideButton.onClick.AddListener(() => { 
            if(bGuideOn)
            {
                bGuideOn = false;
                
                instance.GameIns.inputManager.inputDisAble = false;
                instance.GameIns.inputManager.DragScreen_WindowEditor(true);
                
                //      animalGuide.SetActive(bGuideOn);
                StartCoroutine(FadeInFadeOut(bGuideOn,0));
            }
            else
            {
               // if(state == SceneState.Draw)
                {
                   instance.GameIns.inputManager.inputDisAble = true;
                   instance.GameIns.inputManager.DragScreen_WindowEditor(true);
                }
                bGuideOn = true;
               // animalGuide.SetActive(bGuideOn);
                StartCoroutine(FadeInFadeOut(bGuideOn, 0));
            }

        });

        goRestaurant.onClick.AddListener(() =>
        {
            if (instance.GameIns.app.currentScene != SceneState.Restaurant)
            {
                instance.GameIns.inputManager.inputDisAble = true;

                drawBtn.gameObject.SetActive(false);
                drawSpeedUpBtn.gameObject.SetActive(false);

                StartCoroutine(FadeInFadeOut(true, 1));
            }
        });

        goDraw.onClick.AddListener(() =>
        {
            if (instance.GameIns.app.currentScene != SceneState.Draw)
            {
                instance.GameIns.inputManager.inputDisAble = true;

                instance.GameIns.app.pos = instance.GameIns.inputManager.cameraTrans.position;

            /*    if (instance.GameIns.applianceUIManager.currentBox != null)
                {
                    instance.GameIns.applianceUIManager.currentBox.ClearFishes();
                }*/
                cameraSize = Camera.main.orthographicSize;                

                StartCoroutine(FadeInFadeOut(true, 2));
            }

        });

        drawBtn.onClick.AddListener(()=>{

            instance.GameIns.gatcharManager.SpownAnimal3();
        });

        drawSpeedUpBtn.onClick.AddListener(() => {

            instance.GameIns.gatcharManager.GatcharSpeedUp();
        });
    }


    IEnumerator FadeInFadeOut(bool fades,int t)
    {
      //  if (fades)
        {
            float f = 0;
            while (true)
            {   
                f += Time.unscaledDeltaTime * 8;
                Color c = fadeImage.color;
                c.a = f;
                if (fadeImage.color.a > 0.9)
                {
                    if (t == 1)
                    {
                        Camera.main.orthographicSize = cameraSize;
                    }
                    else if (t == 2)
                    {
                        Camera.main.orthographicSize = 15;
                    }
                    ShowUI(t);
                    c.a = 0;
                    fadeImage.color = c;
                    break;
                }
                    fadeImage.color = c;
                yield return null;
            }
          
        }
    }

    void ShowUI(int t)
    {
       
        if (t == 0)
        {
        
            if (bGuideOn)
            {
                instance.GameIns.applianceUIManager.UIClearAll(false);
                drawBtn.gameObject.SetActive(false);
                drawSpeedUpBtn.gameObject.SetActive(false);
            }
            else
            {
                if (instance.GameIns.app.currentScene == SceneState.Draw)
                {
                    instance.GameIns.applianceUIManager.UIClearAll(false);
                    drawBtn.gameObject.SetActive(true);
                    drawSpeedUpBtn.gameObject.SetActive(true);
                }        
                else
                {
                    instance.GameIns.applianceUIManager.UIClearAll(true);
                }
            }
            animalGuide.SetActive(bGuideOn);
        }
        else if (t == 1)
        {
            instance.GameIns.applianceUIManager.UIClearAll(true);
            animalGuide.SetActive(false);
            bGuideOn = false;
            instance.GameIns.app.ChangeScene_Restaurant();
        }
        else if (t == 2)
        {
            instance.GameIns.applianceUIManager.UIClearAll(false);
            animalGuide.SetActive(false);
            bGuideOn = false;
            instance.GameIns.app.ChangeScene_DrawScene();
        }
    }

    public void UpdateOrder(AnimalController customer, Counter.CounterType counterType)
    {
       
        List<Counter> c = instance.GameIns.workSpaceManager.counters;
        List<PackingTable> p = instance.GameIns.workSpaceManager.packingTables;
        if (counterType == Counter.CounterType.Delivery)
        {
            int count = 0;
            for (int i=0; i<p.Count; i++)
            {
                if (p[i].counterType == counterType)
                {
                    if (customer.foodStacks[0].needFoodNum - customer.foodStacks[0].foodStack.Count != 0)
                    {
                        PrintOrder(customer, customer.foodStacks[0].type, customer.foodStacks[0].needFoodNum - customer.foodStacks[0].foodStack.Count, count, counterType);
                        count++;
                    }
                    else
                    {
                        ClearOrder(counterType);
                    }
                }
            }
        }
        else
        {
            int count = 0;
            for (int i = 0; i < c.Count; i++)
            {
                if (c[i].Customer == customer)
                {
                    for (int j = 0; j < customer.foodStacks.Count; j++)
                    {
                        if (customer.foodStacks[j].needFoodNum - customer.foodStacks[j].foodStack.Count != 0)
                        {
                            PrintOrder(customer, customer.foodStacks[j].type, customer.foodStacks[j].needFoodNum - customer.foodStacks[j].foodStack.Count, count, counterType);
                            count++;
                        }
                        else if(count == 0)
                        {
                            ClearOrder(counterType);
                        }

                    }
                }
            }
        }
    }

    void ClearOrder(Counter.CounterType counterType)
    {
        order[(int)counterType - 1].gameObject.SetActive(false);
        order[(int)counterType - 1].transform.SetParent(null);
    }

    void PrintOrder(AnimalController customer, FoodMachine.MachineType type, int num, int count, Counter.CounterType counterType)
    {

        order[(int)counterType - 1].animalController = customer;
        // transform.position = order[(int)counterType - 1].animalController.transform.position + new Vector3(0, 10, 0);
        //  transform.rotation = Quaternion.Euler(new Vector3(60, 45, 0));
        order[(int)counterType - 1].transform.position = order[(int)counterType - 1].animalController.transform.position + new Vector3(0, 10, 0);
        order[(int)counterType - 1].transform.rotation = Quaternion.Euler(new Vector3(60, 45, 0));

        order[(int)counterType - 1].transform.SetParent(customer.transform);
        order[(int)counterType - 1].ShowOrder(type, num,count);
        order[(int)counterType - 1].gameObject.SetActive(true);
        if (num == 0) ClearOrder(counterType);
    }

    public float currentMoney = 900f; // 초기 돈 설정
   // public TextMeshProUGUI moneyText; // UI 텍스트를 연결할 변수

   
   /* // 돈을 추가하는 메서드
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyText();
    }
*/
   /* // 돈을 소모하는 메서드
    public void SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyText();
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
        }
    }*/

    // UI 텍스트 업데이트 메서드
    public void UpdateMoneyText(float money)
    {
        currentMoney = money;
        if (currentMoney >= 1_000_000_000)
        {
            // 1_000_000_000 이상일 경우 B 단위로 표시
            float valueInK = currentMoney / 1_000_000_000f;
            moneyText.text = valueInK.ToString("F2") + "B";
        }
        else if (currentMoney >= 1_000_000)
        {
            // 1000000 이상일 경우 M 단위로 표시
            float valueInK = currentMoney / 1_000_000f;
            moneyText.text = valueInK.ToString("F2") + "M";
        }
        else if (currentMoney >= 1_000)
        {
            // 1000 이상일 경우 K 단위로 표시
            float valueInK = currentMoney / 1_000f;
            moneyText.text = valueInK.ToString("F2") + "K";
        }
        else
        {
            // 1000 미만일 경우 일반 표시
            moneyText.text = currentMoney.ToString();
        }
    }
}
