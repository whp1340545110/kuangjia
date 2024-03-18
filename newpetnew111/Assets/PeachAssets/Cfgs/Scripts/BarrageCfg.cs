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
	public partial class BarrageCfgEntry : EERowData
	{
		/// <summary>
		///ID
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///名字
		/// </summary>
		[SerializeField]
		private string _mName;
		public  string mName { get { return _mName; } }

		/// <summary>
		///提示
		/// </summary>
		[SerializeField]
		private string _mTip;
		public  string mTip { get { return _mTip; } }

		/// <summary>
		///弹幕时间
		/// </summary>
		[SerializeField]
		private float _mTime;
		public  float mTime { get { return _mTime; } }


		public BarrageCfgEntry()
		{
		}

#if UNITY_EDITOR
		public BarrageCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			_mName = sheet[row][column++] ?? "";
			_mTip = sheet[row][column++] ?? "";
			float.TryParse(sheet[row][column++], out _mTime);
		}
#endif
		public void ChangeLocalProperties(BarrageCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class BarrageCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class BarrageCfg : CfgBase
	{
		[SerializeField]
		private List<BarrageCfgEntry> _entryList = new List<BarrageCfgEntry>();

		public IReadOnlyList<BarrageCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as BarrageCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public BarrageCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as BarrageCfgEntry;

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
			BarrageCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as BarrageCfgLocalBase;
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
	public abstract class BarrageCfgLocalBase : LocalizationCfgBase
	{
		public abstract BarrageCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649