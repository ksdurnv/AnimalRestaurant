using UnityEngine;

public class Money : MonoBehaviour
{
    public void OnMouseDown()
    {
        // �θ� ������Ʈ�� MoneyPile ��ũ��Ʈ�� ������
        MoneyPile moneyPile = GetComponentInParent<MoneyPile>();
        if (moneyPile != null)
        {
            moneyPile.RemoveAllChildren();
        }
    }
}