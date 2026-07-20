import { useQuery } from "@tanstack/react-query";
import type { UseQueryResult } from "@tanstack/react-query";
import { managedClient } from "@/lib/api/managed-client";
import { QueryKeys } from "@/lib/constants/queryKeys";
import type { GourmetLocationLog } from "@/types/api";

export function useGourmetLocation(): UseQueryResult<GourmetLocationLog[]> {
	return useQuery({
		queryKey: QueryKeys.managed.gourmetLocation,
		queryFn: () => managedClient.getGourmetLocation(),
	});
}
