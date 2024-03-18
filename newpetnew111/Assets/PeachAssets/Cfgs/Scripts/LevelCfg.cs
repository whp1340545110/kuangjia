//------------------------------------------
//auto-generated
//-------------------------------------------
#pragma warning disable 0649
using System;
using System.Collections.Generic;
using UnityEngine;
using PFramework;
using PFramework.Cfg;

namespace Game.Cfg
{
	[Serializable]
	public partial class LevelCfgEntry : EERowData
	{
		/// <summary>
		///ID
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///任务名字
		/// </summary>
		[SerializeField]
		private string _mLevelName;
		public  string mLevelName { get { return _mLevelName; } }

		/// <summary>
		///任务进度
		/// </summary>
		[SerializeField]
		private int _mLevelMax;
		public  int mLevelMax { get { return _mLevelMax; } }

		/// <summary>
		///循环等级
		/// </summary>
		[SerializeField]
		private int _mLevel;
		public  int mLevel { get { return _mLevel; } }

		/// <summary>
		///任务类型
		/// </summary>
		[SerializeField]
		private int _mLevelType;
		public  int mLevelType { get { return _mLevelType; } }


		public LevelCfgEntry()
		{
		}

#if UNITY_EDITOR
		public LevelCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			_mLevelName = sheet[row][column++] ?? "";
			int.TryParse(sheet[row][column++], out _mLevelMax);
			int.TryParse(sheet[row][column++], out _mLevel);
			int.TryParse(sheet[row][column++], out _mLevelType);
		}
#endif
		public void ChangeLocalProperties(LevelCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class LevelCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class LevelCfg : CfgBase
	{
		[SerializeField]
		private List<LevelCfgEntry> _entryList = new List<LevelCfgEntry>();

		public IReadOnlyList<LevelCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as LevelCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public LevelCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as LevelCfgEntry;

		}

		partial void OnCfgLoadCustomized();

		public override void OnCfgLoad()
		{
			if (_entryList == null || _entryList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _entryList.Count; i++)
			{
				_entryList[i].OnEntryLoad();
			}
			OnCfgLoadCustomized();
		}

		public override void SetLanguage(SystemLanguage language)
		{
			var cfgName = GetType().Name+ "_" + language;
			LevelCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as LevelCfgLocalBase;
				so.OnCfgLoad();
				for (int i = 0; i < _entryList.Count; i++)
				{
					 _entryList[i].ChangeLocalProperties(so.GetEntryByIndex(i));
				}
			}
			else
			{
				 PDebug.LogWarning($"Load LocalCfg Asset Error {cfgName}");
			}
		}
	}

	[PreferBinarySerialization]
	[Serializable]
	public abstract class LevelCfgLocalBase : LocalizationCfgBase
	{
		public abstract LevelCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649