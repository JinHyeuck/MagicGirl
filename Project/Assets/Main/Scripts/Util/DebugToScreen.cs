using System.Collections;
using UnityEngine;

public class DebugToScreen : MonoBehaviour
{
    string myLog;
    Queue myLogQueue = new Queue();
    GUIStyle myStyle = new GUIStyle();

    bool onload = false;

    public void Init()
    {
        onload = true;
        myStyle.fontSize = 20;
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        if (onload == true)
            Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (myLogQueue.Count > 20)
            myLogQueue.Dequeue();
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }

    void OnGUI()
    {
        GUILayout.Label(myLog, myStyle);
    }
}