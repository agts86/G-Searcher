import type { ButtonHTMLAttributes } from "react";

type Variant = "primary" | "secondary" | "danger";

type Props = ButtonHTMLAttributes<HTMLButtonElement> & {
	variant?: Variant;
	loading?: boolean;
};

const variantClass: Record<Variant, string> = {
	primary: "bg-blue-600 text-white hover:bg-blue-700 disabled:bg-blue-300",
	secondary: "bg-slate-200 text-slate-800 hover:bg-slate-300 disabled:bg-slate-100",
	danger: "bg-red-600 text-white hover:bg-red-700 disabled:bg-red-300",
};

export function Button({
	variant = "primary",
	loading = false,
	children,
	...props
}: Props): React.ReactElement {
	return (
		<button
			{...props}
			disabled={props.disabled ?? loading}
			className={`inline-flex items-center justify-center rounded px-4 py-2 text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed ${variantClass[variant]} ${props.className ?? ""}`}
		>
			{loading && (
				<span className="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
			)}
			{children}
		</button>
	);
}
