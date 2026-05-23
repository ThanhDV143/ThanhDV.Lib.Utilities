using System.Collections;
using System.Collections.Generic;
using ThanhDV.Utilities;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] private WeightedRandomList<string> _weightedRandomList;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(_weightedRandomList.Random());
        }
    }
}
