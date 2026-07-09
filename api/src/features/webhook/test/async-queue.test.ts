import { describe, expect, it } from 'vitest';
import { AsyncQueue } from '../src/async-queue.js';

describe('AsyncQueue', () => {
  it('enqueue後にdequeueすると同じ値を取得できる', async () => {
    const queue = new AsyncQueue<number>();
    queue.enqueue(1);

    const result = await queue.dequeue();

    expect(result).toBe(1);
  });

  it('複数回enqueueするとFIFO順でdequeueできる', async () => {
    const queue = new AsyncQueue<string>();
    queue.enqueue('a');
    queue.enqueue('b');
    queue.enqueue('c');

    expect(await queue.dequeue()).toBe('a');
    expect(await queue.dequeue()).toBe('b');
    expect(await queue.dequeue()).toBe('c');
  });

  it('空の状態でdequeueすると、後からenqueueされるまで待機する', async () => {
    const queue = new AsyncQueue<number>();

    const pending = queue.dequeue();
    queue.enqueue(42);

    expect(await pending).toBe(42);
  });

  it('複数の待機者がいる場合もFIFOで解決する', async () => {
    const queue = new AsyncQueue<number>();
    const first = queue.dequeue();
    const second = queue.dequeue();

    queue.enqueue(1);
    queue.enqueue(2);

    expect(await first).toBe(1);
    expect(await second).toBe(2);
  });
});
