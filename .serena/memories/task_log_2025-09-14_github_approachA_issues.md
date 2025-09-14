# 2025-09-14 — Approach A のIssue階層・ラベル反映ログ

## 実施概要
- ルート作成: #22 "[meta] Approach A — DDS Interop ロードマップ"（A1〜A5 をチェックリストで管理）
- A1〜A5 のトラッカー作成とリンク:
  - #21 A1: Minimal Interop (String) — サブIssue進行トラッカー（Blocked by #22）
  - #23 A2: Add Types (Twist) — サブIssue進行トラッカー（Blocked by #21）
  - #26 A3: Large Payload + Baseline Perf — サブIssue進行トラッカー（Blocked by #23）
  - #29 A4: QoS Profiles + App Config — サブIssue進行トラッカー（Blocked by #26）
  - #32 A5: Alt Path (RTI Connector) — サブIssue進行トラッカー（Blocked by #21）
- サブIssue（抜粋・全件 area:approach-a 付与）
  - A1: #12 #13 #14 #15 #16 #17 #18 #19 #20（Blocked by を本文先頭に明記）
  - A2: #24 #25（#23配下）
  - A3: #27 #28（#26配下）
  - A4: #30 #31（#29配下）
  - A5: #33（#32配下）
- タイトル整備: A1 系サブIssueへ "A1:" プレフィックスを付与済み
- ラベル新設/付与:
  - type:feat → #12 #13 #14 #15 #16 #17 #18 #24 #27 #30 #31 #33
  - type:test → #19 #25
  - type:docs → #20
  - type:perf → #28
  - type:chore → #21 #22 #23 #26 #29 #32
  - 既存の area:approach-a は全件維持

## ブランチ/PR・同期
- ブランチ: `docs/subissue-policy-20250914`（AGENTS.md 追記）→ PR #11 を作成・マージ済み
- ブランチ: `docs/approach-a-tasks-20250914`（ApproachAタスクメモ退避）を作成・push（PRなし）
- ローカル同期: PRブランチをリモート状態へ `git fetch` + `git reset --hard` で一致

## 運用メモ
- 依存表現: Linked issuesのblocksは使用せず、各Issue本文先頭に "Blocked by #<番号>" を明記。
- 開始順: 親Issueのチェックリストの並びをもって開始順とする（A1→A2→A3→A4→A5）。
- 次アクション: priority/status ラベルの運用（例: P0/P1 と ready/in-progress/blocked）を定義・一括適用。