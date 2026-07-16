import { serve } from '@hono/node-server';
import { createApp } from './app.js';

const port = Number(process.env.PORT ?? '3000');
const app = createApp();

serve({ fetch: app.fetch, port }, (info) => {
  // eslint-disable-next-line no-console
  console.log(`api host listening on http://localhost:${info.port}`);
});
