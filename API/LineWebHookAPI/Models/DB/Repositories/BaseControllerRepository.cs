namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// コントローラー基底クラス用リポジトリ
/// </summary>
public class BaseControllerRepository(LineWebHookContext dbContext) : Repository(dbContext){}
