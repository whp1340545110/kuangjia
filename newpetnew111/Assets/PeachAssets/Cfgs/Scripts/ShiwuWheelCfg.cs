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
	public partial class ShiwuWheelCfgEntry : EERowData
	{
		/// <summary>
		///ID
		/// </summary>
		[SerializeField]
		private int _mId;
		public  int mId { get { return _mId; } }

		/// <summary>
		///类型(参考WheelCfg)
		/// </summary>
		[SerializeField]
		private int _mType;
		public  int mType { get { return _mType; } }

		/// <summary>
		///奖励数量
		/// </summary>
		[SerializeField]
		private int[] _mDebris;
		public  int[] mDebris { get { return _mDebris; } }

		/// <summary>
		///实物id（参考ShiWuCfg）
		/// </summary>
		[SerializeField]
		private int _mShiWuId;
		public  int mShiWuId { get { return _mShiWuId; } }

		/// <summary>
		///权重
		/// </summary>
		[SerializeField]
		private int _mFreeProbability;
		public  int mFreeProbability { get { return _mFreeProbability; } }

		/// <summary>
		///红包表(是现金的配不是的不写)
		/// </summary>
		[SerializeField]
		private string _mCfgName;
		public  string mCfgName { get { return _mCfgName; } }

		/// <summary>
		///图片名字
		/// </summary>
		[SerializeField]
		private string _mSprite;
		public  string mSprite { get { return _mSprite; } }


		public ShiwuWheelCfgEntry()
		{
		}

#if UNITY_EDITOR
		public ShiwuWheelCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			int.TryParse(sheet[row][column++], out _mType);
			string _mDebrisArrayString=sheet[row][column++];
			if(!string.IsNullOrEmpty(_mDebrisArrayString))
			{
				string[] _mDebrisArray = _mDebrisArrayString.Split(',');
				int _mDebrisCount = _mDebrisArray.Length;
				_mDebris = new int[_mDebrisCount];
				for(int i = 0; i < _mDebrisCount; i++)
				{
					int.TryParse(_mDebrisArray[i], out _mDebris[i]);
				}
			}
			int.TryParse(sheet[row][column++], out _mShiWuId);
			int.TryParse(sheet[row][column++], out _mFreeProbability);
			_mCfgName = sheet[row][column++] ?? "";
			_mSprite = sheet[row][column++] ?? "";
		}
#endif
		public void ChangeLocalProperties(ShiwuWheelCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class ShiwuWheelCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class ShiwuWheelCfg : CfgBase
	{
		[SerializeField]
		private List<ShiwuWheelCfgEntry> _entryList = new List<ShiwuWheelCfgEntry>();

		public IReadOnlyList<ShiwuWheelCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as ShiwuWheelCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public ShiwuWheelCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as ShiwuWheelCfgEntry;

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
			ShiwuWheelCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as ShiwuWheelCfgLocalBase;
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
	public abstract class ShiwuWheelCfgLocalBase : LocalizationCfgBase
	{
		public abstract ShiwuWheelCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649