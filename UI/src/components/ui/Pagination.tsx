type Props = {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
};

export function Pagination({ currentPage, totalPages, onPageChange }: Props): React.ReactElement {
  return (
    <div className="flex items-center justify-center gap-4 py-4">
      <button
        onClick={() => { onPageChange(currentPage - 1); }}
        disabled={currentPage <= 1}
        className="rounded border border-slate-300 px-3 py-1 text-sm hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
      >
        ← 前へ
      </button>
      <span className="text-sm text-slate-600">
        {currentPage} / {totalPages}
      </span>
      <button
        onClick={() => { onPageChange(currentPage + 1); }}
        disabled={currentPage >= totalPages}
        className="rounded border border-slate-300 px-3 py-1 text-sm hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
      >
        次へ →
      </button>
    </div>
  );
}
