using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIroute : MonoBehaviour
{
    public GameObject target;  //获取目标点
    NavMeshAgent agent;   //声明变量

    void Start()
    {
        //获取自身的NavMeshAgent组件
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //设置目标
        agent.SetDestination(target.transform.position);
    }
}
