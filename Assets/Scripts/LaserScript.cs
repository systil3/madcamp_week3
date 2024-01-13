using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public Vector3 laserStartPoint = Vector3.zero;
	public Vector3 laserEndPoint = Vector3.zero;
    LineRenderer laserLine;

    // Start is called before the first frame update
    void Start()
    {
        //레이저(라인렌더러) 설정
        laserLine = GetComponent<LineRenderer> ();
        laserLine.SetWidth(0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        laserLine.SetPosition(0, laserStartPoint);
        laserLine.SetPosition(1, laserEndPoint);
    }
}
