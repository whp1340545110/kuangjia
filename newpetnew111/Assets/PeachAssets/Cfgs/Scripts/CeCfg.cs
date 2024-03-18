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
	public partial class CeCfgEntry : EERowData
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
		///区间
		/// </summary>
		[SerializeField]
		private int[] _mSection;
		public  int[] mSection { get { return _mSection; } }

		/// <summary>
		///概率
		/// </summary>
		[SerializeField]
		private float _mProbability;
		public  float mProbability { get { return _mProbability; } }


		public CeCfgEntry()
		{
		}

#if UNITY_EDITOR
		public CeCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			int.TryParse(sheet[row][column++], out _mCount);
			string _mSectionArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mSectionArrayString))
			{
				string[] _mSectionArray = _mSectionArrayString.Split(',');
				int _mSectionCount = _mSectionArray.Length;
				_mSection = new int[_mSectionCount];
				for(int i = 0; i < _mSectionCount; i++)
				{
					int.TryParse(_mSectionArray[i], out _mSection[i]);
				}
			}
			float.TryParse(sheet[row][column++], out _mProbability);
		}
#endif
		public void ChangeLocalProperties(CeCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class CeCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class CeCfg : CfgBase
	{
		[SerializeField]
		private List<CeCfgEntry> _entryList = new List<CeCfgEntry>();

		public IReadOnlyList<CeCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as CeCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public CeCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as CeCfgEntry;

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
			CeCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as CeCfgLocalBase;
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
	public abstract class CeCfgLocalBase : LocalizationCfgBase
	{
		public abstract CeCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649