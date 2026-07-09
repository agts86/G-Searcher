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

describe('LineReplyClientImpl.send', () => {
  it('渡されたmessagesをそのままreplyMessageへ渡し、成功時はtrueを返す', async () => {
    replyMessageMock.mockResolvedValue(undefined);
    const client = new LineReplyClientImpl('token-1');
    const messages = [{ type: 'text', text: 'こんにちは' }];

    const result = await client.send('reply-token-1', messages);

    expect(result).toBe(true);
    expect(replyMessageMock).toHaveBeenCalledWith({ replyToken: 'reply-token-1', messages });
  });

  it('replyMessageが失敗したらfalseを返す', async () => {
    replyMessageMock.mockRejectedValue(new Error('line api down'));
    const client = new LineReplyClientImpl('token-1');

    const result = await client.send('reply-token-1', []);

    expect(result).toBe(false);
  });
});
