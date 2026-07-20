export function LoadingSpinner(): React.ReactElement {
	return (
		<div className="flex items-center justify-center py-12">
			<span className="h-8 w-8 animate-spin rounded-full border-4 border-slate-300 border-t-blue-600" />
		</div>
	);
}
