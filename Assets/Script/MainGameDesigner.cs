using UnityEngine;

public class MainGameDesigner : MonoBehaviour
{
    public static MainGameDesigner instance;
    void Awake()
    {
        instance=this;
    }
    public PlayerDataScriptable data;
}
