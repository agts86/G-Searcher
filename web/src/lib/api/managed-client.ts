import type { ErrorLog, GourmetLocationLog, GourmetWordLog, JobLog } from "@/types/api";
import { apiClient } from "./client";

export const managedClient = {
	getGourmetLocation: (): Promise<GourmetLocationLog[]> =>
		apiClient.get<GourmetLocationLog[]>("/managed/gourmet/location"),

	getGourmetWord: (): Promise<GourmetWordLog[]> =>
		apiClient.get<GourmetWordLog[]>("/managed/gourmet/text"),

	getErrorLog: (): Promise<ErrorLog[]> => apiClient.get<ErrorLog[]>("/managed/error-log"),

	getJobLog: (): Promise<JobLog[]> => apiClient.get<JobLog[]>("/managed/job-log"),
};
