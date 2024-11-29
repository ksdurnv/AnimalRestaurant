using UnityEngine;

public class Rolling : MonoBehaviour
{
    private GameInstance gameInstance = new GameInstance();

    private Animator animator;
    private bool isMoving = false; // �̵� ���¸� ����
    private Vector3 targetPosition; // ��ǥ ��ġ

    public float moveSpeed; // �̵� �ӵ�


    void Start()
    {
        targetPosition = Vector3.zero + new Vector3(this.transform.position.x, 0, -1000);
        animator = GetComponentInChildren<Animator>();
        isMoving = true; // �̵� ����
        animator.SetTrigger("Roll"); // Roll �ִϸ��̼� ����
        moveSpeed = gameInstance.GameIns.gatcharManager.rollingSpeed;
    }

    void Update()
    {
        if (isMoving)
        {
            // ���� ��ġ���� ��ǥ ��ġ�� õõ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.unscaledDeltaTime); //deltaTime);

            // ��ǥ ��ġ�� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                transform.position = targetPosition;
                isMoving = false; // �̵� ����
                animator.SetTrigger("Idle A"); // Idle A �ִϸ��̼� ����
            }
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
