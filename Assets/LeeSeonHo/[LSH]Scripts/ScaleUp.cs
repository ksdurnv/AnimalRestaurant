using UnityEngine;
using System.Collections;

public class ScaleUp : MonoBehaviour
{
    public float duration = 0.3f; // �ִϸ��̼� ���� �ð�
    public float bounceIntensity = 0.3f; // ���� ����

    public bool isTable;

    private Vector3 initialScale; // �ʱ� ������
    private int bounceCount = 2; // �ݺ� Ƚ��

    private SaveLoadManager saveLoadManager;

    /*private void OnEnable()
    {
        if (GetComponent<Table>() != null)
        {
            isTable = true;
        }
        // �ʱⰪ ����
        initialScale = transform.localScale; // ���� �������� �ʱ� �����Ϸ� ����
       // saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();

        Bounce();
    }*/
    public void ObjectEnabled(bool load)
    {
        if (GetComponent<Table>() != null)
        {
            isTable = true;
        }
        // �ʱⰪ ����
        initialScale = transform.localScale; // ���� �������� �ʱ� �����Ϸ� ����
                                             // saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();

       if(!load) Bounce();
    }

    public void Bounce()
    {
     //   if(saveLoadManager.isLoading == true)
        {
    //        return;
        }
      //  else
        {
            if (isTable)
            {
                StartCoroutine(TableBounceCoroutine());
            }
            else
            {
                StartCoroutine(BounceCoroutine());
            }
        }        
    }


    private IEnumerator BounceCoroutine()
    {
        int currentBounce = 0;
        float currentBounceIntensity = bounceIntensity;

        while (currentBounce < bounceCount)
        {
            float elapsedTime = 0f;

            // �� ���� Bounce�� ����� ������ ����
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // Ȧ����°: ���ηθ� Ŀ��, ¦����°: ���ηθ� Ŀ��
                float scaleMultiplier = 1 + Mathf.Sin(t * Mathf.PI) * currentBounceIntensity;

                Vector3 scaleChange = currentBounce % 2 == 0
                    ? new Vector3(scaleMultiplier, 1, 1) // ¦��: ���ηθ�
                    : new Vector3(1, scaleMultiplier, 1); // Ȧ��: ���ηθ�

                transform.localScale = new Vector3(
                    initialScale.x * scaleChange.x,
                    initialScale.y * scaleChange.y,
                    initialScale.z * scaleChange.z
                );

                yield return null; // �� ������ ���
            }

            // Bounce�� ���� �� ���� ���� ����
            currentBounceIntensity *= 0.5f;
            currentBounce++;
        }

        // ��� �ݺ��� ������ ���� ũ��� ����
        transform.localScale = initialScale;
    }

    private IEnumerator TableBounceCoroutine()
    {
        int currentBounce = 0;
        float currentBounceIntensity = bounceIntensity;

        while (currentBounce < bounceCount)
        {
            float elapsedTime = 0f;

            // �� ���� Bounce�� ����� ������ ����
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // Ȧ����°: ���ηθ� Ŀ��, ¦����°: ���ηθ� Ŀ��
                float scaleMultiplier = 1 + Mathf.Sin(t * Mathf.PI) * currentBounceIntensity;

                Vector3 scaleChange = currentBounce % 2 == 0
                    ? new Vector3(scaleMultiplier, 1, 1) // ¦��: ���ηθ�
                    : new Vector3(1, 1, scaleMultiplier); // Ȧ��: ���ηθ�

                transform.localScale = new Vector3(
                    initialScale.x * scaleChange.x,
                    initialScale.y * scaleChange.y,
                    initialScale.z * scaleChange.z
                );

                yield return null; // �� ������ ���
            }

            // Bounce�� ���� �� ���� ���� ����
            currentBounceIntensity *= 0.5f;
            currentBounce++;
        }

        // ��� �ݺ��� ������ ���� ũ��� ����
        transform.localScale = initialScale;
    }
}
