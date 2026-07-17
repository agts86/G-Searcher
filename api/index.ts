import { handle } from 'hono/vercel';
import { createApp } from '@api/host';

const app = createApp();

export default handle(app);
