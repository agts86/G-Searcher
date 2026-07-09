import { useState } from 'react';
import type { FilterValues } from '@/components/logs/FilterBar';

type UseFilterResult = {
  filter: FilterValues;
  appliedFilter: FilterValues;
  setFilter: (values: FilterValues) => void;
  applyFilter: () => void;
};

const INITIAL: FilterValues = { keyword: '', from: '', to: '' };

export function useFilter(): UseFilterResult {
  const [filter, setFilter] = useState<FilterValues>(INITIAL);
  const [appliedFilter, setAppliedFilter] = useState<FilterValues>(INITIAL);

  const applyFilter = (): void => {
    setAppliedFilter(filter);
  };

  return { filter, appliedFilter, setFilter, applyFilter };
}
