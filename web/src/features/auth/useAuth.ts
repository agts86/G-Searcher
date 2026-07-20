import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { UseMutationResult, UseQueryResult } from "@tanstack/react-query";
import { authClient } from "@/lib/api/auth-client";
import { QueryKeys } from "@/lib/constants/queryKeys";
import type { LoginRequest, LoginResponse, MeResponse } from "@/types/api";
import { clearAccessTokenExpiresAt, setAccessTokenExpiresAt } from "@/lib/auth/session";

type UseMeOptions = {
	enabled?: boolean;
};

export function useMe(options?: UseMeOptions): UseQueryResult<MeResponse> {
	return useQuery({
		queryKey: QueryKeys.auth.me,
		queryFn: () => authClient.me(),
		retry: false,
		enabled: options?.enabled ?? true,
	});
}

export function useLogin(): UseMutationResult<LoginResponse, Error, LoginRequest> {
	const queryClient = useQueryClient();
	return useMutation({
		mutationFn: (req: LoginRequest) => authClient.login(req),
		onSuccess: (result) => {
			setAccessTokenExpiresAt(result.expiresAt);
			void queryClient.invalidateQueries({ queryKey: QueryKeys.auth.me });
		},
	});
}

export function useRefresh(): UseMutationResult<LoginResponse, Error, void> {
	const queryClient = useQueryClient();
	return useMutation({
		mutationFn: () => authClient.refresh(),
		onSuccess: (result) => {
			setAccessTokenExpiresAt(result.expiresAt);
			void queryClient.invalidateQueries({ queryKey: QueryKeys.auth.me });
		},
		onError: () => {
			clearAccessTokenExpiresAt();
		},
	});
}

export function useLogout(): UseMutationResult<void, Error, void> {
	const queryClient = useQueryClient();
	return useMutation({
		mutationFn: () => authClient.logout(),
		onSuccess: () => {
			clearAccessTokenExpiresAt();
			queryClient.clear();
		},
	});
}
