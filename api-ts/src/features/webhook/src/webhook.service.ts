import { randomUUID } from 'node:crypto';
import type { webhook } from '@line/bot-sdk';
import type { LineReplyService } from './line-reply.service.js';
import type { WebhookRepository } from './webhook.repository.js';
import type { GourmetLogEntry, LocalEventResult, LocalJob, LocalJobResult } from './webhook.types.js';

function toErrorMessage(err: unknown): string {
  return err instanceof Error ? err.message : String(err);
}

/** 既存.NET側 WebhookService と同じ振る舞い（ジョブの生成・処理・永続化） */
export class WebhookService {
  constructor(
    private readonly lineReplyService: LineReplyService,
    private readonly repo: WebhookRepository,
  ) {}

  /** /local/accept 用: ジョブを生成するのみ（DB書込・YOLP呼び出しは一切しない） */
  enqueueLocalJob(webhookBody: webhook.CallbackRequest, genreCode: string | undefined): LocalJob {
    return { id: randomUUID(), webhookBody, genreCode };
  }

  /** キューワーカーから呼ばれる: ジョブ内の全イベントを処理し、結果をまとめる（永続化はしない） */
  async processJob(job: LocalJob): Promise<LocalJobResult> {
    try {
      const results = await this.processEvents(job.webhookBody, job.genreCode);
      return { job, results, isSuccess: true, errorMessage: null };
    } catch (err) {
      return { job, results: [], isSuccess: false, errorMessage: toErrorMessage(err) };
    }
  }

  /** キューワーカーから呼ばれる: ジョブ結果をDBへ永続化する（Gourmet*Log + JobLog） */
  async persistJobResult(result: LocalJobResult): Promise<void> {
    await this.persistMetas(result.results.map((r) => r.meta));
    await this.repo.createJobLog({
      id: result.job.id,
      isSuccess: result.isSuccess,
      contents: JSON.stringify(result.job.webhookBody),
      info: result.errorMessage,
    });
  }

  /** /local 用: 同期的に処理し、DB保存したmetaの配列を返す（JobLogは保存しない） */
  async processSync(webhookBody: webhook.CallbackRequest, genreCode: string | undefined): Promise<GourmetLogEntry[]> {
    const results = await this.processEvents(webhookBody, genreCode);
    const metas = results.map((r) => r.meta);
    await this.persistMetas(metas);
    return metas;
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

  private async persistMetas(metas: GourmetLogEntry[]): Promise<void> {
    for (const meta of metas) {
      if (meta.type === 'location') {
        await this.repo.createGourmetLocationLog({ lat: meta.lat, lng: meta.lng });
      } else {
        await this.repo.createGourmetWordLog({ text: meta.text });
      }
    }
  }
}
