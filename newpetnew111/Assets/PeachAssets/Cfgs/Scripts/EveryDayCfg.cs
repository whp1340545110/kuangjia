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
	public partial class EveryDayCfgEntry : EERowData
	{
		/// <summary>
		///Id
		/// </summary>
		[SerializeField]
		private int _mId;
		[EEKeyProperty]
		public  int mId { get { return _mId; } }

		/// <summary>
		///提现金额
		/// </summary>
		[SerializeField]
		private int[] _mGoid;
		public  int[] mGoid { get { return _mGoid; } }


		public EveryDayCfgEntry()
		{
		}

#if UNITY_EDITOR
		public EveryDayCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			string _mGoidArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mGoidArrayString))
			{
				string[] _mGoidArray = _mGoidArrayString.Split(',');
				int _mGoidCount = _mGoidArray.Length;
				_mGoid = new int[_mGoidCount];
				for(int i = 0; i < _mGoidCount; i++)
				{
					int.TryParse(_mGoidArray[i], out _mGoid[i]);
				}
			}
		}
#endif
		public void ChangeLocalProperties(EveryDayCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class EveryDayCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class EveryDayCfg : CfgBase
	{
		[SerializeField]
		private List<EveryDayCfgEntry> _entryList = new List<EveryDayCfgEntry>();

		public IReadOnlyList<EveryDayCfgEntry> EntryList => _entryList;

		private Dictionary<int, EveryDayCfgEntry> _entryDic;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as EveryDayCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public EveryDayCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as EveryDayCfgEntry;

		}

		public EveryDayCfgEntry GetEntryByKey(int kId)
		{
			EveryDayCfgEntry result;
			if (_entryDic.TryGetValue(kId, out result)){
				return result;
			}
			 return null;
		}

		partial void OnCfgLoadCustomized();

		public override void OnCfgLoad()
		{
			if (_entryList == null || _entryList.Count == 0)
			{
				return;
			}
			_entryDic = new Dictionary<int, EveryDayCfgEntry>(_entryList.Count);
			for (int i = 0; i < _entryList.Count; i++)
			{
				_entryDic.Add(_entryList[i].mId, _entryList[i]);
				_entryList[i].OnEntryLoad();
			}
			OnCfgLoadCustomized();
		}

		public override void SetLanguage(SystemLanguage language)
		{
			var cfgName = GetType().Name+ "_" + language;
			EveryDayCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as EveryDayCfgLocalBase;
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
	public abstract class EveryDayCfgLocalBase : LocalizationCfgBase
	{
		public abstract EveryDayCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649