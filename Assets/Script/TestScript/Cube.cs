using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Cube : MonoBehaviour
{
    //�L���[�u�̃v���n�u���w��B
    [SerializeField] GameObject cubePrefab;
    GameObject[] cubes = new GameObject[100];

    //�|�C���g���X�g���w��B
    [SerializeField] GameObject PointList;
    Point[] point = new Point[100];

    public List<GameObject> colorPrefabs;

    //�L���[�u�̌��B
    int cubeNum = 0;
    //�ݒu�����|�C���g�̌��B
    int pointNum = 0;

    Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        SetCube();
        SetPoint();

        PointSelect(cubeNum);
    }

    //�L���[�u�̌��ݒ�B
    void SetCube()
    {
        cubeNum = 2;

        for (int i = 0; i < cubeNum; i++){
            cubes[i] = GameObject.Instantiate(cubePrefab);
        }
    }

    //�|�C���g�̌��ݒ�B
    void SetPoint()
    {
        bool found = false;

        //�|�C���g���X�g�����݂��邩�����B
        parent = GameObject.Find("PointList")?.transform;
        if (parent == null)
        {
            Debug.LogWarning("PointList ��������܂���ł���");
            return;
        }

        while (!found){
            string name = "Point" + (pointNum + 1).ToString("D3");
            Transform obj = parent.Find(name);
            if (obj != null)
            {
                point[pointNum] = new Point();
                pointNum++;
            }
            else found = true;
        }
    }

    //�����_����Prefab�̌����ڂ�ύX�B
    void SetPrefab(int cubeNo)
    {
        if (colorPrefabs.Count == 0){
            Debug.LogWarning("�v���n�u���o�^����Ă܂���I");
            return;
        }

        int index=Random.Range(0, colorPrefabs.Count);
        GameObject obj = Instantiate(colorPrefabs[index], cubes[cubeNo].transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(20f, 20f, 20f);
        cubes[cubeNo] = obj;
    }

    //�|�C���g��I���B(�����ɂ͐ݒu����cube�̐�����)
    void PointSelect(int count)
    {
        //���X�g���̖��g�p�̃|�C���g�������B
        for (int i = 0; i < count; i++){
            bool found = false;

            while (!found){
                int pointNo = Random.Range(1, pointNum + 1);

                if (!point[pointNo - 1].GetUes()){
                    string name = "Point" + pointNo.ToString("D3");
                    Transform obj = parent.Find(name);

                    if (obj != null)
                    {
                        SetPrefab(i);
                        cubes[i].transform.position = obj.transform.position;
                        point[pointNo - 1].SetUesTrue();
                        found = true;
                    }
                }
                else{}
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
