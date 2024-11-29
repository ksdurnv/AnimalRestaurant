using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CryingSnow.FastFoodRush;
using System;

public class UnlockableBuyer : MonoBehaviour
{
    public MoneyManager MoneyManager;
    public RestaurantManager restaurantManager;
    public Image ProgressFill; // ProgressFill 이미지
    public TextMeshProUGUI PriceLabel; // PriceLabel 텍스트
    public float maxPrice = 10.00f; // 최대 가격
    public float fillSpeed = 1.0f; // Fill Amount 증가 속도
    GameInstance gameInstance = new GameInstance();

    private float fillAmount = 0f; // 현재 Fill Amount
    private float currentPrice; // 현재 가격
    private float firstBalance;
    private float number;
    private Coroutine fillCoroutine; // Fill Progress 코루틴

    public Vector3 blockExtends;
    
    void Start()
    {
      
       // firstBalance = MoneyManager.money;
        currentPrice = maxPrice; // 시작 가격 설정
        UpdatePriceLabel(); // 초기 가격 표시
    }

    void OnMouseDown()
    {
        // 마우스 버튼을 눌렀을 때 Fill Progress 코루틴 시작
      //  fillCoroutine = StartCoroutine(FillProgress());
        //inputManger.inOtherAction = true;

        
    }
    public void MouseDown()
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }
        fillCoroutine = StartCoroutine(FillProgress());
        gameInstance.GameIns.inputManager.inOtherAction = true;

    }
    public void MouseUp()
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        gameInstance.GameIns.inputManager.inOtherAction = false;
        gameInstance.GameIns.inputManager.manyFingers = true;
    }
    private void OnMouseOver()
    {
       // inputManger.inOtherAction = true;
        
    }
    private void OnMouseExit()
    {
      /*  if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        inputManger.inOtherAction = false;
        inputManger.manyFingers = true;*/
    }
    private void OnDisable()
    {
      //  gameInstance.GameIns.inputManager.inOtherAction = false;
        //gameInstance.GameIns.inputManager.manyFingers = true;
    }
    void OnMouseUp()
    {
        // 마우스 버튼을 놓았을 때 Fill Progress 코루틴 중지
      /*  if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        inputManger.inOtherAction = false;
        inputManger.manyFingers = true;*/
        // inputManger.a = true;
    }

    IEnumerator FillProgress()
    {
     
        // 0.2초 대기
        yield return new WaitForSeconds(0.2f);

        while (fillAmount < 1f && gameInstance.GameIns.moneyManager.money > 0)
        {
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, 1f); // Fill Amount 제한
            ProgressFill.fillAmount = fillAmount;

            // 가격 감소
            if (currentPrice > 0 && gameInstance.GameIns.moneyManager.money > 0)
            {
                float priceDecrease = maxPrice * Time.deltaTime; // maxPrice로 가격 감소

                // 가격과 현재 보유 금액 모두 감소
                if (gameInstance.GameIns.moneyManager.money >= priceDecrease)
                {
                    if (currentPrice >= priceDecrease)
                    {
                        currentPrice -= priceDecrease;
                        gameInstance.GameIns.moneyManager.money -= priceDecrease; // 보유 금액 감소    
                    }
                    else
                    {
                        gameInstance.GameIns.moneyManager.money -= currentPrice;
                        currentPrice = 0;

                    }
                                  
                }
                else
                {
                    if(currentPrice > gameInstance.GameIns.moneyManager.money)
                    {
                        currentPrice -= gameInstance.GameIns.moneyManager.money;
                        gameInstance.GameIns.moneyManager.money = 0;
                    }
                    else if(gameInstance.GameIns.moneyManager.money > currentPrice)
                    {
                        gameInstance.GameIns.moneyManager.money -= currentPrice;
                        currentPrice = 0;
                    }
                    else
                    {
                        currentPrice = 0;
                        gameInstance.GameIns.moneyManager.money = 0;
                    }
                   
                }
                UpdatePriceLabel(); // 가격 업데이트
            }
            yield return null; // 다음 프레임까지 대기
        }

        // Fill이 완료되면 추가 처리
        if (fillAmount >= 1f)
        {
            BoxCollider bc = gameInstance.GameIns.restaurantManager.levels[gameInstance.GameIns.restaurantManager.level].GetComponent<BoxCollider>();
            if (bc != null)
            {
                for (int i = 0; i < gameInstance.GameIns.restaurantManager.flyingEndPoints.Count; i++)
                {
                   // Collider[] cold = Physics.OverlapBox(gameInstance.GameIns.restaurantManager.flyingEndPoints[i], new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 10);

                   // for(int j=0; j<cold.Length; j++)
                    {
                        //cold[j].
                    }
                    //checkFlyingAnimal = Physics.CheckBox(gameInstance.GameIns.restaurantManager.flyingEndPoints[i], new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 13);

                }
                bool checkAnimal = Physics.CheckBox(bc.transform.position+ bc.center, bc.size, bc.transform.rotation, 1 << 10);
                bool checkFlyingAnimal = false;
               

                if (checkAnimal || checkFlyingAnimal)
                {
                    Debug.Log("Animal");
                }
                else PurchaseItem();
            }
            else
            {
                PurchaseItem();
            }
                    //    bool checkAnimal = Physics.CheckBox(bc.bounds.center, bc.bounds.size, bc.transform.rotation, 1 << 10);
                 //       if (!checkAnimal) PurchaseItem();
                     //   else Debug.Log("Animal Exists");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(6.5f, 6, 4f));
    }

    void PurchaseItem()
    {
        gameInstance.GameIns.inputManager.inOtherAction = false;
        gameInstance.GameIns.inputManager.manyFingers = true;
        restaurantManager.LevelUp();
    }

    void UpdatePriceLabel()
    {
        if (currentPrice >= 1_000_000_000)
        {
            // 1_000_000_000 이상일 경우 B 단위로 표시
            PriceLabel.text = ((currentPrice) / 1_000_000_000).ToString("0.00") + "B";
        }
        else if (currentPrice >= 1_000_000)
        {
            // 1000000 이상일 경우 M 단위로 표시
            PriceLabel.text = ((currentPrice) / 1_000_000).ToString("0.00") + "M";
        }
        else if (currentPrice >= 1_000)
        {
            // 1000 이상일 경우 K 단위로 표시
            PriceLabel.text = ((currentPrice) / 1000).ToString("0.00") + "K";
        }
        else
        {
            // 1000 미만일 경우 일반 표시
            PriceLabel.text = (currentPrice).ToString("0");
        }
    }
}
