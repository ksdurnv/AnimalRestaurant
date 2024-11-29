using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // ***** Additive ���� �� �ε� (�ߺ� �ε� ����)
    public void LoadSceneAdditive(string sceneName)
    {
        if (IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"�� '{sceneName}'�� �̹� �ε�Ǿ� �ֽ��ϴ�.");
            return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        Debug.Log($"�� '{sceneName}'�� Additive ���� �ε��߽��ϴ�.");
    }

    // ***** �� ��ε� (�̹� ��ε�� ��� ����)
    public void UnloadScene(string sceneName)
    {
        if (!IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"�� '{sceneName}'�� �̹� ��ε�Ǿ����ϴ�.");
            return;
        }

        SceneManager.UnloadSceneAsync(sceneName);
        Debug.Log($"�� '{sceneName}'�� ��ε��߽��ϴ�.");
    }

    // ***** �� �ε� ���� Ȯ�� �޼���
    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true; // ���� �̹� �ε��
            }
        }
        return false; // ���� �ε���� ����
    }
}
