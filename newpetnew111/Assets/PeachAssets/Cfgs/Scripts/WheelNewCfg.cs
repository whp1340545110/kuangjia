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
	public partial class WheelNewCfgEntry : EERowData
	{
		/// <summary>
		///Id
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
		///奖励数量
		/// </summary>
		[SerializeField]
		private int[] _mRrwerd;
		public  int[] mRrwerd { get { return _mRrwerd; } }

		/// <summary>
		///权重
		/// </summary>
		[SerializeField]
		private int _mFreeProbability;
		public  int mFreeProbability { get { return _mFreeProbability; } }

		/// <summary>
		///图片名字
		/// </summary>
		[SerializeField]
		private string _mSprite;
		public  string mSprite { get { return _mSprite; } }

		/// <summary>
		///红包表
		/// </summary>
		[SerializeField]
		private string _mCfgName;
		public  string mCfgName { get { return _mCfgName; } }


		public WheelNewCfgEntry()
		{
		}

#if UNITY_EDITOR
		public WheelNewCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			_mName = sheet[row][column++] ?? "";
			int.TryParse(sheet[row][column++], out _mType);
			string _mRrwerdArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mRrwerdArrayString))
			{
				string[] _mRrwerdArray = _mRrwerdArrayString.Split(',');
				int _mRrwerdCount = _mRrwerdArray.Length;
				_mRrwerd = new int[_mRrwerdCount];
				for(int i = 0; i < _mRrwerdCount; i++)
				{
					int.TryParse(_mRrwerdArray[i], out _mRrwerd[i]);
				}
			}
			int.TryParse(sheet[row][column++], out _mFreeProbability);
			_mSprite = sheet[row][column++] ?? "";
			_mCfgName = sheet[row][column++] ?? "";
		}
#endif
		public void ChangeLocalProperties(WheelNewCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class WheelNewCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class WheelNewCfg : CfgBase
	{
		[SerializeField]
		private List<WheelNewCfgEntry> _entryList = new List<WheelNewCfgEntry>();

		public IReadOnlyList<WheelNewCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as WheelNewCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public WheelNewCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as WheelNewCfgEntry;

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
			WheelNewCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as WheelNewCfgLocalBase;
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
	public abstract class WheelNewCfgLocalBase : LocalizationCfgBase
	{
		public abstract WheelNewCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649