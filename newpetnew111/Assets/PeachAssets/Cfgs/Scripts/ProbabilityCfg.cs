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
	public partial class ProbabilityCfgEntry : EERowData
	{
		/// <summary>
		///id
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///权重
		/// </summary>
		[SerializeField]
		private int[] _mWeight;
		public  int[] mWeight { get { return _mWeight; } }

		/// <summary>
		///卡牌类型
		/// </summary>
		[SerializeField]
		private int[] _mType;
		public  int[] mType { get { return _mType; } }

		/// <summary>
		///最高卡牌
		/// </summary>
		[SerializeField]
		private int _mMaxType;
		public  int mMaxType { get { return _mMaxType; } }


		public ProbabilityCfgEntry()
		{
		}

#if UNITY_EDITOR
		public ProbabilityCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			string _mWeightArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mWeightArrayString))
			{
				string[] _mWeightArray = _mWeightArrayString.Split(',');
				int _mWeightCount = _mWeightArray.Length;
				_mWeight = new int[_mWeightCount];
				for(int i = 0; i < _mWeightCount; i++)
				{
					int.TryParse(_mWeightArray[i], out _mWeight[i]);
				}
			}
			string _mTypeArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mTypeArrayString))
			{
				string[] _mTypeArray = _mTypeArrayString.Split(',');
				int _mTypeCount = _mTypeArray.Length;
				_mType = new int[_mTypeCount];
				for(int i = 0; i < _mTypeCount; i++)
				{
					int.TryParse(_mTypeArray[i], out _mType[i]);
				}
			}
			int.TryParse(sheet[row][column++], out _mMaxType);
		}
#endif
		public void ChangeLocalProperties(ProbabilityCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class ProbabilityCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class ProbabilityCfg : CfgBase
	{
		[SerializeField]
		private List<ProbabilityCfgEntry> _entryList = new List<ProbabilityCfgEntry>();

		public IReadOnlyList<ProbabilityCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as ProbabilityCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public ProbabilityCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as ProbabilityCfgEntry;

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
			ProbabilityCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as ProbabilityCfgLocalBase;
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
	public abstract class ProbabilityCfgLocalBase : LocalizationCfgBase
	{
		public abstract ProbabilityCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649