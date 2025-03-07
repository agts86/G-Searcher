using System.Text.Json.Serialization;
using G_Searcher.Models.Exceptions;

namespace G_Searcher.Models.Dto.Search;

/// <summary>
/// ホットペッパーAPIエラーレスポンスのDTO
/// </summary>
public abstract class HotPepperErrorResponseDto
{
     /// <summary>
    /// エラー詳細
    /// </summary>
    [JsonPropertyName("error")]
    public ErrorInfo Error { get; set; }

    /// <summary>
    /// エラー詳細
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }

    /// <summary>
    /// 実はエラーでないかチェック
    /// </summary>
    public void CheckResult()
    {
        if(Error is null) return;
        // 詳細は以下のリンクを参照
        // https://webservice.recruit.co.jp/hotpepper/reference.html

        if(Error.Code == "3000") 
            throw new BadRequestException(new ResponseError(Error.Message));
        throw new Exception(Error.Message);
    }
}