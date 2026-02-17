'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useMe } from '@/features/auth/useAuth';
import { LoadingSpinner } from '@/components/ui/LoadingSpinner';
import { getAccessTokenExpiresAt } from '@/lib/auth/session';

type Props = {
  children: React.ReactNode;
};

export function AuthGuard({ children }: Props): React.ReactElement {
  const router = useRouter();
  const hasToken = getAccessTokenExpiresAt() !== null;
  const { isLoading, error, data } = useMe({ enabled: hasToken });

  useEffect(() => {
    if (!hasToken || error) {
      router.push('/login');
    }
  }, [hasToken, error, router]);

  if (!hasToken) return <LoadingSpinner />;
  if (isLoading) return <LoadingSpinner />;
  if (error || !data) return <LoadingSpinner />;

  return <>{children}</>;
}
