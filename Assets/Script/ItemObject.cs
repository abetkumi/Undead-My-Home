using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class ItemObject : MonoBehaviour
{
    [SerializeField, Tooltip("-1�͒��ׂ���A�C�e��0�ȏ�͎擾�ł���A�C�e��")]
    int ItemID = -1;    //�A�C�e�����ʗp�ԍ�
    [SerializeField] ItemData itemDataBase; //�A�C�e�����X�g

    [SerializeField] string Name; //�J�[�\�������킹���Ƃ��ɕ\�����閼�O
    [SerializeField, Multiline(3)] protected string Explanation; //���ׂ��Ƃ��ɕ\�����閼�O

    Outline m_outline;
    const float SELECT_OUTLINE_WIDTH = 16.0f;

    const float ITEMDROP_POWER = 20.0f;     //�A�C�e�����̂Ă鎞�ɂ������
    const float ITEMDROP_TORQUE = 60.0f;    //�A�C�e�����̂Ă鎞�ɂ������]��

    [SerializeField] bool IsCheck = false;

    //�Q�[���}�l�[�W���[
    protected GameManager m_gameManager;

    public void SetIsCheck(bool flag)
    {
        IsCheck = flag;
    }



    //GameObject m_platformObj = null;  //�������u����Ă���

    private void Awake()
    {
        //�A�E�g���C���̏�����
        m_outline = GetComponent<Outline>(); 
        m_outline.OutlineMode = Outline.Mode.OutlineVisible;
        m_outline.OutlineColor = Color.red;
        m_outline.OutlineWidth = SELECT_OUTLINE_WIDTH;
        m_outline.enabled = false;
        //�Q�[���}�l�[�W���[���擾
        //�y�q���g1�zGameManager�̃^�O���́uGameController�v�ł�
        //�y�q���g2�z�R���|�[�l���g���擾����֐���GetComponent�ł��B
        m_gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    //�A�C�e���𒲂ׂ����̉��z�֐�
    public virtual void ItemCheck()
    {
        //���ׂ����̊�{����
        ItemGet();
    }

    //�A�C�e���𒲂ׂĎ��̊�{����
    public void ItemGet()
    {
        //���ׂ��Ȃ����
        if (IsCheck)
        {
            return;
        }

        if (ItemID == -1)
        {
            //�A�C�e���𒲂ׂ�

            //�f�o�b�O�p�A�C�e�����Ɛ��������R���\�[���ɏo��
            //Debug/Log("�A�C�e����:"+Name+"\n������:"+Explanation);

            //�l���ł��Ȃ��A�C�e���Ȃ̂Ő�����\��
            m_gameManager.GetSearchUI().SearchUI_On(Explanation, true);
            m_gameManager.GetSearchUI().AutoOff();

            //���ʉ����Đ�
            //GameManager.PlaySE(m_gameManager.GetEnterSE());
        }
        else
        {
            //�A�C�e�����擾
            //�y�q���g�z�Q�[���}�l�[�W���[��GetItem�֐����g����
            bool isGet = m_gameManager.GetItem(ItemID);

            //�A�C�e�����ɋ󂫂����������ǂ����ŕ���
            if (isGet)
            {
                //�A�C�e�����l���ł���

                //�f�o�b�O�p �l�������A�C�e�������R���\�[���ɏo��
                Debug.Log(itemDataBase.Items[ItemID].ItemName + "���擾");

                //�A�C�e�������폜����
                //m_gameManager.GetSearchUI().SearchUI_Off();

                //A�{�^���\�����Â�����
                //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
                //    "", false);

                //���g���폜����
                Destroy(gameObject);
            }
            else
            {
                //�A�C�e�����������ς�������

                //�f�o�b�O�p
                Debug.Log("�A�C�e�����������ς��ł�");

                //�A�C�e���������ς��Ȃ̂ŏE���Ȃ�
                //m_gameManager.GetSearchUI().SearchUI_On("����ȏ�A�C�e�������Ă܂���I", true);
                //m_gameManager.GetSearchUI().AutoOff();

                //���ʉ��Đ�
                //GameManager.PlaySE(m_gameManager.GetEnterSE());
            }
        }
    }

    //�A�C�e�����̂Ă����̏���
    public void ItemDrop(Vector3 playerVelocity)
    {
        //���W�b�h�{�f�B�̎擾
        Rigidbody rb = GetComponent<Rigidbody>();
        //�������Z��L���ɂ���i�H�H�H�H�H�j
        rb.isKinematic = false;
        //�J�����̑O�������ɔ�΂�
        rb.AddForce((Camera.main.transform.forward * ITEMDROP_POWER) + playerVelocity, ForceMode.Impulse);
        //�����_���ɉ�]
        rb.AddTorque(Random.onUnitSphere * Random.Range(-ITEMDROP_TORQUE, ITEMDROP_TORQUE));

        //���΂炭���ׂ��Ȃ��悤�ɂ���
        IsCheck = true;
        //1�b��ɒ��ׂ���悤�ɂ���
        Invoke("CheckWait", 1.0f);
    }

    //Invole�ŌĂяo���֐�
    void CheckWait()
    {
        IsCheck = false;
    }

    //�A�C�e�����g�p�������̉��z�֐�
    public virtual void ItemUse()
    {

    }

    //�����ɃJ�[�\������������
    public virtual void StartSelect()
    {
        //���ׂ��Ȃ����
        if (IsCheck)
        {
            return;
        }

        //�A�E�g���C����\������
        m_outline.enabled = true;

        //�A�C�e������\��
        string name;
        if (ItemID == -1)
        {
            //���ׂ�n�̃A�C�e���Ȃ̂Ŏw�肵�����O���g��
            name = Name;
            //��������̍X�V
            //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
            //    "���ׂ�", true);
        }
        else
        {
            //�l���ł���A�C�e���Ȃ̂ŃA�C�e���f�[�^�x�[�X���疼�O�����������Ă���
            name = m_gameManager.GetItemData().Items[ItemID].ItemName;
            //��������̍X�V
            //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
            //    "�E��", true);
        }
        //m_gameManager.GetSearchUI().SearchUI_On(name, false);
    }

    //�������J�[�\������O�ꂽ��
    public virtual void EndSelect()
    {
        //�A�E�g���C�����폜����
        m_outline.enabled = false;

        //�A�C�e�������\���ɂ���
        //m_gameManager.GetSearchUI().SearchUI_Off();

        //��������̍X�V
        //m_gameManager.GetOperationUI().SetOperation(UI_Operation.Button.enButton_A,
        //     "", false);
    }

    //�����ɉ������Փ˂����u��
    private void OnCollisionEnter(Collision collision)
    {
        //�������A�C�e���o�Ȃ��Ȃ璆�f
        if (gameObject.CompareTag("Item") == false)
        {
            return;
        }

        //�d�͓K�����̂ݏՓˉ����Đ�
        if (GetComponent<Rigidbody>().isKinematic == false)
        {
            //GameManager.PlaySE(m_gameManager.GetHitSE(),
            //    gameObject,
            //    1.0f, 1.0f);
        }
    }
}
