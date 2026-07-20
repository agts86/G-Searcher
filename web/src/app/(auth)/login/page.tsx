import { LoginForm } from "@/features/auth/LoginForm";

export default function LoginPage(): React.ReactElement {
	return (
		<div className="flex min-h-screen items-center justify-center px-4">
			<div className="w-full max-w-sm rounded-xl border border-slate-200 bg-white p-8 shadow-sm">
				<LoginForm />
			</div>
		</div>
	);
}
