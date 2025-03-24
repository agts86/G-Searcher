using System.Text.Json.Serialization;

namespace LineWebHookAPI.Constants.HotPepper;

/// <summary>
/// ジャンルコード
/// https://webservice.recruit.co.jp/hotpepper/genre/v1/?key=sample
/// </summary>
public enum GenreCode
{
    /// <summary>
    /// 居酒屋
    /// </summary>
    G001 = 1,

    /// <summary>
    /// ダイニングバー・バル
    /// </summary>
    G002 = 2,

    /// <summary>
    /// 創作料理
    /// </summary>
    G003 = 3,

    /// <summary>
    /// 和食
    /// </summary>
    G004 = 4,

    /// <summary>
    /// 洋食
    /// </summary>
    G005 = 5,

    /// <summary>
    /// イタリアン・フレンチ
    /// </summary>
    G006 = 6,

    /// <summary>
    /// 中華
    /// </summary>
    G007 = 7,

    /// <summary>
    /// 焼肉・ホルモン
    /// </summary>
    G008 = 8,

    /// <summary>
    /// アジア・エスニック料理
    /// </summary>
    G009 = 9,

    /// <summary>
    /// 各国料理
    /// </summary>
    G010 = 10,

    /// <summary>
    /// カラオケ・パーティ
    /// </summary>
    G011 = 11,

    /// <summary>
    /// バー・カクテル
    /// </summary>
    G012 = 12,

    /// <summary>
    /// ラーメン
    /// </summary>
    G013 = 13,

    /// <summary>
    /// カフェ・スイーツ
    /// </summary>
    G014 = 14,

    /// <summary>
    /// その他グルメ
    /// </summary>
    G015 = 15,

    /// <summary>
    /// お好み焼き・もんじゃ
    /// </summary>
    G016 = 16,

    /// <summary>
    /// 韓国料理
    /// </summary>
    G017 = 17
}
