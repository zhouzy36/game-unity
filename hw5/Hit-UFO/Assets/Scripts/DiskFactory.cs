using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DiskFactory : MonoBehaviour
{
    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public GameObject diskPrefab;
    private List<DiskData> used = new List<DiskData>();
    private List<DiskData> free = new List<DiskData>();

    //游戏的一些参数
    private const float speed = 10;
    private const float factor1 = 0.15f;
    private const float factor2 = 0.02f;
    private const float factor3 = 0.04f;

    //单例模式
    private static DiskFactory factory; 

    public static DiskFactory getInstance()
    {
        if (factory == null)
        {
            factory = new DiskFactory();
        }
        return factory;
    }

    //还需要设计一个规则
    public GameObject GetDisk(int round)
    {
        Assert.IsTrue(round > 0 && round <= 10);
        diskPrefab = null;

        int colorSelector;
        if (round <= 3) colorSelector = Random.Range(0, round);
        else colorSelector = Random.Range(0,3);
        //Debug.Log(colorSelector);

        //如果有空闲的飞碟就直接拿来使用，没有的话就实例化新的飞碟
        int x = Random.Range(0, 2) > 0.5 ? 20 : -20;
        if (free.Count > 0)
        {
            diskPrefab = free[0].gameObject;
            diskPrefab.transform.position = new Vector3(x, 0, 0);
            //将该飞碟从free列表中删除
            free.Remove(free[0]);
        }
        else
        { 
            diskPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk"), new Vector3(x, 0, 0), Quaternion.identity);
            diskPrefab.AddComponent<DiskData>();
        }

        diskPrefab.GetComponent<DiskData>().size = diskPrefab.transform.localScale;
        //根据round设置速度
        diskPrefab.GetComponent<DiskData>().speed = speed * Mathf.Pow(1 + factor1, round);

        //根据round设置发射角度
        float angle = Random.Range(Mathf.PI / 12 - factor2 * round, Mathf.PI / 4 - factor3 * round);
        angle = diskPrefab.transform.position.x < 0 ? angle : (Mathf.PI - angle); 
        diskPrefab.GetComponent<DiskData>().direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        //根据colorSelector给飞碟上色，并为其指定分数
        switch (colorSelector)
        {
            case (int)Color.Red:
                diskPrefab.GetComponent<DiskData>().score = 1;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.red;
                break;
            case (int)Color.Green:
                diskPrefab.GetComponent<DiskData>().score = 2;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.green;
                break;
            case (int)Color.Blue:
                diskPrefab.GetComponent<DiskData>().score = 5;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.blue;
                break;
            default:
                diskPrefab.GetComponent<DiskData>().score = 1;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.red;
                break;
        }
        diskPrefab.GetComponent<DiskData>().color = diskPrefab.GetComponent<Renderer>().material.color;

        diskPrefab.SetActive(true);
        //将该飞碟添加到used列表中
        used.Add(diskPrefab.GetComponent<DiskData>());
        return diskPrefab;
    }

    public void FreeDisk(GameObject disk)
    {
        for (int  i = 0; i < used.Count; i++)
        {
            if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID())
            {
                used[i].gameObject.SetActive(false);
                free.Add(used[i]);
                used.Remove(used[i]);
                break;
            }
        }
    }
}
