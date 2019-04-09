using UnityEngine;
using System.Collections;

public class Root : MonoBehaviour
{
    void Start()
    {
        PanelMgr.instance.OpenPanel<TitlePanel>("");
    }
}