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
	public partial class AchievementCfgEntry : EERowData
	{
		/// <summary>
		///索引
		/// </summary>
		[SerializeField]
		private int _mIndex;
		public  int mIndex { get { return _mIndex; } }

		/// <summary>
		///任務Id
		/// </summary>
		[SerializeField]
		private int _mId;
		[EEKeyProperty]
		public  int mId { get { return _mId; } }

		/// <summary>
		///任務内容
		/// </summary>
		[SerializeField]
		private string _mContent;
		public  string mContent { get { return _mContent; } }

		/// <summary>
		///目標數量
		/// </summary>
		[SerializeField]
		private int[] _mTarget;
		public  int[] mTarget { get { return _mTarget; } }

		/// <summary>
		///目标类型
		/// </summary>
		[SerializeField]
		private int _mType;
		public  int mType { get { return _mType; } }

		/// <summary>
		///奖励类型
		/// </summary>
		[SerializeField]
		private int _mRward;
		public  int mRward { get { return _mRward; } }

		/// <summary>
		///奖励类型区间值
		/// </summary>
		[SerializeField]
		private int[] _mValue;
		public  int[] mValue { get { return _mValue; } }

		/// <summary>
		///红包表
		/// </summary>
		[SerializeField]
		private string _mCfgName;
		public  string mCfgName { get { return _mCfgName; } }


		public AchievementCfgEntry()
		{
		}

#if UNITY_EDITOR
		public AchievementCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mIndex);
			int.TryParse(sheet[row][column++], out _mId);
			_mContent = sheet[row][column++] ?? "";
			string _mTargetArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mTargetArrayString))
			{
				string[] _mTargetArray = _mTargetArrayString.Split(',');
				int _mTargetCount = _mTargetArray.Length;
				_mTarget = new int[_mTargetCount];
				for(int i = 0; i < _mTargetCount; i++)
				{
					int.TryParse(_mTargetArray[i], out _mTarget[i]);
				}
			}
			int.TryParse(sheet[row][column++], out _mType);
			int.TryParse(sheet[row][column++], out _mRward);
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
			_mCfgName = sheet[row][column++] ?? "";
		}
#endif
		public void ChangeLocalProperties(AchievementCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class AchievementCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class AchievementCfg : CfgBase
	{
		[SerializeField]
		private List<AchievementCfgEntry> _entryList = new List<AchievementCfgEntry>();

		public IReadOnlyList<AchievementCfgEntry> EntryList => _entryList;

		private Dictionary<int, AchievementCfgEntry> _entryDic;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as AchievementCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public AchievementCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as AchievementCfgEntry;

		}

		public AchievementCfgEntry GetEntryByKey(int kId)
		{
			AchievementCfgEntry result;
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
			_entryDic = new Dictionary<int, AchievementCfgEntry>(_entryList.Count);
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
			AchievementCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as AchievementCfgLocalBase;
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
	public abstract class AchievementCfgLocalBase : LocalizationCfgBase
	{
		public abstract AchievementCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649