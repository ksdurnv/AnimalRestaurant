using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoneyPile))]
public class MoneyPileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MoneyPile moneyPile = (MoneyPile)target;

        // �⺻ �ν����� �Ӽ� �׸���
        DrawDefaultInspector();

        // ��ư�� ����� Ŭ�� �� EarnMoney ȣ��
        if (GUILayout.Button("�� ����"))
        {
           // moneyPile.EarnMoney();
        }
    }
}
