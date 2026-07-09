'use client';

import { useMemo } from 'react';
import { useErrorLog } from './useErrorLog';
import { FilterBar } from '@/components/logs/FilterBar';
import { LoadingSpinner } from '@/components/ui/LoadingSpinner';
import { ErrorMessage } from '@/components/ui/ErrorMessage';
import { Pagination } from '@/components/ui/Pagination';
import { useFilter } from '@/hooks/useFilter';
import { usePagination } from '@/hooks/usePagination';
import { formatDateTime, isInDateRange } from '@/lib/utils/date';
import type { ErrorLog } from '@/types/api';

type TableProps = { pagedItems: ErrorLog[]; currentPage: number };

function ErrorTable({ pagedItems, currentPage }: TableProps): React.ReactElement {
  return (
    <div className="overflow-x-auto rounded-lg border border-slate-200 bg-white">
      <table className="min-w-full text-sm">
        <thead className="border-b bg-slate-50 text-left">
          <tr>
            <th className="px-4 py-3 font-medium text-slate-600">#</th>
            <th className="px-4 py-3 font-medium text-slate-600">内容</th>
            <th className="px-4 py-3 font-medium text-slate-600">日時</th>
          </tr>
        </thead>
        <tbody>
          {pagedItems.length === 0 ? (
            <tr><td colSpan={3} className="py-8 text-center text-slate-400">データがありません</td></tr>
          ) : (
            pagedItems.map((log, i) => (
              <tr key={log.id} className="border-b even:bg-gray-50">
                <td className="px-4 py-3 text-slate-500">{(currentPage - 1) * 20 + i + 1}</td>
                <td className="px-4 py-3 break-all text-red-700">{log.contents}</td>
                <td className="px-4 py-3 text-slate-500">{formatDateTime(log.createdAt)}</td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}

export function ErrorLogPage(): React.ReactElement {
  const { data: logs, isLoading, error, refetch } = useErrorLog();
  const { filter, appliedFilter, setFilter, applyFilter } = useFilter();

  const filtered = useMemo(
    () => (logs ?? []).filter((l) => {
      const matchKeyword = !appliedFilter.keyword || l.contents.includes(appliedFilter.keyword);
      return matchKeyword && isInDateRange(l.createdAt, appliedFilter.from, appliedFilter.to);
    }),
    [logs, appliedFilter],
  );

  const { currentPage, totalPages, pagedItems, setPage, resetPage } = usePagination(filtered);
  const handleSearch = (): void => { applyFilter(); resetPage(); };

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage onRetry={() => { void refetch(); }} />;

  return (
    <div className="flex flex-col gap-4">
      <h2 className="text-lg font-semibold text-slate-800">エラーログ</h2>
      <FilterBar values={filter} onChange={setFilter} onSearch={handleSearch} totalCount={filtered.length} />
      <ErrorTable pagedItems={pagedItems} currentPage={currentPage} />
      <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={setPage} />
    </div>
  );
}
