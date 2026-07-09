import type {
  ManagedRepository,
  GourmetLocationLogRow,
  GourmetWordLogRow,
  ErrorLogRow,
  JobLogRow,
} from '../../src/managed.repository.js';

function sortByCreatedAtDesc<T extends { createdAt: Date }>(rows: T[]): T[] {
  return [...rows].sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime());
}

/** テスト用のインメモリManagedRepository実装。既存.NET側と同じcreatedAt降順ソートを行う */
export class InMemoryManagedRepository implements ManagedRepository {
  gourmetLocationLogs: GourmetLocationLogRow[] = [];
  gourmetWordLogs: GourmetWordLogRow[] = [];
  errorLogs: ErrorLogRow[] = [];
  jobLogs: JobLogRow[] = [];

  async findGourmetLocationLogs(): Promise<GourmetLocationLogRow[]> {
    return sortByCreatedAtDesc(this.gourmetLocationLogs);
  }

  async findGourmetWordLogs(): Promise<GourmetWordLogRow[]> {
    return sortByCreatedAtDesc(this.gourmetWordLogs);
  }

  async findErrorLogs(): Promise<ErrorLogRow[]> {
    return sortByCreatedAtDesc(this.errorLogs);
  }

  async findJobLogs(): Promise<JobLogRow[]> {
    return sortByCreatedAtDesc(this.jobLogs);
  }
}
