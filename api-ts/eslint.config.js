// ESLint v9 新形式設定ファイル
import { resolve } from 'node:path';
import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import unusedImports from 'eslint-plugin-unused-imports';
import boundaries from 'eslint-plugin-boundaries';

const commonRules = {
  // 戻り値型必須化
  '@typescript-eslint/explicit-function-return-type': 'error',

  // any型禁止
  '@typescript-eslint/no-explicit-any': 'error',

  // 未使用変数のチェックをunused-importsプラグインに委譲
  '@typescript-eslint/no-unused-vars': 'off',
  'unused-imports/no-unused-imports': 'error',

  // 循環的複雑度・ネスト・関数長
  complexity: ['error', 10],
  'max-depth': ['error', 3],
  'max-lines-per-function': ['warn', 50],

  // 型のみのインポートでimport typeを強制
  '@typescript-eslint/consistent-type-imports': ['error', { prefer: 'type-imports' }],

  // Promiseの適切な処理を強制
  '@typescript-eslint/no-floating-promises': 'error',

  // console.logの使用を警告
  'no-console': 'warn',
};

export default tseslint.config(
  eslint.configs.recommended,
  ...tseslint.configs.recommended,
  ...tseslint.configs.recommendedTypeChecked,

  {
    // 依存方向ルール（instruction.md 4章）はsrc/配下のみ対象。
    // 「dependencies」ルールはv6以降。インストールされたv5系には無いため「element-types」を使う。
    // pnpmのpackage.json依存宣言が実行時/ビルド時の物理的な壁（強）、
    // これはlint時点で同じ違反を早期検出するための壁（中）。
    files: ['src/**/src/**/*.ts'],

    languageOptions: {
      parser: tseslint.parser,
      parserOptions: {
        project: ['./tsconfig.eslint.json'],
        tsconfigRootDir: import.meta.dirname,
        sourceType: 'module',
      },
    },

    plugins: {
      'unused-imports': unusedImports,
      boundaries,
    },

    settings: {
      'import/resolver': {
        typescript: { project: resolve(import.meta.dirname, 'tsconfig.eslint.json') },
      },
      'boundaries/root-path': resolve(import.meta.dirname),
      'boundaries/elements': [
        { type: 'host', pattern: 'src/host/src/**/*.ts', mode: 'file' },
        {
          type: 'feature',
          pattern: 'src/features/*/src/**/*.ts',
          mode: 'file',
          capture: ['featureName'],
        },
        { type: 'tables', pattern: 'src/tables/src/**/*.ts', mode: 'file' },
        { type: 'shared', pattern: 'src/shared/src/**/*.ts', mode: 'file' },
        { type: 'infrastructure', pattern: 'src/infrastructure/src/**/*.ts', mode: 'file' },
      ],
    },

    rules: {
      ...commonRules,

      'boundaries/no-unknown': 'error',
      'boundaries/no-unknown-files': 'error',
      'boundaries/element-types': [
        'error',
        {
          default: 'disallow',
          rules: [
            // features/* → 同一feature内、tables, shared のみ（infrastructure・他featureへは不可）
            {
              from: ['feature'],
              allow: ['tables', 'shared', ['feature', { featureName: '{{ featureName }}' }]],
            },
            // infrastructure → tables, shared, feature（featureのinterfaceを実装するため）
            { from: ['infrastructure'], allow: ['tables', 'shared', 'feature'] },
            // host → 自分自身(main.tsがapp.tsをimportする等), feature, infrastructure, shared（DI配線・composition root）
            { from: ['host'], allow: ['host', 'feature', 'infrastructure', 'shared'] },
            // tables, shared は最内層。他レイヤーに依存しない
          ],
        },
      ],
    },
  },

  {
    // test/ 配下は品質ルールのみ適用し、境界ルール（boundaries）は課さない
    files: ['**/test/**/*.ts'],

    languageOptions: {
      parser: tseslint.parser,
      parserOptions: {
        project: ['./tsconfig.eslint.json'],
        tsconfigRootDir: import.meta.dirname,
        sourceType: 'module',
      },
    },

    plugins: {
      'unused-imports': unusedImports,
    },

    rules: {
      ...commonRules,
      // テスト用Fake実装はインターフェース適合のためasyncだが内部にawaitを持たないことがある
      '@typescript-eslint/require-await': 'off',
    },
  },

  {
    ignores: ['**/dist/', '**/node_modules/', '**/*.js', '**/*.d.ts'],
  },
);
