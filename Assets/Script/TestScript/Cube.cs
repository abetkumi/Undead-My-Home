using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    //�L���[�u�̃v���n�u���w��B
    [SerializeField] GameObject cubePrefab;
    GameObject[] cubes = new GameObject[100];

    //�|�C���g���X�g���w��B
    [SerializeField] GameObject PointList;
    Point[] point = new Point[100];

    //�L���[�u�̌��B
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetCount();

        for(int i = 1; i <= count; i++){
            cubes[i] = GameObject.Instantiate(cubePrefab);
            point[i] = PointList.GetComponent<Point>();
        }
    }

    //�L���[�u�̌��ݒ�B
    void SetCount()
    {
        count = 3;
    }

    //�|�C���g��I���B(�����ɂ͐ݒu����cube�̐�����)
    void PointSelect(int count)
    {
        //�|�C���g���X�g�����݂��邩�����B
        Transform parent = GameObject.Find("PointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("PointList ��������܂���ł���");
            return;
        }

        //���X�g���̖��g�p�̃|�C���g�������B
        for (int i = 1; i <= count; i++){
            cubes[i] = GameObject.Instantiate(cubePrefab);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
