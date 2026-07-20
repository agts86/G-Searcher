import { useQuery } from "@tanstack/react-query";
import type { UseQueryResult } from "@tanstack/react-query";
import { managedClient } from "@/lib/api/managed-client";
import { QueryKeys } from "@/lib/constants/queryKeys";
import type { ErrorLog } from "@/types/api";

export function useErrorLog(): UseQueryResult<ErrorLog[]> {
	return useQuery({
		queryKey: QueryKeys.managed.errorLog,
		queryFn: () => managedClient.getErrorLog(),
	});
}
