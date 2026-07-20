"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { useLogin } from "./useAuth";
import { ApiError } from "@/lib/api/client";

function useLoginForm(): {
	userName: string;
	password: string;
	error: string;
	isPending: boolean;
	setUserName: (v: string) => void;
	setPassword: (v: string) => void;
	handleSubmit: (e: React.FormEvent<HTMLFormElement>) => void;
} {
	const router = useRouter();
	const login = useLogin();
	const [userName, setUserName] = useState("");
	const [password, setPassword] = useState("");
	const [error, setError] = useState("");

	const handleSubmit = (e: React.FormEvent<HTMLFormElement>): void => {
		e.preventDefault();
		setError("");
		login.mutate(
			{ userName, password },
			{
				onSuccess: () => {
					router.push("/dashboard/gourmet/location");
				},
				onError: (err) => {
					if (err instanceof ApiError && err.status === 401) {
						setError("ユーザー名またはパスワードが正しくありません");
					} else {
						setError("ログインに失敗しました。しばらく後に再試行してください");
					}
				},
			},
		);
	};

	return {
		userName,
		password,
		error,
		isPending: login.isPending,
		setUserName,
		setPassword,
		handleSubmit,
	};
}

export function LoginForm(): React.ReactElement {
	const { userName, password, error, isPending, setUserName, setPassword, handleSubmit } =
		useLoginForm();

	return (
		<form onSubmit={handleSubmit} className="flex flex-col gap-4">
			<h1 className="text-center text-xl font-semibold text-slate-800">LineWebHook 管理画面</h1>
			{error && <p className="rounded bg-red-50 p-3 text-sm text-red-600">{error}</p>}
			<Input
				label="ユーザー名"
				type="text"
				value={userName}
				onChange={(e) => {
					setUserName(e.target.value);
				}}
				autoComplete="username"
				required
			/>
			<Input
				label="パスワード"
				type="password"
				value={password}
				onChange={(e) => {
					setPassword(e.target.value);
				}}
				autoComplete="current-password"
				required
			/>
			<Button type="submit" loading={isPending} className="w-full">
				ログイン
			</Button>
		</form>
	);
}
