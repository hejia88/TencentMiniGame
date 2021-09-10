using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum ItemType
{
    ActiveItem,
    PassiveItem
}

public class ItemManager : MonoBehaviour
{
    public List<GameObject> prefab_FreshPointList;
    public List<GameObject> Prefab_ItemList;
    public int MaxRefreshNum;
    public float RefreshCD;

    private float currentTime;

    private List<int> list_FreshPointState;
    private Dictionary<GameObject,int> dict_ItemInScene;
    private List<GameObject> list_ItemInScene;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
        list_FreshPointState = new List<int>();
        list_ItemInScene = new List<GameObject>();
        dict_ItemInScene = new Dictionary<GameObject, int>();

        Debug.Log(string.Format("Substract--{0}", prefab_FreshPointList.Count));
        for (int i=0; i< prefab_FreshPointList.Count; i++)
        {
            //-1表示数据配置有误，prefab是null
            list_FreshPointState.Add(-1);
            if (prefab_FreshPointList[i])
            {
                //0表示该刷新点没有东西
                list_FreshPointState[i] = 0;
            }          
        }
        RefreshPoints();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > RefreshCD)
        {
            currentTime = 0;
            Substract();
            RefreshPoints();
        }
    }

    void RefreshPoints()
    {
        while (dict_ItemInScene.Count < MaxRefreshNum)
        {
            int PointIndex = Random.Range(0, prefab_FreshPointList.Count);
            if (list_FreshPointState[PointIndex] == 0)
            {
                list_FreshPointState[PointIndex] = 1;
                int ItemIndex = Random.Range(0, Prefab_ItemList.Count);
                Transform pointTransform = prefab_FreshPointList[PointIndex].GetComponent<Transform>();
                GameObject go = GameObject.Instantiate(Prefab_ItemList[ItemIndex], pointTransform);
                dict_ItemInScene.Add(go, PointIndex);
                list_ItemInScene.Add(go);
            }
        }
    }
    void Substract()
    {
        for(int i=0;i< list_ItemInScene.Count;i++)
        {
            GameObject go = list_ItemInScene[i];
            int PointIndex = dict_ItemInScene[go];
            list_FreshPointState[PointIndex] = 0;
            dict_ItemInScene.Remove(go);
            GameObject.DestroyImmediate(go);
            Debug.Log(string.Format("Substract{0}", dict_ItemInScene.Count));
        }
        list_ItemInScene.Clear();
    }
}
