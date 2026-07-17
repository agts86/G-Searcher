import type { webhook } from '@line/bot-sdk';
import type { LineReplyService } from './line-reply.service.js';
import type { WebhookRepository } from './webhook.repository.js';
import type { LocalEventResult, PersistedMeta } from './webhook.types.js';

function isLocationMeta(meta: PersistedMeta): meta is Extract<PersistedMeta, { lat: number }> {
  return 'lat' in meta;
}

/** 既存.NET側 WebhookService と同じ振る舞い（イベント処理・永続化） */
export class WebhookService {
  constructor(
    private readonly lineReplyService: LineReplyService,
    private readonly repo: WebhookRepository,
  ) {}

  /** /local 用: 同期的に処理し、DB保存した上でreply/meta/isReplySucceededの一覧を返す */
  async processSync(webhookBody: webhook.CallbackRequest, genreCode: string | undefined): Promise<LocalEventResult[]> {
    const results = await this.processEvents(webhookBody, genreCode);
    await this.persistMetas(results.map((r) => r.meta));
    return results;
  }

  private async processEvents(
    webhookBody: webhook.CallbackRequest,
    genreCode: string | undefined,
  ): Promise<LocalEventResult[]> {
    const results: LocalEventResult[] = [];
    for (const event of webhookBody.events) {
      const result = await this.lineReplyService.processEvent(event, genreCode);
      if (result) {
        results.push(result);
      }
    }
    return results;
  }

  private async persistMetas(metas: PersistedMeta[]): Promise<void> {
    for (const meta of metas) {
      if (isLocationMeta(meta)) {
        await this.repo.createGourmetLocationLog(meta);
      } else {
        await this.repo.createGourmetWordLog(meta);
      }
    }
  }
}
