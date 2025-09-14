# Serena MCP メモリ運用ポリシー（2025-09-14）

- 原則: メモリの閲覧・作成・更新・削除は Serena MCP のみを使用。
  - 読み取り: `serena__list_memories` / `serena__read_memory`
  - 追加/更新: `serena__write_memory`
  - 削除: `serena__delete_memory`（ユーザー明示指示がある場合のみ）
- 禁止/回避: `.serena/memories/` 配下を `apply_patch`/shell で直接編集しない。`github__push_files` は原則使用しない（例外はユーザー明示指示）。
- リポジトリ反映: 共有が必要な場合のみ Git で同期。
  - 手順: `git__git_add` → `git__git_commit`（branch: `docs/serena-memo-YYYYMMDD`）→ PR 作成 → マージ後にブランチ削除
  - 直push禁止リポジトリでは必ず PR 経由。push は承認を得て実行。
- 命名規則: `task_log_YYYY-MM-DD_topic` / `work_summary_YYYY-MM-DD` など既存規則に準拠。
- ログ徹底: 重要操作後は `work_summary_YYYY-MM-DD` に要約を追加（Serena MCP で）。