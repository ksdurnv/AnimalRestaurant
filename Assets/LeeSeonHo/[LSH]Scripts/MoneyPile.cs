using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoneyPile : MonoBehaviour
{
    public MoneyManager MoneyManager;
    public GameObject moneyPrefab;
    public int earnCount;
    private int remainMoney = 0;

    public CurrentMoney currentMoney;
    public void EarnMoney(int n)
    {
        float PosX;
        float PosY;
        float PosZ = 0;

        for (int i = remainMoney; i < remainMoney + n; i++)
        {
            if (i % 2 == 0)
            {
                PosX = -0.4f;
            }
            else
            {
                PosX = 0.4f;
            }

            PosY = i / 8 * 0.1f;

            switch (i % 8)
            {
                case 0:
                case 1:
                    PosZ = -0.6f;
                    break;
                case 2:
                case 3:
                    PosZ = -0.2f;
                    break;
                case 4:
                case 5:
                    PosZ = 0.2f;
                    break;
                case 6:
                case 7:
                    PosZ = 0.6f;
                    break;
            }

            moneyPrefab.transform.localPosition = new Vector3(PosX, PosY, PosZ);

            Instantiate(moneyPrefab, this.transform);

        }

        remainMoney += n;
    }

    /*public void EarnMoney()
    {       
        float PosX;
        float PosY;
        float PosZ = 0;

        for(int i = remainMoney; i < remainMoney+earnCount; i++)
        {
            if(i % 2 == 0)
            {
                PosX = -0.8f;
            }
            else
            {
                PosX = 0.8f;
            }

            PosY = i / 8 * 0.2f;

            switch(i % 8)
            {
                case 0:
                case 1:
                    PosZ = -1.2f;
                    break;
                case 2:
                case 3:
                    PosZ = -0.4f;
                    break;
                case 4:
                case 5:
                    PosZ = 0.4f;
                    break;
                case 6:
                case 7:
                    PosZ = 1.2f;
                    break;
            }

            moneyPrefab.transform.localPosition = new Vector3(PosX, PosY, PosZ);

            Instantiate(moneyPrefab, this.transform);          
            
        }

        remainMoney += earnCount;
    }*/

    public void RemoveAllChildren()
    {
     //   Debug.Log($"{remainMoney*1000}���� �������ϴ�.");

        remainMoney = 0;
        StartCoroutine(DestroyChildren());
    }

    IEnumerator DestroyChildren()
    {    

        GameInstance ins = new GameInstance();
        
        int childCount = transform.childCount;
        int childDivide5 = childCount / 5;

        if (childDivide5 > 0)
        {
            for (int i = 0; i < childDivide5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Transform child = transform.GetChild(childCount - 1 - j - i * 5);

                    Destroy(child.gameObject);
                    ins.GameIns.moneyManager.money += 1000;
                }
                yield return null;
            }
        }
        else
        {
            for (int i = childCount - 1; i >= 0; i--) // �������� ����
            {
                Transform child = transform.GetChild(i);

                Destroy(child.gameObject);
                ins.GameIns.moneyManager.money += 1000;

                yield return null;
            }
        }


    }

    //IEnumerator DestroyChildren()
    //{
    //    int childCount = transform.childCount;

    //    for (int i = childCount - 1; i >= 0; i--) // �������� ����
    //    {
    //        Transform child = transform.GetChild(i);

    //        // ��ũ�� �»�������� ��ġ ����
    //        Vector3 screenTopLeft = new Vector3(0, 0, 0);
    //        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenTopLeft);
    //        worldPosition.z = child.position.z; // z�� ��ġ ����

    //        // �ڽ� ������Ʈ�� ��ũ�� �»������ �̵�
    //        float moveDuration = 0.01f; // �̵� �ð�
    //        float elapsedTime = 0f;

    //        while (elapsedTime < moveDuration)
    //        {
    //            child.position = Vector3.Lerp(child.position, worldPosition, elapsedTime / moveDuration);
    //            elapsedTime += Time.deltaTime;
    //            yield return null;
    //        }

    //        // ���� ��ġ ����
    //        child.position = worldPosition;

    //        // ������Ʈ ���� �� �� �߰�
    //        Destroy(child.gameObject);
    //        currentMoney.AddMoney(1000);
    //    }
    //}

}
