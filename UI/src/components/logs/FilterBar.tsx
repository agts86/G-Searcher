import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';

export type FilterValues = {
  keyword: string;
  from: string;
  to: string;
};

type Props = {
  values: FilterValues;
  onChange: (values: FilterValues) => void;
  onSearch: () => void;
  showKeyword?: boolean;
  totalCount: number;
};

export function FilterBar({ values, onChange, onSearch, showKeyword = true, totalCount }: Props): React.ReactElement {
  return (
    <div className="flex flex-wrap items-end gap-3 rounded-lg border border-slate-200 bg-white p-4">
      {showKeyword && (
        <Input
          label="キーワード"
          type="text"
          placeholder="検索..."
          value={values.keyword}
          onChange={(e) => { onChange({ ...values, keyword: e.target.value }); }}
          className="w-48"
        />
      )}
      <Input
        label="開始日"
        type="date"
        value={values.from}
        onChange={(e) => { onChange({ ...values, from: e.target.value }); }}
      />
      <Input
        label="終了日"
        type="date"
        value={values.to}
        onChange={(e) => { onChange({ ...values, to: e.target.value }); }}
      />
      <Button onClick={onSearch}>検索</Button>
      <span className="ml-auto text-sm text-slate-500">合計 {totalCount} 件</span>
    </div>
  );
}
