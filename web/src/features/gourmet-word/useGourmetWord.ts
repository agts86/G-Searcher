import { useQuery } from "@tanstack/react-query";
import type { UseQueryResult } from "@tanstack/react-query";
import { managedClient } from "@/lib/api/managed-client";
import { QueryKeys } from "@/lib/constants/queryKeys";
import type { GourmetWordLog } from "@/types/api";

export function useGourmetWord(): UseQueryResult<GourmetWordLog[]> {
	return useQuery({
		queryKey: QueryKeys.managed.gourmetWord,
		queryFn: () => managedClient.getGourmetWord(),
	});
}
