using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;


public interface IConfigDataTable
{
	/// <summary>
	/// Get name
	/// </summary>
	string GetName();

	/// <summary>
	/// Begin append load
	/// </summary>
	void BeginLoadAppend();

	/// <summary>
	/// Begin append load
	/// </summary>
	void EndLoadAppend();

	/// <summary>
	/// Load data from string from memory
	/// </summary>
	void LoadFromString(string content);

	/// <summary>
	/// Load data from a text asset
	/// </summary>
	void LoadFromAsset(TextAsset asset);

	/// <summary>
	/// Load data from a text asset path
	/// </summary>
	void LoadFromAssetPath(string assetPath);

	/// <summary>
	/// Clear all data
	/// </summary>
	void Clear();

}

public class GConfigDataTable<TDataRecord> : IConfigDataTable, IEnumerable<TDataRecord> where TDataRecord : class
{
	// Index fields
	public class IndexField<TIndex> : Dictionary<TIndex, object> { };

	// Record list
	public List<TDataRecord> records { get; private set; }

	// Indices lookup
	private Dictionary<string, object> indices;

	// Name
	public string name { get; private set; }

	// Is loaded
	public bool isLoaded { get; private set; }

	// Is empty
	public bool isEmpty { get { return records.Count == 0; } }
	
	// Get num records
	public int count { get { return records.Count; } }

	// Flag to mark append loading
	private bool isAppendLoading = false;

	/// <summary>
	/// Constructor
	/// </summary>
	public GConfigDataTable()
	{
		this.name = GetType().Name;
		records = new List<TDataRecord>();
	}

	/// <summary>
	/// Constructor
	/// </summary>
	public GConfigDataTable(string name)
	{
		this.name = name;
		records = new List<TDataRecord>();
	}

	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnDataLoaded()
	{
	}

	/// <summary>
	/// Get name
	/// </summary>
	public string GetName()
	{
		return name;
	}

	public void BeginLoadAppend()
	{
		isAppendLoading = true;
	}

	public void EndLoadAppend()
	{
		if (isAppendLoading)
		{
			isLoaded = true;
			OnDataLoaded();
			isAppendLoading = false;
		}
	}

	/// <summary>
	/// Load data from string from memory
	/// </summary>
	public void LoadFromString(string content)
	{
		//Debug.LogWarning("load from string ===== " +content);
		if (string.IsNullOrEmpty(content))
			throw new ArgumentException("Content is null or empty");

		if (!isAppendLoading && isLoaded)
			Clear();

		FileHelpers.FileHelperEngine fileEngine = new FileHelpers.FileHelperEngine(typeof(TDataRecord));
		records.AddRange(fileEngine.ReadString(content).Select(r => r as TDataRecord));

		if (!isAppendLoading)
		{
			isLoaded = true;
			OnDataLoaded();
		}
	}
	public bool AppendConfigFromData(string data)
	{
		try
		{
			BeginLoadAppend();
			LoadFromString (data);
			EndLoadAppend();
			return true;
		}
		catch(System.Exception ex) {
			Debug.LogError ("Fail To Parse configSongData:" + ex.Message+"\n"+data);
			return false;
		}
	}

	/// <summary>
	/// Load data from a text asset
	/// </summary>
	public void LoadFromAsset(TextAsset asset)
	{
		if (asset == null)
			throw new ArgumentNullException("Asset data is invalid");

		LoadFromString(asset.text);
	}

	/// <summary>
	/// Load data from a text asset path
	/// </summary>
	public void LoadFromAssetPath(string assetPath)
	{
		if( string.IsNullOrEmpty(assetPath) )
			throw new ArgumentException("Asset path is null or empty");

		TextAsset res = AvGameObjectUtils.LoadTextAsset(assetPath);
		LoadFromAsset(res);
	}

	/// <summary>
	/// Clear all data
	/// </summary>
	public void Clear()
	{
		records.Clear();
		isLoaded = false;
	}

	/// <summary>
	/// Build index from a table field
	/// </summary>
	public void RebuildIndexField<TIndex>(string fieldName)
	{
		if (isEmpty)
			return;

		// Search field name by reflection
		Type recordType = typeof(TDataRecord);

		// Check the record type to find the field you need to indexed
		FieldInfo fieldInfo = recordType.GetField(fieldName);
		if (fieldInfo == null)
			throw new Exception("Field [" + fieldName + "] not found");

		if (indices == null)
			indices = new Dictionary<string, object>();

		// Add new index column object
		IndexField<TIndex> indexField = new IndexField<TIndex>();
		indices[fieldName] = indexField;

		// Build index column field from records
		for (int i = 0; i < records.Count; i++)
		{
			// Get field value
			var fieldValue = (TIndex) fieldInfo.GetValue(records[i]);

			// the value of the index maybe a single record or a list of records that have the same key field
			object indexedValue;
			if (!indexField.TryGetValue(fieldValue, out indexedValue))
				indexField.Add(fieldValue, records[i]);
			else
			{
				// If indexedValue is a list, append data
				if (indexedValue is List<TDataRecord>)
					(indexedValue as List<TDataRecord>).Add(records[i]);
				else
				{
					var listRecords = new List<TDataRecord>();
					listRecords.Add(indexedValue as TDataRecord);
					listRecords.Add(records[i]);
					indexField[fieldValue] = listRecords;
				}
			}

		}
	}

	/// <summary>
	/// Check if the field is indexed
	/// </summary>
	public bool IsFieldIndexed(string fieldName)
	{
		if (indices == null)
			return false;

		return indices.ContainsKey(fieldName);
	}

	/// <summary>
	/// 
	/// </summary>
	private TDataRecord FindRecordByIndex<TIndex>(object _indexField, TIndex value)
	{
		var indexField = _indexField as IndexField<TIndex>;
		if (indexField == null)
			throw new InvalidOperationException("Index type and search key mismatch");

		// Find
		object indexedValue;
		if (!indexField.TryGetValue(value, out indexedValue))
			return null;

		// Get first item in the list
		if (indexedValue is List<TDataRecord>)
			return (indexedValue as List<TDataRecord>).FirstOrDefault() as TDataRecord;

		return indexedValue as TDataRecord;
	}

	/// <summary>
	/// 
	/// </summary>
	private List<TDataRecord> FindRecordsByIndex<TIndex>(object _indexField, TIndex value)
	{
		var indexField = _indexField as IndexField<TIndex>;
		if (indexField == null)
			throw new InvalidOperationException("Index type mismatch");

		// Find
		object indexedValue;
		if (!indexField.TryGetValue(value, out indexedValue))
			return null;

		// Get first item in the list
		if (indexedValue is List<TDataRecord>)
			return indexedValue as List<TDataRecord>;

		return new List<TDataRecord>() { indexedValue as TDataRecord };
	}

	/// <summary>
	/// Find single record of a index
	/// </summary>
	public TDataRecord FindRecordByIndex<TIndex>(string indexName, TIndex value)
	{
		// Do not have index
		object indexField;
		if (indices == null || !indices.TryGetValue(indexName, out indexField))
		{
			if (null != indexName && indexName.Length > 0)
				throw new InvalidOperationException("Index not found 1: " + indexName);
			else
				throw new InvalidOperationException("Index not found 1.");
		}

		return FindRecordByIndex<TIndex>(indexField, value);
	}

	/// <summary>
	/// Find many records of a index
	/// </summary>
	public List<TDataRecord> FindRecordsByIndex<TIndex>(string indexName, TIndex value)
	{
		// Do not have index
		object indexField;
		if (indices == null || !indices.TryGetValue(indexName, out indexField))
		{
			if (null != indexName && indexName.Length > 0)
				throw new InvalidOperationException("Index not found 2" + indexName);
        else
				throw new InvalidOperationException("Index not found 2.");
		}

		return FindRecordsByIndex<TIndex>(indexField, value);
	}
	/*
	//sontt get all record
	public List<TDataRecord> FindAllRecords()
	{
		return records;
	}

	//end sontt
	*/
	#region IEnumerable<TDataRecord> Members

	public IEnumerator<TDataRecord> GetEnumerator()
	{
		return records.GetEnumerator();
	}

	#endregion

	#region IEnumerable Members

	IEnumerator IEnumerable.GetEnumerator()
	{
		return records.GetEnumerator();
	}

	#endregion
}

