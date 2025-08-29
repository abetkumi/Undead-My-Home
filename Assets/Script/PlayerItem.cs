using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    GameObject m_cameraObject;  // ���C���J����
    GameObject m_hitObject;     // �I�𒆂̃I�u�W�F�N�g

    const float SPHERE_RADIUS = 0.8f;           // SphereCast�Ŕ��˂��鋅�̂̔��a
    const float SPHERE_MAX_DISTANCE = 16.0f;    // SphereCast�ŋ��̂𔭎˂��鋗��


    void Awake()
    {
        // ���C���J�������擾����
        m_cameraObject = Camera.main.gameObject;
    }


    void Update()
    {
        // ���̂𔭎˂���
        RaycastHit hit;
        if (Physics.SphereCast(m_cameraObject.transform.position, SPHERE_RADIUS,
            m_cameraObject.transform.forward, out hit, SPHERE_MAX_DISTANCE))
        {
            // �����Ă���I�u�W�F�N�g�ƈႤ�ꍇ�͑I���I��
            if (m_hitObject != hit.collider.gameObject && m_hitObject != null)
            {
                EneSelect();
            }


            // �Փ˂����I�u�W�F�N�g���擾
            ItemObject itemObject = hit.collider.gameObject.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                // �I�𒆂̏���
                itemObject.StartSelect();
                m_hitObject = hit.collider.gameObject;


                // ���莞�̏���
                if ((Input.GetKeyDown("joystick button 0") || Input.GetKeyDown(KeyCode.Return)))
                {
                    // �A�C�e���ɉ���������
                    itemObject.ItemCheck();
                }


                // �A�C�e���g�p���̏���
                if ((Input.GetKeyDown("joystick button 2") || Input.GetKeyDown(KeyCode.I)))
                {
                    // �A�C�e���ɉ���������
                    itemObject.ItemUse();
                }
            }

        }
        else
        {
            // �ǂ̃I�u�W�F�N�g�ɂ��q�b�g���Ă��Ȃ��̂őI���I��
            if (m_hitObject != null)
            {
                EneSelect();
            }
        }

    }


    // �I���I��
    void EneSelect()
    {
        m_hitObject.GetComponent<ItemObject>().EndSelect();
        m_hitObject = null;
    }
}
