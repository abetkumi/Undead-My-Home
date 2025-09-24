using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    //���ʉ��Đ��֐�
    //static public OneShotAudioClip PlaySE(AudioClip clip,
    //    GameObject sauceObject = null,
    //    float volume = 1.0f,
    //    float spatialBlend = 0.0f,
    //    float minDistance = 0.0f,
    //    float maxDistance = 0.0f)
    //{
    //    //���ʉ��I�u�W�F�N�g��ݒ�
    //    GameObject oneShotObj = Instantiate((GameObject)Resources.Load("OneShotSE"));

    //    //���W��ݒ�
    //    if (sauceObject != null)
    //    {
    //        oneShotObj.transform.position = sauceObject.transform.position;
    //    }

    //    //�I�[�f�B�I�N���b�v��ݒ�
    //    OneShotAudioClip oneShotAudio = oneShotObj.GetComponent<OneShotAudioClip>();
    //    oneShotAudio.PlaySE(clip, volume,
    //        spatialBlend, minDistance, maxDistance);

    //    return oneShotAudio;
    //}

    //�Q�[���̏��
    public enum GameState
    {
        enGameState_Play,
        enGameState_Clear,
    }
    static GameState m_gameState = GameState.enGameState_Play;

    public void SetGameState(GameState gameState)
    {
        m_gameState = gameState;
    }
    //�ǂ�����ł��Ăяo����֐�
    public static GameState GetGameState()
    {
        return m_gameState;
    }

    //�A�C�e���f�[�^
    [SerializeField] ItemData Item_Data;
    public ItemData GetItemData()
    {
        return Item_Data;
    }

    //�I�𒆂̃A�C�e���ԍ��i�A�C�e�����z��̔ԍ��j
    [SerializeField] int SelectItemNo = 0;
    public int GetSelectItemNo()
    {
        return SelectItemNo;
    }

    //�A�C�e����
    [SerializeField] int[] ItemID;
    //�����ԃX���b�g�̃A�C�e�����擾
    public int GetItemID(int no)
    {
        return ItemID[no];
    }

    //�A�C�e������UI
    [SerializeField]
    UI_Item ItemUI;

    //�A�C�e�����\����UI
    [SerializeField]
    UI_Search SearchUI;
    public UI_Search GetSearchUI()
    {
        return SearchUI;
    }


    //���������UI
    [SerializeField]
    UI_Operation OperationUI;
    public UI_Operation GetOperationUI()
    {
        return OperationUI;
    }

    //���ʉ�
    [SerializeField]
    AudioClip ItemGetSE, ItemHitSE, SelectSE, EnterSE;
    public AudioClip GetHitSE()
    {
        return ItemHitSE;
    }
    public AudioClip GetEnterSE()
    {
        return EnterSE;
    }

    //�A�C�e�����擾����
    //�A�C�e�����ɋ󂫂���������true �Ȃ�������false��Ԃ�
    public bool GetItem(int getItemID)
    {
        int selectID = SelectItemNo;

        //���ԂɃA�C�e�������m�F���Ă����āA�󂢂Ă���ꏊ��ID���i�[
        for (int i = 0; i < ItemID.Length; i++)
        {
            if (ItemID[selectID] == -1)
            {
                //�󂫂�����̂ŃA�C�e��ID���i�[
                ItemID[selectID] = getItemID;
                //���ʉ��Đ�
                //PlaySE(ItemGetSE);
                //UI���X�V
                ItemUI.UpdateUI();

                return true;
            }

            selectID++;

            if (selectID > ItemID.Length - 1)
            {
                //�I�[�o�[�����̂�0�ɖ߂�
                selectID = 0;
            }
        }

        //�󂫂��Ȃ�����
        return false;
    }

    //�����ԃX���b�g�̃A�C�e�����̂Ă�
    void ItemDrop()
    {
        //�A�C�e�������邩�m�F
        if (ItemID[SelectItemNo] == -1)
        {
            Debug.Log("�y�G���[�z" + SelectItemNo + "�ԂɃA�C�e��������܂���I");
            return;
        }

        //�v���C���[�̈ړ��ʂ��擾
        Rigidbody playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        Vector3 velocity = playerRb.velocity;
        velocity.y = 0.0f;

        //�̂Ă�A�C�e���𐶐�
        Vector3 itemPos = Camera.main.transform.position;
        GameObject dropItem = Instantiate(Item_Data.Items[ItemID[SelectItemNo]].ItemPrefab,
            itemPos, Camera.main.transform.rotation);
        //�O���ɔ���
        dropItem.GetComponent<ItemObject>().ItemDrop(velocity);

        //�A�C�e������ID�����Z�b�g
        ItemID[SelectItemNo] = -1;

        //UI���X�V
        ItemUI.UpdateUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        //UI���X�V
        ItemUI.UpdateUI();
        //�X�e�[�g�̍X�V�i�������j
        m_gameState = GameState.enGameState_Play;
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���łȂ��Ȃ璆�f
        if (m_gameState != GameState.enGameState_Play)
        {
            return;
        }

        //B�{�^���Ŏ̂Ă�
        if ((Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.G)))
        {
            ItemDrop();
        }

        //�I���A�C�e���̕ύX
        if ((Input.GetKeyDown("joystick button 4") || Input.GetAxis("Mouse ScrollWheel") > 0))
        {
            SelectItemNo++;
            if (SelectItemNo > ItemID.Length - 1)
            {
                SelectItemNo = 0;
            }
            //UI���X�V
            ItemUI.UpdateUI();
            //���ʉ��Đ�
            //PlaySE(SelectSE);
        }
        if ((Input.GetKeyDown("joystick button 5") || Input.GetAxis("Mouse ScrollWheel") < 0))
        {
            SelectItemNo--;
            if (SelectItemNo < 0)
            {
                SelectItemNo = ItemID.Length - 1;
            }
            //UI���X�V
            ItemUI.UpdateUI();
            //���ʉ��Đ�
            //PlaySE(SelectSE);
        }
    }
}
