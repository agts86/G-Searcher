'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useMe } from '@/features/auth/useAuth';
import { LoadingSpinner } from '@/components/ui/LoadingSpinner';

type Props = {
  children: React.ReactNode;
};

export function AuthGuard({ children }: Props): React.ReactElement {
  const router = useRouter();
  const { isLoading, error, data } = useMe();

  useEffect(() => {
    if (!error) return;
    router.push('/login');
  }, [error, router]);

  if (isLoading) return <LoadingSpinner />;
  if (error || !data) return <LoadingSpinner />;

  return <>{children}</>;
}
