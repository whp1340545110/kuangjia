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
	public partial class GameSettingCfgEntry : EERowData
	{
		/// <summary>
		///能量概率
		/// </summary>
		[SerializeField]
		private float _energy_probability;
		public  float Energy_probability { get { return _energy_probability; } }

		/// <summary>
		///翻卡最大能量值
		/// </summary>
		[SerializeField]
		private int _floop_max_enery;
		public  int Floop_max_enery { get { return _floop_max_enery; } }

		/// <summary>
		///百分比几率
		/// </summary>
		[SerializeField]
		private int _floopr_probability;
		public  int Floopr_probability { get { return _floopr_probability; } }

		/// <summary>
		///翻卡能量增长值
		/// </summary>
		[SerializeField]
		private int _floop_up_enery;
		public  int Floop_up_enery { get { return _floop_up_enery; } }

		/// <summary>
		///转盘最大能量值
		/// </summary>
		[SerializeField]
		private int _wheel_max_emery;
		public  int Wheel_max_emery { get { return _wheel_max_emery; } }

		/// <summary>
		///转盘能量百分比几率
		/// </summary>
		[SerializeField]
		private int _wheel_probability;
		public  int Wheel_probability { get { return _wheel_probability; } }

		/// <summary>
		///转盘能量增长值
		/// </summary>
		[SerializeField]
		private int _wheel_up_enery;
		public  int Wheel_up_enery { get { return _wheel_up_enery; } }

		/// <summary>
		///摇钱树最大能量值
		/// </summary>
		[SerializeField]
		private int _tree_max_emery;
		public  int Tree_max_emery { get { return _tree_max_emery; } }

		/// <summary>
		///摇钱树能量百分比几率
		/// </summary>
		[SerializeField]
		private int _tree_probability;
		public  int Tree_probability { get { return _tree_probability; } }

		/// <summary>
		///摇钱树能量增长值
		/// </summary>
		[SerializeField]
		private int _tree_up_enery;
		public  int Tree_up_enery { get { return _tree_up_enery; } }

		/// <summary>
		///32M消除以后加钻石数量
		/// </summary>
		[SerializeField]
		private int _diamond_Num;
		public  int Diamond_Num { get { return _diamond_Num; } }

		/// <summary>
		///弹幕一开始出来的时间
		/// </summary>
		[SerializeField]
		private int _barrageInitTime;
		public  int BarrageInitTime { get { return _barrageInitTime; } }

		/// <summary>
		///弹幕以后随机时间最小值
		/// </summary>
		[SerializeField]
		private int _barrageMinTime;
		public  int BarrageMinTime { get { return _barrageMinTime; } }

		/// <summary>
		///弹幕以后随机时间最大值
		/// </summary>
		[SerializeField]
		private int _barrageMaxTime;
		public  int BarrageMaxTime { get { return _barrageMaxTime; } }

		/// <summary>
		///摇钱树离线时间（小时）
		/// </summary>
		[SerializeField]
		private int _offlineTime;
		public  int OfflineTime { get { return _offlineTime; } }

		/// <summary>
		///购买道具用的钻石
		/// </summary>
		[SerializeField]
		private int _daoJuJewel;
		public  int DaoJuJewel { get { return _daoJuJewel; } }

		/// <summary>
		///免费钻石
		/// </summary>
		[SerializeField]
		private int[] _freeJewel;
		public  int[] FreeJewel { get { return _freeJewel; } }

		/// <summary>
		///初始过关美金
		/// </summary>
		[SerializeField]
		private int _startLevelMoney;
		public  int StartLevelMoney { get { return _startLevelMoney; } }

		/// <summary>
		///增长系数
		/// </summary>
		[SerializeField]
		private float _percentageNum;
		public  float PercentageNum { get { return _percentageNum; } }


		public GameSettingCfgEntry()
		{
		}

#if UNITY_EDITOR
		public GameSettingCfgEntry(List<List<string>> sheet, int row, int column)
		{
			float.TryParse(sheet[row][column++], out _energy_probability);
			int.TryParse(sheet[row][column++], out _floop_max_enery);
			int.TryParse(sheet[row][column++], out _floopr_probability);
			int.TryParse(sheet[row][column++], out _floop_up_enery);
			int.TryParse(sheet[row][column++], out _wheel_max_emery);
			int.TryParse(sheet[row][column++], out _wheel_probability);
			int.TryParse(sheet[row][column++], out _wheel_up_enery);
			int.TryParse(sheet[row][column++], out _tree_max_emery);
			int.TryParse(sheet[row][column++], out _tree_probability);
			int.TryParse(sheet[row][column++], out _tree_up_enery);
			int.TryParse(sheet[row][column++], out _diamond_Num);
			int.TryParse(sheet[row][column++], out _barrageInitTime);
			int.TryParse(sheet[row][column++], out _barrageMinTime);
			int.TryParse(sheet[row][column++], out _barrageMaxTime);
			int.TryParse(sheet[row][column++], out _offlineTime);
			int.TryParse(sheet[row][column++], out _daoJuJewel);
			string _freeJewelArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_freeJewelArrayString))
			{
				string[] _freeJewelArray = _freeJewelArrayString.Split(',');
				int _freeJewelCount = _freeJewelArray.Length;
				_freeJewel = new int[_freeJewelCount];
				for(int i = 0; i < _freeJewelCount; i++)
				{
					int.TryParse(_freeJewelArray[i], out _freeJewel[i]);
				}
			}
			int.TryParse(sheet[row][column++], out _startLevelMoney);
			float.TryParse(sheet[row][column++], out _percentageNum);
		}
#endif
		public void ChangeLocalProperties(GameSettingCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class GameSettingCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class GameSettingCfg : CfgBase
	{
		[SerializeField]
		private List<GameSettingCfgEntry> _entryList = new List<GameSettingCfgEntry>();

		public IReadOnlyList<GameSettingCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as GameSettingCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public GameSettingCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as GameSettingCfgEntry;

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
			GameSettingCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as GameSettingCfgLocalBase;
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
	public abstract class GameSettingCfgLocalBase : LocalizationCfgBase
	{
		public abstract GameSettingCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649