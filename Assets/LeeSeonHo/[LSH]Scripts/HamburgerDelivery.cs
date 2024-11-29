using UnityEngine;
using System.Collections.Generic;

public class HamburgerDelivery : MonoBehaviour
{
    public Transform startPos;
    public Transform initialDestination;
    public Transform deliveryPoint;
    public GameObject hamburgerSetPrefab;
    public float moveSpeed = 0.5f;
    public float orderQuantity = 3f;
    public GameObject penguin;
    public GameObject delieverPenguin;

    private Vector3 initialDestinationPosition; // ������ ���� ���� ��ġ ��
    private Vector3 penguinInitialPosition; // �ر��� ���� ��ġ ����
    private Vector3 deliverPenguinInitialPosition; // ��� �ر��� ���� ��ġ ����
    private List<GameObject> hamburgerSets = new List<GameObject>();
    private int index = 0;

    void Start()
    {
        initialDestinationPosition = initialDestination.position; // �ʱ� ��ġ ����
        penguinInitialPosition = penguin.transform.position; // �ر��� ���� ��ġ ����
        deliverPenguinInitialPosition = delieverPenguin.transform.position; // ��� �ر��� ���� ��ġ ����
        delieverPenguin.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (index < orderQuantity)
            {
                CreateAndMoveHamburgerSet();
            }
            else
            {
                DeliverHamburgerSets();
            }
        }
    }

    void CreateAndMoveHamburgerSet()
    {
        GameObject hamburgerSet = Instantiate(hamburgerSetPrefab, startPos.position, Quaternion.identity);
        hamburgerSets.Add(hamburgerSet);
        StartCoroutine(MoveToPosition(hamburgerSet, initialDestination.position + Vector3.up * (0.3f * index), moveSpeed));

        index++;
    }

    System.Collections.IEnumerator MoveToPosition(GameObject obj, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2));
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;
            yield return null;
        }
    }

    void DeliverHamburgerSets()
    {
        foreach (GameObject hamburgerSet in hamburgerSets)
        {
            StartCoroutine(MoveToDeliveryPoint(hamburgerSet, deliveryPoint.position, 1f));
        }
    }

    System.Collections.IEnumerator MoveToDeliveryPoint(GameObject obj, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        float elapsed = 0;
        delieverPenguin.SetActive(true);
        penguin.SetActive(false);

        yield return new WaitForSeconds(1f);

        // ��� �ر��� ���� ��ġ���� ��� ��ġ�� �̵�
        Vector3 penguinStartPos = penguin.transform.position;
        Vector3 deliverPenguinStartPos = deliverPenguinInitialPosition; // ��� �ر��� �ʱ� ��ġ

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2));
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;

            // ��� �ر� �̵�            
            delieverPenguin.transform.position = Vector3.Lerp(deliverPenguinStartPos, targetPosition, t) + Vector3.up * height;

            yield return null;
        }

        obj.SetActive(false);
        delieverPenguin.SetActive(false); // ��� �� �ر� ��Ȱ��ȭ

        // ��� �ܹ��� ��Ʈ�� ��Ȱ��ȭ�Ǿ����� �ʱ�ȭ
        if (hamburgerSets.TrueForAll(h => !h.activeSelf))
        {
            ResetDelivery();
        }
    }

    void ResetDelivery()
    {
        index = 0;
        initialDestination.position = initialDestinationPosition; // ���� ������ ���� ��ġ�� �ʱ�ȭ
        hamburgerSets.Clear(); // ����Ʈ �ʱ�ȭ
        delieverPenguin.SetActive(false); // ��� �ر� ��Ȱ��ȭ
        penguin.SetActive(true); // ���� �ر� Ȱ��ȭ

        // �رϰ� ��� �ر��� ���� ��ġ�� �̵�
        penguin.transform.position = penguinInitialPosition; // �ر��� ���� ��ġ�� ���ư�
        delieverPenguin.transform.position = deliverPenguinInitialPosition; // ��� �ر��� ���� ��ġ�� ���ư�
    }
}
