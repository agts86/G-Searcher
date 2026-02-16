'use client';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { AuthGuard } from '@/lib/auth/AuthGuard';
import { useLogout, useMe } from '@/features/auth/useAuth';
import { useAutoRefresh } from '@/features/auth/useAutoRefresh';

const NAV_ITEMS = [
  { label: 'ロケーション', href: '/dashboard/gourmet/location' },
  { label: 'ワード', href: '/dashboard/gourmet/word' },
  { label: 'エラーログ', href: '/dashboard/error-log' },
  { label: 'ジョブログ', href: '/dashboard/job-log' },
] as const;

function Header(): React.ReactElement {
  const router = useRouter();
  const { data } = useMe();
  const logout = useLogout();
  const me = data;

  const handleLogout = (): void => {
    logout.mutate(undefined, {
      onSuccess: () => { router.push('/login'); },
    });
  };

  return (
    <header className="flex h-14 items-center justify-between border-b border-slate-200 bg-slate-800 px-6">
      <span className="text-sm font-semibold text-white">LineWebHook 管理画面</span>
      <div className="flex items-center gap-4">
        {me && <span className="text-xs text-slate-300">{me.userName}</span>}
        <button
          onClick={handleLogout}
          className="rounded px-3 py-1 text-xs text-slate-300 hover:bg-slate-700 hover:text-white"
        >
          ログアウト
        </button>
      </div>
    </header>
  );
}

function SideNav(): React.ReactElement {
  const pathname = usePathname();

  return (
    <nav className="w-40 shrink-0 border-r border-slate-200 bg-slate-100">
      <ul className="flex flex-col py-4">
        {NAV_ITEMS.map((item) => (
          <li key={item.href}>
            <Link
              href={item.href}
              className={`block px-4 py-2.5 text-sm transition-colors hover:bg-slate-200 ${
                pathname === item.href ? 'bg-white font-medium text-blue-600' : 'text-slate-700'
              }`}
            >
              {item.label}
            </Link>
          </li>
        ))}
      </ul>
    </nav>
  );
}

type Props = {
  children: React.ReactNode;
};

export default function DashboardLayout({ children }: Props): React.ReactElement {
  useAutoRefresh();

  return (
    <AuthGuard>
      <div className="flex h-screen flex-col">
        <Header />
        <div className="flex flex-1 overflow-hidden">
          <SideNav />
          <main className="flex-1 overflow-y-auto p-6">{children}</main>
        </div>
      </div>
    </AuthGuard>
  );
}
