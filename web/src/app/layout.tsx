import type { Metadata } from "next";
import "./globals.css";
import { Providers } from "./providers";

export const metadata: Metadata = {
	title: "LineWebHook 管理画面",
};

type Props = {
	children: React.ReactNode;
};

export default function RootLayout({ children }: Props): React.ReactElement {
	return (
		<html lang="ja">
			<body className="bg-slate-50 text-slate-900 antialiased">
				<Providers>{children}</Providers>
			</body>
		</html>
	);
}
