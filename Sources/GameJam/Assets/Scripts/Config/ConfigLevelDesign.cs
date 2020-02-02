using UnityEngine;
using System.Collections.Generic;
using System;
using FileHelpers;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreEmptyLines()]
[FileHelpers.IgnoreCommentedLines("//")]
public class ConfigLevelDesignItem
{
	public int id;
	public string type;
	public float time;
	public float duration;
	public int objectId;
	public int line;
	
}

public class ConfigLevelDesign : GConfigDataTable<ConfigLevelDesignItem>
{
	public ConfigLevelDesign()
		: base("ConfigDailyQuest")
	{
	}

	protected override void OnDataLoaded()
	{
		RebuildIndexField<int>("id");
	}

	public ConfigLevelDesignItem GetQuestItemById(int id)
	{
		return FindRecordByIndex<int>("id", id);
	}
}