# アプローチA（純DDSインタープ）タスクリスト — 2025-09-14

以下は Specification.md（v2025-09-14）に基づく、アプローチA完遂に必要なタスク分解。

## Epic
- A: 純DDSインタープ（Connext .NET APIを主経路、Connectorは代替）

## Milestones
- A1: 最小往復（std_msgs/String）
- A2: 追加型（geometry_msgs/Twist）
- A3: 大容量型（Image/PointCloud2）と基本性能
- A4: QoSプロファイルと設定仕組み
- A5: RTI Connector 代替実装（任意）
- A6: ドキュメント/CI/配布

## Issues（概要）
1. [feat] interop-core: Connext .NET 依存導入とライブラリ雛形
2. [feat] domain-config: DomainParticipant 管理と DomainID 設定
3. [feat] name-mapping: ROS2→DDS トピック名マッピング実装＋テスト
4. [feat] qos-mapper: QoS マッピング（Reliable/Volatile/KeepLast 他）＋テスト
5. [feat] type-registry-idl: std_msgs/String の IDL 生成と型登録
6. [feat] publisher-string: rt/chatter Publisher 実装（String）
7. [feat] subscriber-string: rt/chatter Subscriber 実装（String）
8. [test] smoke-chatter: ROS 2 CLI を用いた往復スモーク
9. [docs] setup: Connext/ライセンス/環境セットアップと使用手順
10. [chore] ci: .NET ビルドCIと統合テスト枠
11. [feat] types-twist: geometry_msgs/Twist 対応（IDL生成/送受信）
12. [feat] types-image-pc2: Image/PointCloud2 対応（分割/フラグメント設定）
13. [perf] bench: スループット/レイテンシ簡易測定
14. [feat] alt-connector: RTI Connector プロバイダ（任意）
15. [docs] qos-profile: Connext QoS XML テンプレ

備考: DoD は本体メッセージを参照（受入基準/ブランチ/ラベル案をそこで詳細化）。