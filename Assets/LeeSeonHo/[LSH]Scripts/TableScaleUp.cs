using UnityEngine;

public class TableScaleUp : MonoBehaviour
{
    public float duration = 0.3f; // �ִϸ��̼� ���� �ð�
    public float bounceIntensity = 0.3f; // ���� ����
    private Vector3 initialScale; // �ʱ� ������
    private float elapsedTime = 0f; // ��� �ð�
    private int bounceCount = 2; // �ݺ� Ƚ��
    private int currentBounce = 0; // ���� �ݺ� ī����
    bool load;
    private void OnEnable()
    {
        // �ʱⰪ ����
       // initialScale = transform.localScale; // ���� �������� �ʱ� �����Ϸ� ����
       // elapsedTime = 0f; // ��� �ð� �ʱ�ȭ
      //  currentBounce = 0; // ���� �ݺ� ī���� �ʱ�ȭ
    }

    public void ObjectEnabled(bool load)
    {
        this.load = load;
        initialScale = new Vector3(3, 1, 3);  //transform.localScale; // ���� �������� �ʱ� �����Ϸ� ����
        if (!load)
        {

            elapsedTime = 0f; // ��� �ð� �ʱ�ȭ
            currentBounce = 0; // ���� �ݺ� ī���� �ʱ�ȭ                              // saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();
        }
        else
        {
            transform.localScale = initialScale;
        }
      //  if (!load) Bounce();
    }
    private void Update()
    {
        // �ִϸ��̼� ���� ���̸�
        if (currentBounce < bounceCount && !load)
        {
            elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
            float t = elapsedTime / duration; // ���� ���

            // Ȧ����°: ���ηθ� Ŀ��, ¦����°: ���ηθ� Ŀ��
            float scaleMultiplier = 1 + Mathf.Sin(t * Mathf.PI) * bounceIntensity; // ���� ȿ��

            // �ִϸ��̼��� ������ ���� ���� ����
            if (t >= 1f)
            {
                bounceIntensity *= 0.5f; // ���� ������ 0.5�� ����
                elapsedTime = 0f; // ��� �ð� �ʱ�ȭ
                currentBounce++; // �ݺ� ī���� ����
            }

            Vector3 scaleChange = currentBounce % 2 == 0
                ? new Vector3(scaleMultiplier, 1, 1) // ¦��: ���ηθ�
                : new Vector3(1, 1, scaleMultiplier); // Ȧ��: ���ηθ�

            // �� ���� ���������� ������
            transform.localScale = new Vector3(
                initialScale.x * scaleChange.x,
                initialScale.y * scaleChange.y,
                initialScale.z * scaleChange.z
            );
        }
        else
        {
            // ��� �ݺ��� ������ ���� ũ��� ����
            transform.localScale = initialScale;
        }
    }
}
