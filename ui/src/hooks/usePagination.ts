import { useState } from 'react';

const PAGE_SIZE = 20;

type UsePaginationResult<T> = {
  currentPage: number;
  totalPages: number;
  pagedItems: T[];
  setPage: (page: number) => void;
  resetPage: () => void;
};

export function usePagination<T>(items: T[]): UsePaginationResult<T> {
  const [currentPage, setCurrentPage] = useState(1);

  const totalPages = Math.max(1, Math.ceil(items.length / PAGE_SIZE));
  const safePage = Math.min(currentPage, totalPages);
  const pagedItems = items.slice((safePage - 1) * PAGE_SIZE, safePage * PAGE_SIZE);

  const setPage = (page: number): void => {
    setCurrentPage(Math.max(1, Math.min(page, totalPages)));
  };

  const resetPage = (): void => { setCurrentPage(1); };

  return { currentPage: safePage, totalPages, pagedItems, setPage, resetPage };
}
