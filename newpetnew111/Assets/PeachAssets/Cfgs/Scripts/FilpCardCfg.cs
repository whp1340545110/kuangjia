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
	public partial class FilpCardCfgEntry : EERowData
	{
		/// <summary>
		///奖励Id
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///奖励名字
		/// </summary>
		[SerializeField]
		private string _mName;
		public  string mName { get { return _mName; } }

		/// <summary>
		///獎勵類型
		/// </summary>
		[SerializeField]
		private int _mType;
		public  int mType { get { return _mType; } }

		/// <summary>
		///奖励值区间
		/// </summary>
		[SerializeField]
		private int[] _mValue;
		public  int[] mValue { get { return _mValue; } }

		/// <summary>
		///权重
		/// </summary>
		[SerializeField]
		private int _mWeight;
		public  int mWeight { get { return _mWeight; } }

		/// <summary>
		///红包表
		/// </summary>
		[SerializeField]
		private string _mCfgName;
		public  string mCfgName { get { return _mCfgName; } }


		public FilpCardCfgEntry()
		{
		}

#if UNITY_EDITOR
		public FilpCardCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			_mName = sheet[row][column++] ?? "";
			int.TryParse(sheet[row][column++], out _mType);
			string _mValueArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mValueArrayString))
			{
				string[] _mValueArray = _mValueArrayString.Split(',');
				int _mValueCount = _mValueArray.Length;
				_mValue = new int[_mValueCount];
				for(int i = 0; i < _mValueCount; i++)
				{
					int.TryParse(_mValueArray[i], out _mValue[i]);
				}
			}
			int.TryParse(sheet[row][column++], out _mWeight);
			_mCfgName = sheet[row][column++] ?? "";
		}
#endif
		public void ChangeLocalProperties(FilpCardCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class FilpCardCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class FilpCardCfg : CfgBase
	{
		[SerializeField]
		private List<FilpCardCfgEntry> _entryList = new List<FilpCardCfgEntry>();

		public IReadOnlyList<FilpCardCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as FilpCardCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public FilpCardCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as FilpCardCfgEntry;

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
			FilpCardCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as FilpCardCfgLocalBase;
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
	public abstract class FilpCardCfgLocalBase : LocalizationCfgBase
	{
		public abstract FilpCardCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649