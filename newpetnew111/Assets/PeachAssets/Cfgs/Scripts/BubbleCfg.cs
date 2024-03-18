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
	public partial class BubbleCfgEntry : EERowData
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
		///概率
		/// </summary>
		[SerializeField]
		private float _mProbability;
		public  float mProbability { get { return _mProbability; } }


		public BubbleCfgEntry()
		{
		}

#if UNITY_EDITOR
		public BubbleCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			int.TryParse(sheet[row][column++], out _mCount);
			float.TryParse(sheet[row][column++], out _mProbability);
		}
#endif
		public void ChangeLocalProperties(BubbleCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class BubbleCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class BubbleCfg : CfgBase
	{
		[SerializeField]
		private List<BubbleCfgEntry> _entryList = new List<BubbleCfgEntry>();

		public IReadOnlyList<BubbleCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as BubbleCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public BubbleCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as BubbleCfgEntry;

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
			BubbleCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as BubbleCfgLocalBase;
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
	public abstract class BubbleCfgLocalBase : LocalizationCfgBase
	{
		public abstract BubbleCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649