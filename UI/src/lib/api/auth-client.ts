import type { LoginRequest, LoginResponse, MeResponse } from '@/types/api';
import { apiClient } from './client';

export const authClient = {
  login: (req: LoginRequest): Promise<LoginResponse> =>
    apiClient.post<LoginResponse>('/auth/login', req),

  logout: (): Promise<void> => apiClient.post<void>('/auth/logout', {}),

  me: (): Promise<MeResponse> => apiClient.get<MeResponse>('/auth/me'),
};
