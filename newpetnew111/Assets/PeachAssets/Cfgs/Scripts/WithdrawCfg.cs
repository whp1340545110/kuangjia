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
	public partial class WithdrawCfgEntry : EERowData
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

		/// <summary>
		///转盘次数
		/// </summary>
		[SerializeField]
		private int _mVideoCount;
		public  int mVideoCount { get { return _mVideoCount; } }

		/// <summary>
		///黄金筹码
		/// </summary>
		[SerializeField]
		private int _mGoldChip;
		public  int mGoldChip { get { return _mGoldChip; } }

		/// <summary>
		///提现内容
		/// </summary>
		[SerializeField]
		private string _mTip1;
		public  string mTip1 { get { return _mTip1; } }

		/// <summary>
		///提现内容
		/// </summary>
		[SerializeField]
		private string _mTip2;
		public  string mTip2 { get { return _mTip2; } }

		/// <summary>
		///提现内容
		/// </summary>
		[SerializeField]
		private string _mTip3;
		public  string mTip3 { get { return _mTip3; } }


		public WithdrawCfgEntry()
		{
		}

#if UNITY_EDITOR
		public WithdrawCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			int.TryParse(sheet[row][column++], out _mJewelValue);
			int.TryParse(sheet[row][column++], out _mVideoCount);
			int.TryParse(sheet[row][column++], out _mGoldChip);
			_mTip1 = sheet[row][column++] ?? "";
			_mTip2 = sheet[row][column++] ?? "";
			_mTip3 = sheet[row][column++] ?? "";
		}
#endif
		public void ChangeLocalProperties(WithdrawCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class WithdrawCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class WithdrawCfg : CfgBase
	{
		[SerializeField]
		private List<WithdrawCfgEntry> _entryList = new List<WithdrawCfgEntry>();

		public IReadOnlyList<WithdrawCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as WithdrawCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public WithdrawCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as WithdrawCfgEntry;

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
			WithdrawCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as WithdrawCfgLocalBase;
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
	public abstract class WithdrawCfgLocalBase : LocalizationCfgBase
	{
		public abstract WithdrawCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649