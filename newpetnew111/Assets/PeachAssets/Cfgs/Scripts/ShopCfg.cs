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
	public partial class ShopCfgEntry : EERowData
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
		///类型
		/// </summary>
		[SerializeField]
		private int _mType;
		public  int mType { get { return _mType; } }

		/// <summary>
		///碎片数量
		/// </summary>
		[SerializeField]
		private int _mDebris;
		public  int mDebris { get { return _mDebris; } }

		/// <summary>
		///兑换券数量
		/// </summary>
		[SerializeField]
		private int _mCoin;
		public  int mCoin { get { return _mCoin; } }

		/// <summary>
		///测试数据
		/// </summary>
		[SerializeField]
		private int _mTxt;
		public  int mTxt { get { return _mTxt; } }


		public ShopCfgEntry()
		{
		}

#if UNITY_EDITOR
		public ShopCfgEntry(List<List<string>> sheet, int row, int column)
		{
			int.TryParse(sheet[row][column++], out _mId);
			_mName = sheet[row][column++] ?? "";
			int.TryParse(sheet[row][column++], out _mType);
			int.TryParse(sheet[row][column++], out _mDebris);
			int.TryParse(sheet[row][column++], out _mCoin);
			int.TryParse(sheet[row][column++], out _mTxt);
		}
#endif
		public void ChangeLocalProperties(ShopCfgLocalBaseEntry localEntry)
		{
		}

		partial void OnEntryLoadCustomized();

		public override void OnEntryLoad()
		{
			OnEntryLoadCustomized();
		}
	}

	[Serializable]
	public abstract class ShopCfgLocalBaseEntry : EERowData
	{
	}

	[PreferBinarySerialization]
	[Serializable]
	public partial class ShopCfg : CfgBase
	{
		[SerializeField]
		private List<ShopCfgEntry> _entryList = new List<ShopCfgEntry>();

		public IReadOnlyList<ShopCfgEntry> EntryList => _entryList;

		public override void AddEntry(EERowData data)
		{
			_entryList.Add(data as ShopCfgEntry);
		}

		public override int GetEntryCount()
		{
			return _entryList.Count;
		}

		public ShopCfgEntry GetEntryByIndex(int index)
		{
			return _entryList[index] as ShopCfgEntry;

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
			ShopCfgLocalBase so = null;
			var localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + "/" + cfgName);
			if (localCfgObject != null)
			{
				so = localCfgObject as ShopCfgLocalBase;
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
	public abstract class ShopCfgLocalBase : LocalizationCfgBase
	{
		public abstract ShopCfgLocalBaseEntry GetEntryByIndex(int index);
	}
}
#pragma warning restore 0649