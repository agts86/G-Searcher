import { startPGliteServer, stopPGliteServer } from './pglite-server.js';

export default async function setup(): Promise<() => Promise<void>> {
  await startPGliteServer();
  return async function teardown() {
    await stopPGliteServer();
  };
}
