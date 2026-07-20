import { Button } from "./Button";

type Props = {
	message?: string;
	onRetry?: () => void;
};

export function ErrorMessage({
	message = "エラーが発生しました",
	onRetry,
}: Props): React.ReactElement {
	return (
		<div className="flex flex-col items-center gap-4 py-12">
			<p className="text-sm text-red-600">{message}</p>
			{onRetry && (
				<Button variant="secondary" onClick={onRetry}>
					再試行
				</Button>
			)}
		</div>
	);
}
