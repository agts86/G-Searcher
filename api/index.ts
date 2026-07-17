import { handle } from '@hono/node-server/vercel';
import { createApp } from '@api/host';

const app = createApp();

export default handle(app);
