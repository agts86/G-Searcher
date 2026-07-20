"use client";

import { useMemo } from "react";
import { useJobLog } from "./useJobLog";
import { FilterBar } from "@/components/logs/FilterBar";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";
import { ErrorMessage } from "@/components/ui/ErrorMessage";
import { Pagination } from "@/components/ui/Pagination";
import { Badge } from "@/components/ui/Badge";
import { useFilter } from "@/hooks/useFilter";
import { usePagination } from "@/hooks/usePagination";
import { formatDateTime, isInDateRange } from "@/lib/utils/date";
import type { JobLog } from "@/types/api";

type TableProps = { pagedItems: JobLog[]; currentPage: number };

function JobTable({ pagedItems, currentPage }: TableProps): React.ReactElement {
	return (
		<div className="overflow-x-auto rounded-lg border border-slate-200 bg-white">
			<table className="min-w-full text-sm">
				<thead className="border-b bg-slate-50 text-left">
					<tr>
						<th className="px-4 py-3 font-medium text-slate-600">#</th>
						<th className="px-4 py-3 font-medium text-slate-600">結果</th>
						<th className="px-4 py-3 font-medium text-slate-600">内容</th>
						<th className="px-4 py-3 font-medium text-slate-600">備考</th>
						<th className="px-4 py-3 font-medium text-slate-600">日時</th>
					</tr>
				</thead>
				<tbody>
					{pagedItems.length === 0 ? (
						<tr>
							<td colSpan={5} className="py-8 text-center text-slate-400">
								データがありません
							</td>
						</tr>
					) : (
						pagedItems.map((log, i) => (
							<tr key={log.id} className="border-b even:bg-gray-50">
								<td className="px-4 py-3 text-slate-500">{(currentPage - 1) * 20 + i + 1}</td>
								<td className="px-4 py-3">
									<Badge variant={log.isSuccess ? "success" : "error"}>
										{log.isSuccess ? "成功" : "失敗"}
									</Badge>
								</td>
								<td className="px-4 py-3 break-all">{log.contents ?? "-"}</td>
								<td className="px-4 py-3 text-slate-500">{log.info ?? "-"}</td>
								<td className="px-4 py-3 text-slate-500">{formatDateTime(log.createdAt)}</td>
							</tr>
						))
					)}
				</tbody>
			</table>
		</div>
	);
}

export function JobLogPage(): React.ReactElement {
	const { data: logs, isLoading, error, refetch } = useJobLog();
	const { filter, appliedFilter, setFilter, applyFilter } = useFilter();

	const filtered = useMemo(
		() =>
			(logs ?? []).filter((l) => {
				const kw = appliedFilter.keyword;
				const matchKeyword = !kw || (l.contents ?? "").includes(kw) || (l.info ?? "").includes(kw);
				return matchKeyword && isInDateRange(l.createdAt, appliedFilter.from, appliedFilter.to);
			}),
		[logs, appliedFilter],
	);

	const { currentPage, totalPages, pagedItems, setPage, resetPage } = usePagination(filtered);
	const handleSearch = (): void => {
		applyFilter();
		resetPage();
	};

	if (isLoading) return <LoadingSpinner />;
	if (error)
		return (
			<ErrorMessage
				onRetry={() => {
					void refetch();
				}}
			/>
		);

	return (
		<div className="flex flex-col gap-4">
			<h2 className="text-lg font-semibold text-slate-800">ジョブログ</h2>
			<FilterBar
				values={filter}
				onChange={setFilter}
				onSearch={handleSearch}
				totalCount={filtered.length}
			/>
			<JobTable pagedItems={pagedItems} currentPage={currentPage} />
			<Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={setPage} />
		</div>
	);
}
