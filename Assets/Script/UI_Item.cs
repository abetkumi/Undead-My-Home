using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Item : MonoBehaviour
{
    GameManager m_gameManager;

    [SerializeField]
    GameObject[] ItemCamara; //UI�p�J����

    [SerializeField]
    TextMeshProUGUI ItemNameText, ItemMessageText;

    [SerializeField]
    Vector3 ItemOffset; //�A�C�e���������W�̕␳�ʁi���ʁj

    //UI�p�ɐ��������A�C�e��
    GameObject[] m_itemObject;

    //�\�����e���X�V����@�A�C�e���C���x���g�����X�V�����^�C�~���O�ŌĂ�
    public void UpdateUI()
    {
        //������
        if (m_gameManager == null)
        {
            m_gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            //�v�f���̓J�����̐��Ɠ���
            m_itemObject = new GameObject[ItemCamara.Length];
        }

        //�ϐ���p�ӂ���
        Vector3 itemPos;
        int itemNo;
        int selectNo = m_gameManager.GetSelectItemNo();

        //�A�C�e�������X�V����
        for (int i = 0; i < ItemCamara.Length; i++)
        {
            //�w��X���b�g�̃A�C�e���ԍ����擾
            itemNo = m_gameManager.GetItemID(selectNo);
            //�A�C�e������������폜
            Destroy(m_itemObject[i]);

            //���O��������̍X�V
            if (selectNo == m_gameManager.GetSelectItemNo())
            {
                //�\���̗L��
                if (itemNo == -1)
                {
                    //�A�C�e�����Ȃ����ߐ������͔�\��
                    ItemNameText.enabled = false;
                    ItemMessageText.enabled = false;

                    //��������̍X�V
                    m_gameManager.GetOperationUI().SetOperation
                        (UI_Operation.Button.enButton_B, "", false);

                }
                else
                {
                    //��������\��
                    ItemNameText.enabled = true;
                    ItemMessageText.enabled = true;

                    //�e�L�X�g���X�V
                    ItemNameText.text = m_gameManager.GetItemData().Items[itemNo].ItemName;
                    ItemMessageText.text = m_gameManager.GetItemData().Items[itemNo].ItemExplanation;

                    //��������̍X�V
                    m_gameManager.GetOperationUI().SetOperation
                        (UI_Operation.Button.enButton_B, "�̂Ă�", true);
                }
            }

            //�Z���N�g�ԍ����X�V
            selectNo++;
            if (selectNo > ItemCamara.Length - 1)
            {
                selectNo = 0;
            }
            if (itemNo == -1)
            {
                //�A�C�e�����Ȃ����߂����ŏI��
                continue;
            }

            //�A�C�e���𐶐�
            //�܂��͍��W�����߂�
            itemPos = ItemCamara[i].transform.position;
            itemPos += ItemOffset;
            itemPos += m_gameManager.GetItemData().Items[itemNo].UI_Offset;

            //����
            GameObject item = Instantiate(m_gameManager.GetItemData().Items[itemNo].ItemPrefab,
                itemPos, Quaternion.identity);

            //�A�C�e�����o���Ă���
            m_itemObject[i] = item;

            //��]
            item.transform.Rotate(m_gameManager.GetItemData().Items[itemNo].UI_Rotation,
                Space.World);
            //�傫������
            item.transform.localScale = m_gameManager.GetItemData().Items[itemNo].UI_Scale;

            //���C���[��UI�p�ɕύX�i���O�Ō���)
            item.layer = LayerMask.NameToLayer("UI_Item");
        }
    }
}
