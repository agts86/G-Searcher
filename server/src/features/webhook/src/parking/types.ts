import type { Reply } from '../common.types.js';

/** バイク駐車場検索版のイベント結果（DB永続化しないためmetaは持たない） */
export interface ParkingEventResult {
  reply: Reply;
  isReplySucceeded: boolean;
}
