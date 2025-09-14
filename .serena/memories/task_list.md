# タスクリスト（Serenaメモ）

- name: <task_name>   # ブランチ名と同一
  branch: <task_name>
  goal: <タスクの目的・達成条件（1–2文）>
  status: pending
  notes: <補足（任意）>

# 追加ルール
- タスク名とブランチ名は必ず一致させる。
- 目的は一読で内容が分かるよう日本語で簡潔に書く。
- 進行状況に応じて `status` を更新する（pending/in_progress/completed）。
