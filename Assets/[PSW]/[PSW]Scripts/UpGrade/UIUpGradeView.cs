using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FoodMachine;

public class UIUpGradeView : MonoBehaviour
{
    public GameObject contents;
    public GameObject uiListPrefab;
    GameInstance gameInstance;
    public int boxNumber = 10; // �����Ҷ� �����Ǵ� �ڽ� ����
    public float y = -30f; // �ڽ��� �ڽ��� y�� ���� ����
    public float initialYPosition = 130f;  // ù��° ������ �ڽ� ���� ���� ��ġ

    // Start is called before the first frame update
    void Start()
    {
        gameInstance = new GameInstance();
        boxPos();
    }

    void boxPos()
    {
        int foodMachinesCount = gameInstance.GameIns.workSpaceManager.foodMachines.Count;
        for (int i = 0; i < foodMachinesCount; i++)
        {
            //Instantiate<GameObject>(this.uiListPrefab, contents.transform);
            // ù ������ ��ġ ���
            float yPos = initialYPosition + (y * i);
            // ���ο� ��ġ ���
            Vector3 newPosition = new Vector3(contents.transform.position.x, yPos, contents.transform.position.z);
            // ������ �ν��Ͻ� ���� �� ��ġ ����
            GameObject newUI = Instantiate<GameObject>(this.uiListPrefab, newPosition, Quaternion.identity, contents.transform);
            // �θ� �ڽ� ���� ����
            newUI.transform.SetParent(contents.transform, false);            
        }
    }
}
