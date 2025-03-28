namespace LineWebHookAPI.Models.DB.Repositories;

/// <summary>
/// ミドルウェア用リポジトリ
/// </summary>
public class MiddleWareRepository(LineWebHookContext dbContext) : Repository(dbContext){}
