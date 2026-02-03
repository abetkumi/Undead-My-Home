using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���o�[��N���b�N���ĕ����̓S�B�̔��J����V�X�e��
/// </summary>
public class LeverDoorSystem : MonoBehaviour
{
    [Header("��̐ݒ�")]
    [Tooltip("�J������Transform�z��")]
    public Transform[] doors;

    [Tooltip("����J�������̍���")]
    public float openHeight = 3.5f;

    [Tooltip("��̊J���x")]
    public float doorSpeed = 2f;

    [Tooltip("�����ԂɊJ���x�����ԁi0�œ����j")]
    public float doorDelay = 0.2f;

    [Header("���o�[�̐ݒ�")]
    [Tooltip("���o�[��Transform�i��]���镔���j")]
    public Transform lever;

    [Tooltip("���o�[����������̊p�x")]
    public float leverDownAngle = 60f;

    [Tooltip("���o�[�̉�]���x")]
    public float leverSpeed = 3f;

    [Header("���")]
    public bool isDoorOpen = false;

    [Header("�f�o�b�O")]
    [Tooltip("�f�o�b�O���O���")]
    public bool showDebugLog = true;

    [Header("�n�C���C�g�ݒ�")]
    [Tooltip("�z�o�[���̃n�C���C�g�J���[")]
    public Color highlightColor = Color.red;

    [Tooltip("�n�C���C�g�̑���")]
    public float outlineWidth = 0.05f;

    private Vector3[] doorClosedPositions;
    private Vector3[] doorOpenPositions;
    private float[] doorAnimationStartTimes;
    private Quaternion leverUpRotation;
    private Quaternion leverDownRotation;
    private bool isAnimating = false;
    private float animationStartTime;
    private bool isHoveringLever = false;
    private Renderer[] leverRenderers;
    private Material[][] originalMaterials;

    [SerializeField] AudioClip m_leverSE;
    [SerializeField] private EnemyGenerator generator;

    void Start()
    {
        if (showDebugLog)
        {
            Debug.Log("=== LeverDoorSystem �������J�n ===");
        }

        // �e��̏����ʒu��ۑ�
        if (doors != null && doors.Length > 0)
        {
            doorClosedPositions = new Vector3[doors.Length];
            doorOpenPositions = new Vector3[doors.Length];
            doorAnimationStartTimes = new float[doors.Length];

            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] != null)
                {
                    doorClosedPositions[i] = doors[i].localPosition;
                    doorOpenPositions[i] = doorClosedPositions[i] + new Vector3(0, openHeight, 0);

                    if (showDebugLog)
                    {
                        Debug.Log($"�� {i}: �����ʒu = {doorClosedPositions[i]}, �J�����ʒu = {doorOpenPositions[i]}");
                    }
                }
                else
                {
                    Debug.LogWarning($"�� {i} ���ݒ肳��Ă��܂���I");
                }
            }
        }
        else
        {
            Debug.LogError("���1��ݒ肳��Ă��܂���IInspector�Ŕ��ݒ肵�Ă��������B");
        }

        // ���o�[�̏�����]��ۑ�
        if (lever != null)
        {
            leverUpRotation = lever.localRotation;
            leverDownRotation = Quaternion.Euler(leverDownAngle, 0, 0) * leverUpRotation;

            if (showDebugLog)
            {
                Debug.Log($"���o�[�ݒ芮��: ��={leverUpRotation.eulerAngles}, ��={leverDownRotation.eulerAngles}");
            }

            // ���o�[��Renderer��擾���ă}�e���A����ۑ�
            leverRenderers = lever.GetComponentsInChildren<Renderer>();
            if (leverRenderers.Length > 0)
            {
                originalMaterials = new Material[leverRenderers.Length][];
                for (int i = 0; i < leverRenderers.Length; i++)
                {
                    originalMaterials[i] = leverRenderers[i].materials;
                }
            }
        }
        else
        {
            Debug.LogError("���o�[���ݒ肳��Ă��܂���IInspector�Ń��o�[��ݒ肵�Ă��������B");
        }

        if (showDebugLog)
        {
            Debug.Log("=== LeverDoorSystem ���������� ===");
        }
    }

    void Update()
    {
        // �}�E�X�z�o�[���o
        CheckLeverHover();

        // ���o�[�̃N���b�N���o�i���N���b�N�̂݁j
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Action"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (showDebugLog)
            {
                Debug.Log("���N���b�N�����o����܂���");
            }

            if (Physics.Raycast(ray, out hit))
            {
                if (showDebugLog)
                {
                    Debug.Log($"�q�b�g: {hit.transform.name}");
                }

                // ���o�[�܂��͂��̎q�I�u�W�F�N�g��N���b�N�����ꍇ
                if (hit.transform == lever || hit.transform.IsChildOf(lever))
                {
                    if (showDebugLog)
                    {
                        Debug.Log("���o�[���N���b�N����܂����I");
                    }

                    // ���o�[����̏�Ԃ̎��̂݉�������
                    if (!isDoorOpen)
                    {
                        if (showDebugLog)
                        {
                            Debug.Log("���J���܂�");
                        }
                        ToggleDoors();
                        PullLever();
                    }
                    else
                    {
                        if (showDebugLog)
                        {
                            Debug.Log("��͊��ɊJ���Ă��܂�");
                        }
                    }
                }
            }
            else
            {
                if (showDebugLog)
                {
                    Debug.Log("����q�b�g���܂���ł���");
                }
            }
        }

        // ��ƃ��o�[�̃A�j���[�V����
        if (isAnimating)
        {
            AnimateDoorsAndLever();
        }
    }

    /// <summary>
    /// �}�E�X�z�o�[���̃��o�[�n�C���C�g����
    /// </summary>
    private void CheckLeverHover()
    {
        if (lever == null || isDoorOpen) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool isHovering = false;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == lever || hit.transform.IsChildOf(lever))
            {
                isHovering = true;
            }
        }

        // �z�o�[��Ԃ��ω������ꍇ
        if (isHovering != isHoveringLever)
        {
            isHoveringLever = isHovering;

            if (isHovering)
            {
                ApplyHighlight();
            }
            else
            {
                RemoveHighlight();
            }
        }
    }

    /// <summary>
    /// ���o�[�Ƀn�C���C�g��K�p
    /// </summary>
    private void ApplyHighlight()
    {
        if (leverRenderers == null) return;

        foreach (Renderer renderer in leverRenderers)
        {
            Material[] mats = renderer.materials;
            foreach (Material mat in mats)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", highlightColor * 0.5f);
            }
        }

        // �J�[�\����ύX
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// ���o�[����n�C���C�g�����
    /// </summary>
    private void RemoveHighlight()
    {
        if (leverRenderers == null || originalMaterials == null) return;

        for (int i = 0; i < leverRenderers.Length; i++)
        {
            if (originalMaterials[i] != null)
            {
                foreach (Material mat in leverRenderers[i].materials)
                {
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    /// <summary>
    /// ���ׂĂ̔�̊J��؂�ւ���
    /// </summary>
    public void ToggleDoors()
    {
        isDoorOpen = !isDoorOpen;
        isAnimating = true;
        animationStartTime = Time.time;

        GameManager.PlaySE(m_leverSE);

        if (showDebugLog)
        {
            Debug.Log($"=== ToggleDoors ���s isDoorOpen={isDoorOpen} ===");
        }

        // �e��̃A�j���[�V�����J�n���Ԃ�ݒ�
        for (int i = 0; i < doorAnimationStartTimes.Length; i++)
        {
            doorAnimationStartTimes[i] = animationStartTime + (i * doorDelay);

            if (showDebugLog)
            {
                Debug.Log($"�� {i} �A�j���[�V�����J�n����: {doorAnimationStartTimes[i]}");
            }
        }
    }

    /// <summary>
    /// ��ƃ��o�[��A�j���[�V����������
    /// </summary>
    private void AnimateDoorsAndLever()
    {
        bool allDoorsReached = true;
        bool leverReached = true;

        // �e��̃A�j���[�V����
        if (doors != null)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] == null) continue;

                // ��̃A�j���[�V�����J�n���ԂɒB���Ă��邩�m�F
                if (Time.time >= doorAnimationStartTimes[i])
                {
                    Vector3 targetPosition = isDoorOpen ? doorOpenPositions[i] : doorClosedPositions[i];
                    Vector3 oldPosition = doors[i].localPosition;
                    doors[i].localPosition = Vector3.Lerp(doors[i].localPosition, targetPosition, Time.deltaTime * doorSpeed);

                    if (showDebugLog && i == 0) // �ŏ��̔�̂݃��O�o��
                    {
                        Debug.Log($"�� {i}: ���݈ʒu={doors[i].localPosition}, �ڕW�ʒu={targetPosition}, ����={Vector3.Distance(doors[i].localPosition, targetPosition)}");
                    }

                    if (Vector3.Distance(doors[i].localPosition, targetPosition) > 0.01f)
                    {
                        allDoorsReached = false;
                    }
                    else
                    {
                        doors[i].localPosition = targetPosition;

                        if (showDebugLog && oldPosition != targetPosition)
                        {
                            Debug.Log($"�� {i} ���ڕW�ʒu�ɓ��B���܂���: {targetPosition}");
                        }
                    }
                }
                else
                {
                    allDoorsReached = false;
                }
            }
        }

        // ���o�[�̃A�j���[�V����
        if (lever != null)
        {
            Quaternion targetRotation = isDoorOpen ? leverDownRotation : leverUpRotation;
            lever.localRotation = Quaternion.Lerp(lever.localRotation, targetRotation, Time.deltaTime * leverSpeed);

            if (Quaternion.Angle(lever.localRotation, targetRotation) > 0.1f)
            {
                leverReached = false;
            }
            else
            {
                lever.localRotation = targetRotation;
            }
        }

        // �A�j���[�V���������`�F�b�N
        if (allDoorsReached && leverReached)
        {
            isAnimating = false;

            if (showDebugLog)
            {
                Debug.Log("=== �A�j���[�V�������� ===");
            }
        }
    }

    /// <summary>
    /// ���ׂĂ̔��J���i�O������Ăяo���\�j
    /// </summary>
    public void OpenDoors()
    {
        if (!isDoorOpen)
        {
            ToggleDoors();
        }
    }

    /// <summary>
    /// ���ׂĂ̔�����i�O������Ăяo���\�j
    /// </summary>
    public void CloseDoors()
    {
        if (isDoorOpen)
        {
            ToggleDoors();
        }
    }

    /// <summary>
    /// ����̔�݂̂�J�i�C���f�b�N�X�w��j
    /// </summary>
    public void ToggleSpecificDoor(int doorIndex)
    {
        if (doors != null && doorIndex >= 0 && doorIndex < doors.Length && doors[doorIndex] != null)
        {
            // �ʔ����̎����i�K�v�ɉ����Ċg���j
            Debug.Log($"�� {doorIndex} ��؂�ւ��܂���");
        }
    }

    public void PullLever() { generator.SpawnEnemyStart(); }
}