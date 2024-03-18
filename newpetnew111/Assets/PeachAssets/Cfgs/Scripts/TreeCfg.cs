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
	public partial class TreeCfgEntry : EERowData
	{
		/// <summary>
		///ID
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///lv
		/// </summary>
		[SerializeField]
		private int _mLv;
		public  int mLv { get { return _mLv; } }

		/// <summary>
		///单次点击显示
		/// </summary>
		[SerializeField]
		private int _mCount;
		public  int mCount { get { return _mCount; } }

		/// <summary>
		///升级消耗
		/// </summary>
		[SerializeField]
		private int _mExpend;
		public  int mExpend { get { return _mExpend; } }


		public TreeCfgEntry()
		{
		}

#if UNITY_EDITOR
		public TreeCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			int.TryParse(sheet[row][column++], out _mLv);
			int.TryParse(sheet[row][column++], out _mCount);
			int.TryParse(sheet[row][column++], out _mExpend);
		}
#endif
		public void ChangeLocalProperties(TreeCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class TreeCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class TreeCfg : CfgBase
	{
		[SerializeField]
		private List<TreeCfgEntry> _entryList = new List<TreeCfgEntry>();

		public IReadOnlyList<TreeCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as TreeCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public TreeCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as TreeCfgEntry;

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
			TreeCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as TreeCfgLocalBase;
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
	public abstract class TreeCfgLocalBase : LocalizationCfgBase
	{
		public abstract TreeCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649