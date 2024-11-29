using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentMoney : MonoBehaviour
{
    public int currentMoney = 10000; // �ʱ� �� ����
    public TextMeshProUGUI moneyText; // UI �ؽ�Ʈ�� ������ ����

    public void Start()
    {
        UpdateMoneyText();
    }

    // ���� �߰��ϴ� �޼���
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyText();
    }

    // ���� �Ҹ��ϴ� �޼���
    public void SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyText();
        }
        else
        {
            //Debug.Log("���� �����մϴ�!");
        }
    }

    // UI �ؽ�Ʈ ������Ʈ �޼���
    private void UpdateMoneyText()
    {        
        if (currentMoney >= 1_000_000_000)
        {
            // 1_000_000_000 �̻��� ��� B ������ ǥ��
            float valueInK = currentMoney / 1_000_000_000f;
            moneyText.text = valueInK.ToString("F2") + "B";
        }
        else if (currentMoney >= 1_000_000)
        {
            // 1000000 �̻��� ��� M ������ ǥ��
            float valueInK = currentMoney / 1_000_000f;
            moneyText.text = valueInK.ToString("F2") + "M";
        }
        else if (currentMoney >= 1_000)
        {
            // 1000 �̻��� ��� K ������ ǥ��
            float valueInK = currentMoney / 1_000f;
            moneyText.text = valueInK.ToString("F2") + "K";
        }
        else
        {
            // 1000 �̸��� ��� �Ϲ� ǥ��
            moneyText.text = currentMoney.ToString();
        }
    }
}
