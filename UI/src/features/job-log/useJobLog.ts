import { useQuery } from '@tanstack/react-query';
import type { UseQueryResult } from '@tanstack/react-query';
import { managedClient } from '@/lib/api/managed-client';
import { QueryKeys } from '@/lib/constants/queryKeys';
import type { JobLog } from '@/types/api';

export function useJobLog(): UseQueryResult<JobLog[]> {
  return useQuery({
    queryKey: QueryKeys.managed.jobLog,
    queryFn: () => managedClient.getJobLog(),
  });
}
