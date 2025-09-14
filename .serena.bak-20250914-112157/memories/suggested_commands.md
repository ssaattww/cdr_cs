# 推奨コマンド
- 計画表示/更新: Serena の `update_plan`
- 検索: `rg`, Serena の `find_file` / `search_for_pattern` / `find_symbol`
- 差分適用: Codex `apply_patch`
- C# ビルド/テスト（必要時）: `dotnet build`, `dotnet test`（.sln/プロジェクト作成後）
- 相互検証（必要時）: `pip install mcap` → `mcap info out.mcap` / Python スニペットで読取テスト
- 変更確認: `git status` / `git diff`（CI 連携は後続）