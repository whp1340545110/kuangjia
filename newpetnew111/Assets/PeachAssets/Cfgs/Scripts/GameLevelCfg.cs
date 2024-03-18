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
	public partial class GameLevelCfgEntry : EERowData
	{
		/// <summary>
		///level
		/// </summary>
		[SerializeField]
		private int _mLevel;
		public  int mLevel { get { return _mLevel; } }

		/// <summary>
		///目标id
		/// </summary>
		[SerializeField]
		private int[] _mTarget;
		public  int[] mTarget { get { return _mTarget; } }

		/// <summary>
		///初始生成
		/// </summary>
		[SerializeField]
		private int[] _mCreate;
		public  int[] mCreate { get { return _mCreate; } }

		/// <summary>
		///最小方块
		/// </summary>
		[SerializeField]
		private int _mMin;
		public  int mMin { get { return _mMin; } }

		/// <summary>
		///最大方块
		/// </summary>
		[SerializeField]
		private int _mMax;
		public  int mMax { get { return _mMax; } }

		/// <summary>
		///获得的美金
		/// </summary>
		[SerializeField]
		private int[] _mGoid;
		public  int[] mGoid { get { return _mGoid; } }

		/// <summary>
		///关卡结束获得美金
		/// </summary>
		[SerializeField]
		private int _mEndGoid;
		public  int mEndGoid { get { return _mEndGoid; } }


		public GameLevelCfgEntry()
		{
		}

#if UNITY_EDITOR
		public GameLevelCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mLevel);
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
			string _mCreateArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mCreateArrayString))
			{
				string[] _mCreateArray = _mCreateArrayString.Split(',');
				int _mCreateCount = _mCreateArray.Length;
				_mCreate = new int[_mCreateCount];
				for(int i = 0; i < _mCreateCount; i++)
				{
					int.TryParse(_mCreateArray[i], out _mCreate[i]);
				}
			}
			int.TryParse(sheet[row][column++], out _mMin);
			int.TryParse(sheet[row][column++], out _mMax);
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
			int.TryParse(sheet[row][column++], out _mEndGoid);
		}
#endif
		public void ChangeLocalProperties(GameLevelCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class GameLevelCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class GameLevelCfg : CfgBase
	{
		[SerializeField]
		private List<GameLevelCfgEntry> _entryList = new List<GameLevelCfgEntry>();

		public IReadOnlyList<GameLevelCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as GameLevelCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public GameLevelCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as GameLevelCfgEntry;

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
			GameLevelCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as GameLevelCfgLocalBase;
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
	public abstract class GameLevelCfgLocalBase : LocalizationCfgBase
	{
		public abstract GameLevelCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649