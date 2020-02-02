using UnityEngine;
using Gamejam.Utils;

public class ConfigService 
{
	public static TConfigTable LoadDataConfig<TConfigTable>(params string[] dataPaths) where TConfigTable : IConfigDataTable, new()
	{
		TConfigTable configTable = new TConfigTable();
		try
		{
			configTable.BeginLoadAppend();
			foreach (var path in dataPaths)
			{
				configTable.LoadFromAssetPath(path);
			}
			configTable.EndLoadAppend();
			
			return configTable;

		}
		catch (System.Exception ex)
		{
			Debug.LogError(configTable.GetName()+","+ex.Message);
		}

		return configTable;
	}

	
	public static ConfigLevelDesign LoadLevelDesign(string data)
	{
		try
		{
			ConfigLevelDesign configLevel = new ConfigLevelDesign ();
			configLevel.BeginLoadAppend();
			configLevel.LoadFromString (data);
			configLevel.EndLoadAppend();
			return configLevel;
		}
		catch(System.Exception ex) {
			Debug.LogError ("Parse Congfig Error:"+ex.Message);
			return null;
		}
	}

}
 