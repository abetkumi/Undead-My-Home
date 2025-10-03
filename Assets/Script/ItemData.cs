using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�A�C�e���p�f�[�^�x�[�X
[CreateAssetMenu(fileName = "itemDataBase", menuName = "CreateItemDataBase")]
public class ItemData : ScriptableObject
{
    //�A�C�e�����̍\����
    [System.Serializable]
    public struct Item
    {
        public string ItemName; //�A�C�e����
        [Multiline(2)]
        public string ItemExplanation; //�A�C�e�����ŕ\�����������
        public GameObject ItemPrefab;

        //�ݒu�p�p�����[�^
        public Vector3 ItemPivot;   //�A�C�e����u�����Ƃ��ɍ��W��␳�����

        //UI�p�p�����[�^
        public Vector3 UI_Offset;
        public Vector3 UI_Rotation;
        public Vector3 UI_Scale;

        //�d���B(�P�ʂ�kg)
        public float weight;
        //���l�B
        public float value;

    }

    //�A�C�e�����X�g�̉ϒ��z��
    public Item[] Items;
}
