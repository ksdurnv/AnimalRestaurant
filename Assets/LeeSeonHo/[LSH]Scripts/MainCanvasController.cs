using UnityEngine;

public class MainCanvasController : MonoBehaviour
{
    private static MainCanvasController instance; // ***** �ߺ� ������ ���� static ����

    private void Awake()
    {
        // ***** �ߺ��� mainCanvas�� �������� �ʵ��� ����
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // ***** �� ��ȯ �� ������Ʈ ����
    }
}
