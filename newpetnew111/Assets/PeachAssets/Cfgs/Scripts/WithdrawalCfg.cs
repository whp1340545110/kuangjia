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
	public partial class WithdrawalCfgEntry : EERowData
	{
		/// <summary>
		///Id
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///提现金额
		/// </summary>
		[SerializeField]
		private int _mJewelValue;
		public  int mJewelValue { get { return _mJewelValue; } }


		public WithdrawalCfgEntry()
		{
		}

#if UNITY_EDITOR
		public WithdrawalCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			int.TryParse(sheet[row][column++], out _mJewelValue);
		}
#endif
		public void ChangeLocalProperties(WithdrawalCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class WithdrawalCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class WithdrawalCfg : CfgBase
	{
		[SerializeField]
		private List<WithdrawalCfgEntry> _entryList = new List<WithdrawalCfgEntry>();

		public IReadOnlyList<WithdrawalCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as WithdrawalCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public WithdrawalCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as WithdrawalCfgEntry;

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
			WithdrawalCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as WithdrawalCfgLocalBase;
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
	public abstract class WithdrawalCfgLocalBase : LocalizationCfgBase
	{
		public abstract WithdrawalCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649