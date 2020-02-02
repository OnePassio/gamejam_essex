using UnityEngine;

// Code that runs in development build only
class Development
{
	private static bool isActiveLog=true;
	public static void SetActiveLog(bool isActive){
		isActiveLog=isActive;
	}
	public static void Log(string s,LogType type= LogType.Log)
	{
		if(isActiveLog)
		{
			switch(type){
				case  LogType.Log:
					UnityEngine.Debug.Log(s);
				break;
				case  LogType.Warning:
					UnityEngine.Debug.LogWarning(s);
				break;
				case  LogType.Assert:
					UnityEngine.Debug.LogAssertion(s);
				break;
				case  LogType.Error:
					UnityEngine.Debug.LogError(s);
				break;
				default:
					UnityEngine.Debug.LogError(s);
				break;
			}
			
		}
	}
}
