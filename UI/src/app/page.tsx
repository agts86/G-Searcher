'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';

export default function RootPage(): React.ReactElement {
  const router = useRouter();

  useEffect(() => {
    router.replace('/dashboard/gourmet/location');
  }, [router]);

  return <></>;
}
