using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static FoodMachine;

public class HamburgerPackaging : MonoBehaviour
{
    public Transform startPos; // �ܹ��� ��� ��ġ
    public Transform hambergerSetStartPos; // �ܹ��� ��Ʈ ��� ��ġ
    public GameObject box; // �ڽ� ������Ʈ
    public GameObject[] targetPositions; // �ڽ� ���� ��ǥ ��ġ �迭
    public GameObject a, b, c, d; // ���������� Ȱ��ȭ�� ������Ʈ��
    public GameObject hamburgerSetPrefab; // �ܹ��� ��Ʈ ������
    public Transform destinationTrans; // �ܹ��� ��Ʈ�� ���� ���� ��ġ
    public Mesh hamburgerSetMesh;
    public MachineType machineType;

    private PackingTable packingTable;
    private PackageFood packageBox;
    private Food f; // �ܹ���
    public Action packingAction;

    private float moveSpeed = 0.2f;

    private int currentPositionIndex = 0;
    private int CurrentPackedNumber { get { return currentPositionIndex; } set { currentPositionIndex = value; packingTable.packingNumber = value; } }
  //  private bool allFilled = false;
    private bool isHamburgerSetMoving = false; // ��Ʈ �̵� �� ���θ� Ȯ���� ����


    float coroutineTimer = 0;
    private void Start()
    {
        packingTable = GetComponent<PackingTable>();
        PackageFood hamburgerSet = (PackageFood)FoodManager.GetPackageBox();
        hamburgerSet.parentType = MachineType.PackingTable;
        packageBox = hamburgerSet;
        packageBox.transform.position = packingTable.smallTable.position;
    }

    void Update()
    {
        // �ʿ信 ���� ���� Ȯ��
    }
    float elapsed = 0;
    public void StartMove(Food food, bool bStart)
    {
     
        StartCoroutine(MoveFood(food, false));
    }
    public IEnumerator MoveFood(Food food, bool bStart)
    {

        if (currentPositionIndex < targetPositions.Length && !isHamburgerSetMoving) // �÷��� Ȯ��
        {
            if (bStart)
            {
                elapsed = 0;
                coroutineTimer = 0;
            }
            //
            //f = food;
                    //   yield return StartCoroutine(MoveToPosition(f, targetPositions[currentPositionIndex].transform.position, moveSpeed));

            Vector3 startPosition = food.transform.position;
         
            while (elapsed < moveSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveSpeed;
                float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2)); // ������ ����
                food.transform.position = Vector3.Lerp(startPosition, packageBox.packageTrans[currentPositionIndex].transform.position, t) + Vector3.up * height;
                yield return null;
            }

            food.transform.SetParent(packageBox.packageTrans[currentPositionIndex].transform);
            food.transform.localPosition = Vector3.zero;
            //FoodManager.EatFood(f);
            // obj.SetActive(false); // ���� �� �ܹ��� ��Ȱ��ȭ

            // ���� �� �ش� ��ġ�� ���� a, b, c, d ������Ʈ Ȱ��ȭ
            currentPositionIndex++;
            ActivatePositionObject();
            packingAction = null;
            if (currentPositionIndex == 4)// targetPositions.Length)
            {
                //    allFilled = true;
                packingAction += () => { ResumeCreateHamburgerSet(); };
                yield return StartCoroutine(CreateHamburgerSet());
              //  Invoke("CreateHamburgerSet", moveSpeed); // �ܹ��� ��Ʈ ���� ����
            }
        }
    }

   /* System.Collections.IEnumerator MoveToPosition(Food obj, Vector3 targetPosition, float duration)
    {
        Debug.Log("MoveToPosition");
        Vector3 startPosition = obj.transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2)); // ������ ����
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;
            yield return null;
        }

       // obj.transform.SetParent()

     //   FoodManager.EatFood(obj);
       // obj.SetActive(false); // ���� �� �ܹ��� ��Ȱ��ȭ

        // ���� �� �ش� ��ġ�� ���� a, b, c, d ������Ʈ Ȱ��ȭ
        ActivatePositionObject();
    }*/

    void ActivatePositionObject()
    {
        if (currentPositionIndex - 1 == 0) a.SetActive(true);
        else if (currentPositionIndex - 1 == 1) b.SetActive(true);
        else if (currentPositionIndex - 1 == 2) c.SetActive(true);
        else if (currentPositionIndex - 1 == 3) d.SetActive(true);
    }

    public void ResumeCreateHamburgerSet()
    {
        StartCoroutine(CreateHamburgerSet());
    }
    IEnumerator CreateHamburgerSet()
    {
        while(coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }
      //  yield return new WaitForSeconds(0.2f);
        // a, b, c, d ������Ʈ �� �ڽ� ��Ȱ��ȭ
        a.SetActive(false);
        b.SetActive(false);
        c.SetActive(false);
        d.SetActive(false);
        box.SetActive(false); // �ڽ� ��Ȱ��ȭ

        PackageFood pf = packageBox;
        PackageFood hamburgerSet = (PackageFood)FoodManager.GetPackageBox();
        hamburgerSet.parentType = MachineType.PackingTable;
        hamburgerSet.transform.position = packingTable.smallTable.position;
        packageBox = hamburgerSet;
        packingAction = null;

        packingAction += () => { ResumeMoveHamburgerSetToDestination(pf, destinationTrans.position, moveSpeed); };
        coroutineTimer = 0;
        yield return StartCoroutine(MoveHamburgerSetToDestination(pf, destinationTrans.position, moveSpeed));

        packingAction = null;
        // ���� �ܹ��� ��Ʈ �غ� ���� ���� �ʱ�ȭ
        currentPositionIndex = 0;
      //  allFilled = false;
    }

    public void ResumeMoveHamburgerSetToDestination(Food food, Vector3 targetPosition, float duration, bool destroy = false)
    {
        StartCoroutine(MoveHamburgerSetToDestination(food, targetPosition, duration, destroy));
    }
    System.Collections.IEnumerator MoveHamburgerSetToDestination(Food food, Vector3 targetPosition, float duration, bool destroy=false)
    {
        isHamburgerSetMoving = true; // ��Ʈ �̵� ���� �� �÷��� Ȱ��ȭ
        Vector3 startPosition = box.transform.position;

        // Count�� ���� targetPosition�� y �� ����
        targetPosition = new Vector3(destinationTrans.position.x, destinationTrans.position.y + 0.63f * packingTable.packageStack.foodStack.Count, destinationTrans.position.z);

       // float elapsed = 0;

        while (coroutineTimer < duration)
        {
            coroutineTimer += Time.deltaTime;
            float t = coroutineTimer / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2)); // ������ ����

            // ��ǥ ��ġ�� �������� �׸��� �̵�
            food.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;

            yield return null;
        }

        food.transform.position = targetPosition;
       
        // ���ÿ� food �߰�
        packingTable.packageStack.foodStack.Add(food);
       // Debug.Log("Stack Count: " + packingTable.packageStack.foodStack.Count);

     //   if(destroy)FoodManager.EatFood(food);
        // �ڽ� Ȱ��ȭ �� �÷��� ��Ȱ��ȭ
  //      box.SetActive(true);
        isHamburgerSetMoving = false;
        packingAction = null;
        coroutineTimer = 0;
        currentPositionIndex = 0;
        // packingAction = null;
    }


}
