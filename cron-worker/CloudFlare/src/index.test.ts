import { afterEach, describe, expect, it, vi } from 'vitest';

import worker from './index';

describe('scheduled', () => {
	afterEach(() => {
		vi.restoreAllMocks();
		vi.unstubAllGlobals();
	});

	it('sends health check request and does not log on 2xx', async () => {
		const fetchMock = vi.fn().mockResolvedValue(new Response('ok', { status: 200 }));
		vi.stubGlobal('fetch', fetchMock);
		const consoleLogSpy = vi.spyOn(console, 'log').mockImplementation(() => {});

		await worker.scheduled(
			{} as ScheduledEvent,
			{ TARGET_URL: 'https://line-webhook-api.azurewebsites.net/health' },
			{} as ExecutionContext,
		);

		expect(fetchMock).toHaveBeenCalledWith('https://line-webhook-api.azurewebsites.net/health', {
			method: 'GET',
			headers: { 'User-Agent': 'cf-cron-warmup' },
		});
		expect(consoleLogSpy).not.toHaveBeenCalled();
	});

	it('logs status and body when non-2xx response is returned', async () => {
		const fetchMock = vi.fn().mockResolvedValue(new Response('service unavailable', { status: 503 }));
		vi.stubGlobal('fetch', fetchMock);
		const consoleLogSpy = vi.spyOn(console, 'log').mockImplementation(() => {});

		await worker.scheduled({} as ScheduledEvent, { TARGET_URL: 'https://example.com/health' }, {} as ExecutionContext);

		expect(consoleLogSpy).toHaveBeenCalledWith('warmup failed:', 503, 'service unavailable');
	});

	it('propagates fetch errors', async () => {
		const fetchMock = vi.fn().mockRejectedValue(new Error('network error'));
		vi.stubGlobal('fetch', fetchMock);

		await expect(
			worker.scheduled({} as ScheduledEvent, { TARGET_URL: 'https://example.com/health' }, {} as ExecutionContext),
		).rejects.toThrow('network error');
	});
});
