using System;

namespace PVM.Utils
{
    public class EventKeys
    {
        public const string CUS_EXCHANGE_OPEN = "cus_exchange_open";//打开提现
        public const string CUS_EXCHANGE_BUTTON = "cus_exchange_button";//提现各个档位提现次数
        public const string CUS_MEIRIJIANGLI_OPEN = "cus_meirijiangli_open";//打开每日奖励
        public const string CUS_MEIRIJIANGLI_TIXAIN = "cus_meirijiangli_tixain";//每日奖励-提现
        public const string CUS_MEIRIJIANGLI_CHAKANMINGDAN = "cus_meirijiangli_chakanmingdan";//每日奖励-查看奖励名单
        public const string CUS_MEIRIJIANGLI_CLOSE = "cus_meirijiangli_close";//关闭每日奖励
        public const string CUS_XIANSHIHUODONG_OPEN = "cus_xianshihuodong_open";//打开限时活动
        public const string CUS_XIANSHIHUODONG_BAOMING = "cus_xianshihuodong_baoming";//限时活动-点击报名
        public const string CUS_XIANSHIHUODONG_CLOSE = "cus_xianshihuodong_close";//限时活动-关闭界面
        public const string CUS_QIANYUANTIXIAN_OPEN = "cus_qianyuantixian_open";//点击千元提现
        public const string CUS_CUNQIANGUAN_OPEN = "cus_cunqianguan_open";//点击存钱罐
        public const string CUS_CUNQIANGUAN_TIQU = "cus_cunqianguan_tiqu";//存钱罐-提取
        public const string CUS_CUNQIANGUAN_CLOSE = "cus_cunqianguan_close";//存钱罐-关闭
        public const string CUS_JUESETUJIAN_OPEN = "cus_juesetujian_open";//打开角色图鉴
        public const string CUS_JUESETUJIAN_JUESEHONGBAO = "cus_juesetujian_juesehongbao";//角色图鉴-打开角色红包
        public const string CUS_JUESETUJIAN_CLOSE = "cus_juesetujian_close";//图鉴-关闭图鉴
        public const string CUS_SIGNIN = "cus_signin";//打开签到
        public const string CUS_SIGNIN_OPEN = "cus_signin_open";//点击签到
        public const string CUS_SIGNIN_CLOSE = "cus_signin_close";//关闭签到
        public const string CUS_XINSHOULIBAO_OPEN = "cus_xinshoulibao_open";//打开新手礼包
        public const string CUS_XINSHOULIBAO_WANCHENG = "cus_xinshoulibao_wancheng";//新手礼包-完成对应任务
        public const string CUS_XINSHOULIBAO_LINGQU = "cus_xinshoulibao_lingqu";//新手礼包-领取奖励
        public const string CUS_XINSHOULIBAO_TIXIAN = "cus_xinshoulibao_tixian";//新手礼包-点击提现
        public const string CUS_XINSHOULIBAO_CLOSE = "cus_xinshoulibao_close";//新手礼包-关闭
        public const string CUS_CHUANGGUANHONGBAO_OPEN = "cus_chuangguanhongbao_open";//闯关红包-打开
        public const string CUS_CHUANGGUANHONGBAO_CLOSE = "cus_chuangguanhongbao_close";//闯关红包-关闭
        public const string CUS_OLINE_OPEN = "cus_oline_open";//在线礼包-打开
        public const string CUS_OLINE_CLOSE = "cus_oline_close";//在线礼包-关闭
        public const string CUS_RENWU_OPEN = "cus_renwu_open";//每日任务-打开
        public const string CUS_RENWU_LINGJIANG = "cus_renwu_lingjiang";//每日任务-领取奖励
        public const string CUS_KUAISUHECHENG_OPEN = "cus_kuaisuhecheng_open";//快速合成-点击
        public const string CUS_KUAISUHECHENG_BUCHONG = "cus_kuaisuhecheng_buchong";//快速合成-触发补充次数
        public const string CUS_KUAISUCHANCHONG_OPEN = "cus_kuaisuchanchong_open";//快速产宠-点击
        public const string CUS_KUAISUCHANCHONG_BUZU = "cus_kuaisuchanchong_buzu";//快速产宠-触发上限
        public const string CUS_HEZUO_OPEN = "cus_hezuo_open";//合作赚钱-点击
        public const string CUS_KONGTOU_OPEN = "cus_kongtou_open";//空投红包-点击
        public const string CUS_KONGTOU_CLOSE = "cus_kongtou_close";//空投红包-关闭
        public const string CUS_KONGTOU_FREE = "cus_kongtou_free";//空投红包-非rv红包领取
        public const string CUS_TUBIAO_OPEN = "cus_tubiao_open";//功能图标点击
        public const string CUS_TILIBUZU_OPEN = "cus_tilibuzu_open";//体力不足弹窗-打开
        public const string CUS_TILIBUZU_CLOSE = "cus_tilibuzu_close";//体力不足弹窗-关闭
        public const string CUS_PASS_OPEN = "cus_pass_open";//通关弹窗-显示
        public const string CUS_PASS_CLOSE = "cus_pass_close";//通关红包-关闭
        public const string CUS_LOSS_OPEN = "cus_loss_open";//过关失败-显示
        public const string CUS_LOSS_CLOSE = "cus_loss_close";//过关失败-立即前往
        public const string CUS_BOSS_OPEN = "cus_boss_open";//boss红包-展示
        public const string CUS_BOSS_CLOSE = "cus_boss_close";//boss红包-关闭
        public const string CUS_YAOQING_TIJIAO = "cus_yaoqing_tijiao";//邀请码提交
        public const string CUS_YAOQING_CLOSE = "cus_yaoqing_close";//关闭邀请码界面
        public const string CUS_HEZUO_TOUXAING_YAOQING = "cus_hezuo_touxaing_yaoqing";//点击合作赚钱头像邀请
        public const string CUS_HEZUO_YAOQING = "cus_hezuo_yaoqing";//点击立即邀请
        public const string CUS_HEZUO_FUZHI = "cus_hezuo_fuzhi";//复制邀请码
        public const string CUS_GONGXIAN_TIXIAN = "cus_gongxian_tixian_";//贡献金提现次数
        public const string CUS_GONGXIANXIANGQING = "cus_gongxainxiangqing";//贡献详情
        public const string CUS_ZHUANQIANGONGLUE = "cus_zhuanqiangonglue";//赚钱攻略
        public const string CUS_TUIGUANGDASAI_OPEN = "cus_tuiguangdasai_open";//点击推广大赛
        public const string CUS_TUIGUANGDASAI_WEEK = "cus_tuiguangdasai_week";//点击推广大赛每周大赛
        public const string CUS_TUIGUANGDASAI_MONTH = "cus_tuiguangdasai_month";//点击推广大赛每月大赛
        public const string CUS_FENXIANG_WEIXIN = "cus_fenxiang_weixin";//分享 微信
        public const string CUS_FENXIANG_ERWEIMA = "cus_fenxiang_erweima";//分享 二维码
        public const string CUS_FENXIANG_LIANJIE = "cus_fenxiang_lianjie";//分享链接
        public const string CUS_HUJIAOZHIYUAN_DIANJI = "cus_hujiaozhiyuan_dianji";//呼叫支援打点
        public const string CUS_MUZHUANG_DIANJI = "cus_muzhuang_dianji";//升级木桩点击
        
        /// <summary>
        /// 合成次数
        /// </summary>
        /// <param name="mergeCount"></param>
        /// <returns></returns>
        public static string GetCUS_MERGE_(int mergeCount)
        {
            return $"cus_merge_{mergeCount}";
        }
        /// <summary>
        /// 玩家等级打点
        /// </summary>
        /// <param name="petLv"></param>
        /// <returns></returns>
        public static string GetCUS_PET_LEVEL(int petLv)
        {
            return $"cus_pet_level_{petLv}";
        }
        /// <summary>
        /// 通关关卡
        /// </summary>
        /// <param name="LvPass"></param>
        /// <returns></returns>
        public static string GetCUS_LEVEL_(int LvPass)
        {
            return $"cus_level_{LvPass}";
        }
        
        /// <summary>
        /// 底座解锁个数
        /// </summary>
        /// <param name="stakeUnlockCount"></param>
        /// <returns></returns>
        public static string GetCUS_STAKE_(int stakeUnlockCount)
        {
            return $"cus_stake_{stakeUnlockCount}";
        }
        
        /// <summary>
        /// 用户第一次提现时玩家等级
        /// </summary>
        /// <param name="firstWithdrawLv"></param>
        /// <returns></returns>
        public static string GetCUS_WITHDRAW_LEVEL_(int firstWithdrawLv)
        {
            return $"cus_withdraw_level_{firstWithdrawLv}";
        }
        
        /// <summary>
        /// 用户等级低于X级点击提现用户
        /// </summary>
        /// <param name="withdrawClickLv"></param>
        /// <returns></returns>
        public static string GetCUS_MONEY_CLICK_(int withdrawClickLv)
        {
            return $"cus_money_click_{withdrawClickLv}";
        }
        
        public const string CUS_CUS_WITHDRAW_2 = "cus_cus_withdraw_2"; //单日提现次数大于等于2次的用户数
        /// <summary>
        /// 点击提现时身上状态id
        /// </summary>
        /// <param name="buffStr"></param>
        /// <returns></returns>
        public static string GetCUS_WITHDRAW_BUFF_(string buffStr)
        {
            return $"cus_withdraw_buff_{buffStr}";
        }
        /// <summary>
        /// 玩家触发状态id
        /// </summary>
        /// <param name="buffId"></param>
        /// <returns></returns>
        public static string GetCUS_BUFF_(int buffId)
        {
            return $"cus_buff_{buffId}";
        }
        public const string CUS_LOADING_SUCCESS = "cus_loading_success"; //loading完成的用户
        public const string CUS_AD_CLICK_RV_MEIRIJIANGLI = "cus_ad_click_rv_meirijiangli"; //每日奖励-观看rv
        public const string CUS_AD_CLICK_RV_ONLINE = "cus_ad_click_rv_online"; //在线礼包-看rv
        public const string CUS_AD_CLICK_RV_KUAISUHECHENG = "cus_ad_click_rv_kuaisuhecheng"; //快速合成-看rv
        public const string CUS_AD_CLICK_RV_KONGTOU = "cus_ad_click_rv_kongtou"; //空投红包-看rv
        public const string CUS_AD_CLICK_RV_TILIBUZU = "cus_ad_click_rv_tilibuzu"; //体力不足弹窗-看rv
        public const string CUS_AD_CLICK_RV_PASS = "cus_ad_click_rv_pass"; //通关红包-看rv
        public const string CUS_AD_CLICK_RV_BOSS = "cus_ad_click_rv_boss"; //boss红包-点击看rv
        public const string CUS_AD_CLICK_RV_XIANSHIHUODONG = "cus_ad_click_rv_xianshihuodong"; //限时活动-看rv
        public const string CUS_AD_CLICK_RV_CHUANGGUANHONGBAO = "cus_ad_click_rv_chuangguanhongbao"; //闯关红包-看rv


    }


    /// <summary>
    ///     用户的ID参数
    /// </summary>
    [Serializable]
    public class Uid_Cash_LevelArgs
    {
        /// <summary>
        /// 用户的ID，红包金额，猫等级（最高等级）
        /// </summary>
        public string Uid_Cash_Level;

        public Uid_Cash_LevelArgs(int uid, int cash, int starLevel)
        {
            Uid_Cash_Level = $"{uid}:{cash}:{starLevel}";
        }
    }

    /// <summary>
    ///     常见广告参数
    /// </summary>
    [Serializable]
    public class CommonAdArgs
    {
        /// <summary>
        ///     广告位名称
        /// </summary>
        public string af_adrev_ad_type;

        public string af_currency = "CNY";

        public CommonAdArgs(string adType)
        {
            af_adrev_ad_type = adType;
        }
    }

    /// <summary>
    ///     用户的ID参数
    /// </summary>
    [Serializable]
    public class UidArgs
    {
        /// <summary>
        ///     用户的ID，用户的现金数量
        /// </summary>
        public string Uid;

        public UidArgs(string uid)
        {
            Uid = uid;
        }
    }

    /// <summary>
    ///     用户的ID，用户的现金数量参数
    /// </summary>
    [Serializable]
    public class Uid_CashArgs
    {
        /// <summary>
        ///     用户的ID，用户的现金数量
        /// </summary>
        public string Uid_Cash;

        public Uid_CashArgs(string uid, string cash)
        {
            Uid_Cash = uid + "_" + cash;
        }
    }
}