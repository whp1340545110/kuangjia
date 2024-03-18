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
	public partial class SignInCfgEntry : EERowData
	{
		/// <summary>
		///日期
		/// </summary>
		[SerializeField]
		private int _mDay;
		[EEKeyProperty]
		public  int mDay { get { return _mDay; } }

		/// <summary>
		///签到类型
		/// </summary>
		[SerializeField]
		private int _mType;
		public  int mType { get { return _mType; } }

		/// <summary>
		///图片
		/// </summary>
		[SerializeField]
		private string _mPath;
		public  string mPath { get { return _mPath; } }

		/// <summary>
		///奖励数量
		/// </summary>
		[SerializeField]
		private int[] _mNum;
		public  int[] mNum { get { return _mNum; } }

		/// <summary>
		///红包表
		/// </summary>
		[SerializeField]
		private string _mCfgName;
		public  string mCfgName { get { return _mCfgName; } }

		/// <summary>
		///时间
		/// </summary>
		[SerializeField]
		private int _mTime;
		public  int mTime { get { return _mTime; } }

		/// <summary>
		///中午奖励类型
		/// </summary>
		[SerializeField]
		private int _mNoonType;
		public  int mNoonType { get { return _mNoonType; } }

		/// <summary>
		///奖励数量
		/// </summary>
		[SerializeField]
		private int[] _mNoonNum;
		public  int[] mNoonNum { get { return _mNoonNum; } }

		/// <summary>
		///红包表
		/// </summary>
		[SerializeField]
		private string _mCfgNoonName;
		public  string mCfgNoonName { get { return _mCfgNoonName; } }

		/// <summary>
		///时间
		/// </summary>
		[SerializeField]
		private int _mNithtTime;
		public  int mNithtTime { get { return _mNithtTime; } }

		/// <summary>
		///晚上奖励类型
		/// </summary>
		[SerializeField]
		private int _mNightType;
		public  int mNightType { get { return _mNightType; } }

		/// <summary>
		///奖励数量
		/// </summary>
		[SerializeField]
		private int[] _mNightNum;
		public  int[] mNightNum { get { return _mNightNum; } }

		/// <summary>
		///红包表
		/// </summary>
		[SerializeField]
		private string _mCfgNightName;
		public  string mCfgNightName { get { return _mCfgNightName; } }


		public SignInCfgEntry()
		{
		}

#if UNITY_EDITOR
		public SignInCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mDay);
			int.TryParse(sheet[row][column++], out _mType);
			_mPath = sheet[row][column++] ?? "";
			string _mNumArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mNumArrayString))
			{
				string[] _mNumArray = _mNumArrayString.Split(',');
				int _mNumCount = _mNumArray.Length;
				_mNum = new int[_mNumCount];
				for(int i = 0; i < _mNumCount; i++)
				{
					int.TryParse(_mNumArray[i], out _mNum[i]);
				}
			}
			_mCfgName = sheet[row][column++] ?? "";
			int.TryParse(sheet[row][column++], out _mTime);
			int.TryParse(sheet[row][column++], out _mNoonType);
			string _mNoonNumArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mNoonNumArrayString))
			{
				string[] _mNoonNumArray = _mNoonNumArrayString.Split(',');
				int _mNoonNumCount = _mNoonNumArray.Length;
				_mNoonNum = new int[_mNoonNumCount];
				for(int i = 0; i < _mNoonNumCount; i++)
				{
					int.TryParse(_mNoonNumArray[i], out _mNoonNum[i]);
				}
			}
			_mCfgNoonName = sheet[row][column++] ?? "";
			int.TryParse(sheet[row][column++], out _mNithtTime);
			int.TryParse(sheet[row][column++], out _mNightType);
			string _mNightNumArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mNightNumArrayString))
			{
				string[] _mNightNumArray = _mNightNumArrayString.Split(',');
				int _mNightNumCount = _mNightNumArray.Length;
				_mNightNum = new int[_mNightNumCount];
				for(int i = 0; i < _mNightNumCount; i++)
				{
					int.TryParse(_mNightNumArray[i], out _mNightNum[i]);
				}
			}
			_mCfgNightName = sheet[row][column++] ?? "";
		}
#endif
		public void ChangeLocalProperties(SignInCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class SignInCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class SignInCfg : CfgBase
	{
		[SerializeField]
		private List<SignInCfgEntry> _entryList = new List<SignInCfgEntry>();

		public IReadOnlyList<SignInCfgEntry> EntryList => _entryList;

		private Dictionary<int, SignInCfgEntry> _entryDic;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as SignInCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public SignInCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as SignInCfgEntry;

		}

		public SignInCfgEntry GetEntryByKey(int kId)
		{
			SignInCfgEntry result;
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
			_entryDic = new Dictionary<int, SignInCfgEntry>(_entryList.Count);
			for (int i = 0; i < _entryList.Count; i++)
			{
				_entryDic.Add(_entryList[i].mDay, _entryList[i]);
				_entryList[i].OnEntryLoad();
			}
			OnCfgLoadCustomized();
		}

		public override void SetLanguage(SystemLanguage language)
		{
			var cfgName = GetType().Name+ "_" + language;
			SignInCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as SignInCfgLocalBase;
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
	public abstract class SignInCfgLocalBase : LocalizationCfgBase
	{
		public abstract SignInCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649