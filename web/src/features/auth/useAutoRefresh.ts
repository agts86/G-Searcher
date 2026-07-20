"use client";

import { useEffect, useRef } from "react";
import type { MutableRefObject } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { authClient } from "@/lib/api/auth-client";
import { QueryKeys } from "@/lib/constants/queryKeys";
import {
	clearAccessTokenExpiresAt,
	getAccessTokenExpiresAt,
	setAccessTokenExpiresAt,
} from "@/lib/auth/session";

const REFRESH_BEFORE_EXPIRE_MS = 5 * 60 * 1000;
const MIN_REFRESH_DELAY_MS = 5 * 1000;

function getRefreshDelay(expiresAt: string): number {
	const refreshAt = new Date(expiresAt).getTime() - REFRESH_BEFORE_EXPIRE_MS;
	return Math.max(refreshAt - Date.now(), MIN_REFRESH_DELAY_MS);
}

function clearRefreshTimer(timerRef: MutableRefObject<ReturnType<typeof setTimeout> | null>): void {
	if (timerRef.current === null) return;
	clearTimeout(timerRef.current);
	timerRef.current = null;
}

function scheduleRefreshTimer(
	timerRef: MutableRefObject<ReturnType<typeof setTimeout> | null>,
	expiresAt: string,
	onRefresh: () => Promise<void>,
): void {
	clearRefreshTimer(timerRef);
	timerRef.current = setTimeout(() => {
		void onRefresh();
	}, getRefreshDelay(expiresAt));
}

export function useAutoRefresh(): void {
	const queryClient = useQueryClient();
	const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

	useEffect(() => {
		let disposed = false;

		const refreshAndReschedule = async (): Promise<void> => {
			try {
				const result = await authClient.refresh();
				if (disposed) return;

				setAccessTokenExpiresAt(result.expiresAt);
				void queryClient.invalidateQueries({ queryKey: QueryKeys.auth.me });
				scheduleRefreshTimer(timerRef, result.expiresAt, refreshAndReschedule);
			} catch {
				clearAccessTokenExpiresAt();
			}
		};

		const initialize = (): void => {
			const expiresAt = getAccessTokenExpiresAt();
			if (!expiresAt) return;
			scheduleRefreshTimer(timerRef, expiresAt, refreshAndReschedule);
		};

		const dispose = (): void => {
			disposed = true;
			clearRefreshTimer(timerRef);
		};

		void initialize();

		return dispose;
	}, [queryClient]);
}
