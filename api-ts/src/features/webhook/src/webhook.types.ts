import type { webhook } from '@line/bot-sdk';

export interface LocalJob {
  id: string;
  webhookBody: webhook.CallbackRequest;
  genreCode: string | undefined;
}

export type GourmetLogEntry =
  | { type: 'location'; lat: number; lng: number }
  | { type: 'word'; text: string | null };

export interface LocalEventResult {
  meta: GourmetLogEntry;
  isReplySucceeded: boolean;
}

export interface LocalJobResult {
  job: LocalJob;
  results: LocalEventResult[];
  isSuccess: boolean;
  errorMessage: string | null;
}
