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
	public partial class CvCfgEntry : EERowData
	{
		/// <summary>
		///ID
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///拥有区间
		/// </summary>
		[SerializeField]
		private int[] _mSection;
		public  int[] mSection { get { return _mSection; } }

		/// <summary>
		///金额区间
		/// </summary>
		[SerializeField]
		private int[] _mRed;
		public  int[] mRed { get { return _mRed; } }


		public CvCfgEntry()
		{
		}

#if UNITY_EDITOR
		public CvCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
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
			string _mRedArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mRedArrayString))
			{
				string[] _mRedArray = _mRedArrayString.Split(',');
				int _mRedCount = _mRedArray.Length;
				_mRed = new int[_mRedCount];
				for(int i = 0; i < _mRedCount; i++)
				{
					int.TryParse(_mRedArray[i], out _mRed[i]);
				}
			}
		}
#endif
		public void ChangeLocalProperties(CvCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class CvCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class CvCfg : CfgBase
	{
		[SerializeField]
		private List<CvCfgEntry> _entryList = new List<CvCfgEntry>();

		public IReadOnlyList<CvCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as CvCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public CvCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as CvCfgEntry;

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
			CvCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as CvCfgLocalBase;
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
	public abstract class CvCfgLocalBase : LocalizationCfgBase
	{
		public abstract CvCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649