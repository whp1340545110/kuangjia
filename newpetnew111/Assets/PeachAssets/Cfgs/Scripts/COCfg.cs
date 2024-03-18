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
	public partial class COCfgEntry : EERowData
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


		public COCfgEntry()
		{
		}

#if UNITY_EDITOR
		public COCfgEntry(List<List<string>> sheet, int row, int column)
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
		public void ChangeLocalProperties(COCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class COCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class COCfg : CfgBase
	{
		[SerializeField]
		private List<COCfgEntry> _entryList = new List<COCfgEntry>();

		public IReadOnlyList<COCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as COCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public COCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as COCfgEntry;

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
			COCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as COCfgLocalBase;
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
	public abstract class COCfgLocalBase : LocalizationCfgBase
	{
		public abstract COCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649