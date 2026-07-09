/**
 * 既存.NET側 System.Threading.Channels.Channel&lt;T&gt;（インメモリ、単一プロセス前提）相当の
 * 非同期FIFOキュー。DBキューではない。プロセス再起動でキュー内容は失われる。
 */
export class AsyncQueue<T> {
  private readonly items: T[] = [];
  private readonly waiters: Array<(item: T) => void> = [];

  enqueue(item: T): void {
    const waiter = this.waiters.shift();
    if (waiter) {
      waiter(item);
      return;
    }
    this.items.push(item);
  }

  async dequeue(): Promise<T> {
    const item = this.items.shift();
    if (item !== undefined) {
      return item;
    }
    return new Promise<T>((resolve) => {
      this.waiters.push(resolve);
    });
  }
}
