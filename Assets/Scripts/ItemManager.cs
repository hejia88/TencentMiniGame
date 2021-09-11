using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ItemManager : MonoBehaviour
{
    public List<GameObject> prefab_FreshPointList;
    public List<GameObject> Prefab_ItemList;
    public int MaxRefreshNum;
    public float RefreshCD;

    private float currentTime;

    private List<int> list_FreshPointState;
    private Dictionary<GameObject,int> dict_ItemInScene;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
        list_FreshPointState = new List<int>();
        dict_ItemInScene = new Dictionary<GameObject, int>();

        Debug.Log(string.Format("Start--{0}", prefab_FreshPointList.Count));
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
                //将item的管理接口挂过去方便销毁时调用接口
                ItemBase item = go.GetComponent<ItemBase>();
                item.Manager_Item = this;

                dict_ItemInScene.Add(go, PointIndex);
            }
        }
    }

    public void ItemDestroy(GameObject go)
    {
        int PointIndex = dict_ItemInScene[go];
        list_FreshPointState[PointIndex] = 0;
        dict_ItemInScene.Remove(go);
        GameObject.Destroy(go);
        Debug.Log(string.Format("ItemDestroy{0}", dict_ItemInScene.Count));
    }
}
