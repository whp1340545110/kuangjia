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
	public partial class CO1CfgEntry : EERowData
	{
		/// <summary>
		///ID
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///次数
		/// </summary>
		[SerializeField]
		private int _mCount;
		public  int mCount { get { return _mCount; } }

		/// <summary>
		///计算倍数
		/// </summary>
		[SerializeField]
		private float _mMultiples;
		public  float mMultiples { get { return _mMultiples; } }

		/// <summary>
		///显示倍数
		/// </summary>
		[SerializeField]
		private int _mShowMultiples;
		public  int mShowMultiples { get { return _mShowMultiples; } }


		public CO1CfgEntry()
		{
		}

#if UNITY_EDITOR
		public CO1CfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			int.TryParse(sheet[row][column++], out _mCount);
			float.TryParse(sheet[row][column++], out _mMultiples);
			int.TryParse(sheet[row][column++], out _mShowMultiples);
		}
#endif
		public void ChangeLocalProperties(CO1CfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class CO1CfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class CO1Cfg : CfgBase
	{
		[SerializeField]
		private List<CO1CfgEntry> _entryList = new List<CO1CfgEntry>();

		public IReadOnlyList<CO1CfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as CO1CfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public CO1CfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as CO1CfgEntry;

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
			CO1CfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as CO1CfgLocalBase;
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
	public abstract class CO1CfgLocalBase : LocalizationCfgBase
	{
		public abstract CO1CfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649