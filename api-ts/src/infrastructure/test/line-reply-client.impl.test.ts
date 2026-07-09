import { describe, expect, it, vi, beforeEach } from 'vitest';

const replyMessageMock = vi.fn();

vi.mock('@line/bot-sdk', () => ({
  LineBotClient: {
    fromChannelAccessToken: vi.fn(() => ({ replyMessage: replyMessageMock })),
  },
}));

const { LineReplyClientImpl } = await import('../src/line-reply-client.impl.js');

beforeEach(() => {
  replyMessageMock.mockReset();
});

describe('LineReplyClientImpl.replyCarousel', () => {
  it('カルーセルテンプレートを構築してreplyMessageを呼び、成功時はtrueを返す', async () => {
    replyMessageMock.mockResolvedValue(undefined);
    const client = new LineReplyClientImpl('token-1');

    const result = await client.replyCarousel('reply-token-1', [
      { title: '店A', text: '住所A', detailUrl: 'https://example.com/a' },
    ]);

    expect(result).toBe(true);
    expect(replyMessageMock).toHaveBeenCalledWith({
      replyToken: 'reply-token-1',
      messages: [
        {
          type: 'template',
          altText: '検索結果',
          template: {
            type: 'carousel',
            columns: [
              {
                title: '店A',
                text: '住所A',
                actions: [{ type: 'uri', label: '詳細を見る', uri: 'https://example.com/a' }],
              },
            ],
          },
        },
      ],
    });
  });

  it('replyMessageが失敗したらfalseを返す', async () => {
    replyMessageMock.mockRejectedValue(new Error('line api down'));
    const client = new LineReplyClientImpl('token-1');

    const result = await client.replyCarousel('reply-token-1', []);

    expect(result).toBe(false);
  });
});

describe('LineReplyClientImpl.replyText', () => {
  it('テキストメッセージを構築してreplyMessageを呼び、成功時はtrueを返す', async () => {
    replyMessageMock.mockResolvedValue(undefined);
    const client = new LineReplyClientImpl('token-1');

    const result = await client.replyText('reply-token-1', 'こんにちは');

    expect(result).toBe(true);
    expect(replyMessageMock).toHaveBeenCalledWith({
      replyToken: 'reply-token-1',
      messages: [{ type: 'text', text: 'こんにちは' }],
    });
  });

  it('replyMessageが失敗したらfalseを返す', async () => {
    replyMessageMock.mockRejectedValue(new Error('line api down'));
    const client = new LineReplyClientImpl('token-1');

    const result = await client.replyText('reply-token-1', 'こんにちは');

    expect(result).toBe(false);
  });
});
