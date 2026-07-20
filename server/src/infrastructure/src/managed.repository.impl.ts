import type { PrismaClient } from "@prisma/client";
import type {
	ManagedRepository,
	GourmetLocationLogRow,
	GourmetWordLogRow,
	ErrorLogRow,
	JobLogRow,
} from "@api/features-managed";

/** features/managed の ManagedRepository インターフェースをPrisma経由で実装する */
export class PrismaManagedRepository implements ManagedRepository {
	constructor(private readonly prisma: PrismaClient) {}

	async findGourmetLocationLogs(): Promise<GourmetLocationLogRow[]> {
		return this.prisma.gourmetLocationLog.findMany({
			orderBy: { createdAt: "desc" },
		});
	}

	async findGourmetWordLogs(): Promise<GourmetWordLogRow[]> {
		return this.prisma.gourmetWordLog.findMany({
			orderBy: { createdAt: "desc" },
		});
	}

	async findErrorLogs(): Promise<ErrorLogRow[]> {
		return this.prisma.errorLog.findMany({ orderBy: { createdAt: "desc" } });
	}

	async findJobLogs(): Promise<JobLogRow[]> {
		return this.prisma.jobLog.findMany({ orderBy: { createdAt: "desc" } });
	}
}
