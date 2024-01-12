using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{      
    public static GameManager instance;
    public bool isAccelerating;

    public bool getIfAccelerating() {
        return isAccelerating;
    }

    public void setIfAccelerating(bool b) {
        isAccelerating = b;
    }

    // Start is called before the first frame update
    void Start()
    {
        isAccelerating = false;
        // 인스턴스가 null인 경우 현재 객체를 할당하고, 이미 존재하는 경우 현재 객체를 파괴
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
