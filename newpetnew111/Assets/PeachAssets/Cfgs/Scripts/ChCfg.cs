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
	public partial class ChCfgEntry : EERowData
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
		private float[] _mSection;
		public  float[] mSection { get { return _mSection; } }

		/// <summary>
		///金额区间
		/// </summary>
		[SerializeField]
		private float[] _mRed;
		public  float[] mRed { get { return _mRed; } }


		public ChCfgEntry()
		{
		}

#if UNITY_EDITOR
		public ChCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			string _mSectionArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mSectionArrayString))
			{
				string[] _mSectionArray = _mSectionArrayString.Split(',');
				int _mSectionCount = _mSectionArray.Length;
				_mSection = new float[_mSectionCount];
				for(int i = 0; i < _mSectionCount; i++)
				{
					float.TryParse(_mSectionArray[i], out _mSection[i]);
				}
			}
			string _mRedArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mRedArrayString))
			{
				string[] _mRedArray = _mRedArrayString.Split(',');
				int _mRedCount = _mRedArray.Length;
				_mRed = new float[_mRedCount];
				for(int i = 0; i < _mRedCount; i++)
				{
					float.TryParse(_mRedArray[i], out _mRed[i]);
				}
			}
		}
#endif
		public void ChangeLocalProperties(ChCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class ChCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class ChCfg : CfgBase
	{
		[SerializeField]
		private List<ChCfgEntry> _entryList = new List<ChCfgEntry>();

		public IReadOnlyList<ChCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as ChCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public ChCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as ChCfgEntry;

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
			ChCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as ChCfgLocalBase;
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
	public abstract class ChCfgLocalBase : LocalizationCfgBase
	{
		public abstract ChCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649