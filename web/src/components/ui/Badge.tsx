type Variant = "success" | "error" | "neutral";

type Props = {
	variant: Variant;
	children: React.ReactNode;
};

const variantClass: Record<Variant, string> = {
	success: "bg-green-100 text-green-700",
	error: "bg-red-100 text-red-700",
	neutral: "bg-slate-100 text-slate-700",
};

export function Badge({ variant, children }: Props): React.ReactElement {
	return (
		<span
			className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${variantClass[variant]}`}
		>
			{children}
		</span>
	);
}
