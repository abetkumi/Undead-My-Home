using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] Transform lid;
    [SerializeField] float m_speed = 2f;
    [SerializeField] AudioClip m_openSE;
    private const float m_openAngle = -100f;

    bool isOpening = false;

    void Update()
    {
        if (!isOpening)
            CheckRay();

        if (isOpening)
        {
            float currentX = lid.localEulerAngles.x;

            if (currentX > 180f) currentX -= 360f;

            float nextX = Mathf.Lerp(currentX, m_openAngle, Time.deltaTime * m_speed);

            Vector3 rot = lid.localEulerAngles;
            rot.x = nextX;
            lid.localEulerAngles = rot;
        }
    }

    void CheckRay()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Action"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    isOpening = true;
                    GameManager.PlaySE(m_openSE);
                }
            }
        }
    }
}
