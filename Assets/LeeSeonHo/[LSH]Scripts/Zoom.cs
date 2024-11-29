using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{
    public Camera cameraToMove; // �̵��� ī�޶�
    public Transform playerObject; // �Ÿ� ������ �� �÷��̾� ������Ʈ
    private Vector3 originalPosition; // ī�޶��� ���� ��ġ
    private Quaternion originalRotation; // ī�޶��� ���� ȸ��
    public static bool isMoving; // ī�޶� �̵� ����

    private Transform targetObject; // ī�޶� ������ ���� ������Ʈ
    private const float smoothSpeed = 0.1f; // ī�޶� �̵� �ӵ� (������)

    public bool skip =false;

    private void OnEnable()
    {
        if (skip == false)
        {
            targetObject = transform; // �ڽ��� Ÿ�� ������Ʈ�� ����
            isMoving = true; // ī�޶� �̵� ����
            StartCoroutine(PositionCameraWithDelay(1f)); // 1�� �ڿ� ī�޶� ��ġ ����
        }
    }

    private IEnumerator PositionCameraWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ���� �ð� ���
        StartCoroutine(SmoothMoveCamera());
    }

    private IEnumerator SmoothMoveCamera()
    {
        if (cameraToMove == null || playerObject == null)
        {
            yield break; // ī�޶� �÷��̾ ������ ����
        }

        // ī�޶��� ���� ��ġ�� ȸ�� ����
        originalPosition = cameraToMove.transform.position;
        originalRotation = cameraToMove.transform.rotation;

        // Ÿ�� ������Ʈ�� ��ġ
        Vector3 targetPosition = targetObject.position;

        // ī�޶�� �÷��̾� ���� �Ÿ� ���
        float distanceFromTarget = Vector3.Distance(cameraToMove.transform.position, playerObject.position);

        // ī�޶��� ��ǥ ��ġ ���
        Vector3 cameraOffset = new Vector3(0, 0, -distanceFromTarget); // Z������ �̵�
        Vector3 newCameraPosition = targetPosition + cameraToMove.transform.rotation * cameraOffset;

        // ī�޶� �ε巴�� �̵�
        float elapsedTime = 0f;
        Vector3 startingPosition = cameraToMove.transform.position;

        while (elapsedTime < smoothSpeed)
        {
            cameraToMove.transform.position = Vector3.Lerp(startingPosition, newCameraPosition, elapsedTime / smoothSpeed);
            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // ������ ��ġ�� ��Ȯ�� ����
        cameraToMove.transform.position = newCameraPosition;

        // ������Ʈ�� �ٶ󺸵��� ī�޶� ȸ��
        cameraToMove.transform.LookAt(targetPosition);

        // 1�� ��� �� ���� ��ġ�� ���ư��� 
        yield return new WaitForSeconds(1f);
        StartCoroutine(ReturnToOriginalPosition());
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        float elapsedTime = 0f;

        while (elapsedTime < smoothSpeed)
        {
            cameraToMove.transform.position = Vector3.Lerp(cameraToMove.transform.position, originalPosition, elapsedTime / smoothSpeed);
            cameraToMove.transform.rotation = Quaternion.Lerp(cameraToMove.transform.rotation, originalRotation, elapsedTime / smoothSpeed);
            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ��ġ�� ȸ���� ��Ȯ�� ����
        cameraToMove.transform.position = originalPosition;
        cameraToMove.transform.rotation = originalRotation;

        isMoving = false; // ī�޶� �̵� ����
    }
}
