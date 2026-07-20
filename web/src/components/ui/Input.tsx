import type { InputHTMLAttributes } from "react";

type Props = InputHTMLAttributes<HTMLInputElement> & {
	label?: string;
	error?: string;
};

export function Input({ label, error, ...props }: Props): React.ReactElement {
	return (
		<div className="flex flex-col gap-1">
			{label && <label className="text-sm font-medium text-slate-700">{label}</label>}
			<input
				{...props}
				className={`rounded border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${error ? "border-red-500" : "border-slate-300"} ${props.className ?? ""}`}
			/>
			{error && <span className="text-xs text-red-600">{error}</span>}
		</div>
	);
}
