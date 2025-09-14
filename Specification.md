# 仕様書: C# から ROS 2 と相互通信するための 2 アプローチ（RTI Connext 利用）

最終更新: 2025-09-14（JST）

---

## 0. 目的・非目的

- **目的**  
  - C#（.NET）アプリが、ROS 2 システムと**トピックレベルで相互通信**できるようにする。  
  - 自作 P/Invoke を書かずに、**RTI Connext の公式 .NET API** または **RTI Connector for .NET** を用いる。

- **非目的（当初スコープ外）**  
  - ROS 2 ノード/グラフAPI/パラメータ/サービス/アクションの完全再現。  
  - DDS 独自機能への依存（標準DDS範囲内で構築）。

---

## 1. アプローチ概要

### A. 純DDSインタープ（ROS層をバイパス）
- C# から **Connext .NET API (`Rti.ConnextDds`)** もしくは **RTI Connector for .NET** を使用。  
- ROS 2 側（RMW が Fast DDS / Cyclone DDS / Connext のいずれでも）と、**DDS の相互運用**によりデータ平面で通信。  
- 成功条件は **(1) 同一の型定義 (IDL)**、**(2) トピック名のマッピング準拠**、**(3) QoS 整合**。

### B. “C#版クライアントライブラリ”を Connext 上に実装
- Connext の Publisher/Subscriber/Request-Reply を足場に、**C# 側で ROS 2 の語彙（ノード/パラメータ/サービス/アクション等）を再現**。  
- A を実証・安定化後、段階的に B を整備（長期ロードマップ）。

---

## 2. 参照標準と設計ドキュメント

- Connext .NET API: <https://community.rti.com/static/documentation/connext-dds/current/doc/api/connext_dds/api_csharp/index.html>  
- RTI Connector for .NET: <https://rticommunity.github.io/rticonnextdds-connector-cs/>（API概要: <https://rticommunity.github.io/rticonnextdds-connector-cs/articles/api_overview.html>）  
- ROS 2: Topic/Service 名の DDS マッピング: <https://design.ros2.org/articles/topic_and_service_names.html>  
- RMW Connext 実装（参考）: <https://github.com/ros2/rmw_connextdds> / <https://index.ros.org/p/rmw_connextdds/>  
- DDS を ROS 2 下で使う背景: <https://design.ros2.org/articles/ros_on_dds.html>

---

## 3. 共通要件（A/B に共通）

### 3.1 型定義（IDL）とコード生成
- ROS 2 の `.msg/.idl` と**同一レイアウトの DDS IDL**を用意すること。  
- 生成方法（いずれか）  
  - `rtiddsgen` による **IDL → C# 型生成**  
  - **Dynamic Data API** を用い、XML/IDL でランタイム登録  
- **互換性原則**  
  - フィールド順・型幅・可変長/配列指定を厳密に一致。  
  - `builtin_interfaces` 等の標準型は既存 IDL を再利用（改変禁止）。

### 3.2 トピック名マッピング（ROS 2 → DDS）
- ROS 2 トピック `/foo/bar` は、DDS では一般に **`rt/foo/bar`** にマップ（設計文書に従う）。  
- 名前空間・サブネームスペースは `/` を保持。  
- 例: ROS 2 `/robot/cmd_vel` → DDS `rt/robot/cmd_vel`。  
- サービス/アクションを扱う場合は、Request/Reply トピック命名規約に従う。

### 3.3 QoS 整合（推奨初期値）
| 項目 | ROS 2 既定の目安 | Connext 側指定例 |
|---|---|---|
| Reliability | Reliable | `ReliabilityKind.Reliable` |
| Durability | Volatile | `DurabilityKind.Volatile` |
| History | KeepLast(depth=10) | `HistoryKind.KeepLast`, depth=10 |
| Deadline | 未指定 | `Duration.Infinite` |
| Lifespan | 未指定 | 必要に応じ設定 |
| Liveliness | Automatic | `LivelinessKind.Automatic` |

> 注意: 実際の既定は RMW/型/ツール（`ros2 topic pub/echo` 等）で差異があるため、通信相手の QoS を確認して合わせること。

### 3.4 ドメイン/ディスカバリ
- **Domain ID** は通信相手と一致させる（既定: `0` が多い）。  
- マルチキャスト/ユニキャスト・Participant QoS はネットワーク環境に応じ調整。

### 3.5 セキュリティ（任意）
- DDS Security（Access Control/Authentication/Encryption）を使用可能。  
- ROS 2 側が SROS2 等でセキュア化されている場合、DDS Security 文書に従い証明書類を合わせる。

---

## 4. アプローチ A の仕様（純DDSインタープ）

### 4.1 依存関係
- NuGet: `Rti.ConnextDds`（.NET API）  
  - NuGet パッケージ: <https://www.nuget.org/packages/Rti.ConnextDds>  
- または RTI Connector for .NET（DLL + XML 設定）

### 4.2 構成要素
- `DomainParticipant`、`Topic<T>`、`DataWriter<T>`、`DataReader<T>` で構成。  
- `T` は IDL から生成した C# 型、または Dynamic Data を使用。

### 4.3 実行フロー（Publisher/Subscriber）
1. **Participant 作成**（Domain ID を ROS 2 と一致）  
2. **型登録**（IDL 生成型 or Dynamic Data）  
3. **Topic 作成**（名前は `rt/...` マッピングを適用）  
4. **QoS 設定**（Reliability/Durability/History 等を相手に合わせる）  
5. **Writer/Reader 作成**  
6. **送受信**（`writer.Write(data)` / `reader.OnDataAvailable` または Poll）

### 4.4 動作確認
- ROS 2 側で `ros2 topic echo /chatter`，C# 側から `rt/chatter` に Publish。  
- 逆方向は `ros2 topic pub` を使用し C# 側で受信確認。  
- 大規模データ（`sensor_msgs/PointCloud2` など）も、適切な QoS/Fragment 設定で実運用可。

### 4.5 制約
- ノード/パラメータ/グラフ情報は**自動では見えない**。  
- ツール（`ros2 node list` 等）への露出は限定的（データ平面のみ）。

### 4.6 A1: Minimal Interop (String) — ワイヤ形式
- エンコード: UTF-8（BOMなし）
- 長さヘッダ: 32ビット符号なし整数（4バイト）。値は「UTF-8バイト数 + 終端NUL(1バイト)」。
- 配置: `[len:4B][payload:len-1B][0x00]`。ヘッダは4B境界に整列。
- エンディアン: CDRカプセル化の指定に追従（A1の最小実装では Little Endian を既定）。
- 検証/エラー: デコード時に以下を検証し、不一致は例外とする。
  - バッファ長が `4 + len` 以上であること
  - `payload` 末尾に `0x00`（NUL）が存在すること
  - `payload`（`len-1`バイト）が厳格UTF-8として復元可能であること

例（Little Endian）
- "hello": 文字列長=5, バイト列=`68 65 6c 6c 6f` → len=6 →
  `06 00 00 00  68 65 6c 6c 6f 00`
- "ほげ": UTF-8=`E3 81 BB E3 81 92`（6バイト）→ len=7 →
  `07 00 00 00  E3 81 BB E3 81 92 00`

備考
- 本仕様は ROS 2 の CDR 文字列表現（長さに終端NULを含む）に整合する。将来的にエンディアンはカプセル化フラグで切替予定。

---

## 5. アプローチ B の仕様（Connext 上で C# クライアント語彙を再現）

### 5.1 目的
- C# だけで ROS 2 に近い開発体験を提供（ノード、パラメータ、サービス/アクション、タイマー等）。

### 5.2 設計ガイド
- **Node 相当**: Participant + 名前空間 + ライフサイクル管理  
- **Publisher/Subscriber**: A と同様（Topic/QoS 整合）  
- **Service/Action**: **DDS Request-Reply** パターンを採用し、ROS 2 の命名規約に合わせる  
- **グラフ情報**: ROS 2 内部のメタトピックを購読し、C# 側でビューを構築（段階導入）

### 5.3 互換性・参照
- RMW Connext 実装（`rmw_connextdds`）の命名/QoS/型の扱いを参照。  
- 公式リリースに合わせ、ディストリごとの差分（型記述/ハッシュなど）が出た場合は追従。

### 5.4 品質/配布
- **品質レベル**: REP-2004 に準拠した Quality Declaration を整備。  
- **CI**: Linux/Windows の .NET（LTS）＋複数 DDS 実装との相互運用試験。  
- **サンプル**: `std_msgs/String`, `geometry_msgs/Twist`, `sensor_msgs/PointCloud2` から開始。

---

## 6. ビルド・デプロイ・運用

- **ビルド**:  
  - Connext .NET API → 通常の `dotnet build`。  
  - RTI Connector → XML 設計ファイルを同梱し、ランタイムにロード。  
- **運用**:  
  - Domain/Discovery 設定を運用環境でプロファイル化（XML QoS プロファイル）。  
  - コンテナ/VM 配布では、ライセンスとネイティブ依存（ランタイム）配置に留意。

---

## 7. テスト方針

- **単体**: 型のシリアル化/デシリアル化、可変長/配列境界、NaN/Inf。  
- **相互運用**:  
  - RMW = Fast DDS / Cyclone DDS / Connext の ROS 2 と往復通信。  
  - 複数 QoS 組合せ（信頼性/歴史/デッドライン等）。  
- **負荷**: 高頻度トピック、スループット、レイテンシ、ロス率。  
- **長期**: Discovery 再参加、ネットワーク切断/復旧、Participant 再生成。

---

## 8. 既知のリスクと回避策

- **名前/型/QoS の不一致** → 通信不可  
  - → 片側の QoS/トピック名/IDL を検証し整合させるチェックリストを運用。  
- **DDS 実装差異**（フラグメント化/リライアビリティ挙動）  
  - → 実装横断の試験ベンチを常設。  
- **セキュリティ/証明書配布**  
  - → SROS2/DDS Security の手順を文書化し、証明書ローテーションを手順化。

---

## 9. ロードマップ

1. **A の最小実装**（`std_msgs/String`）で往復通信  
2. `geometry_msgs/Twist`、`sensor_msgs/Image/PointCloud2` を追加  
3. QoS プロファイルのテンプレ整備  
4. **B の着手**: Request-Reply による Service、Action（Goal/Result/Feedback）の順で対応  
5. グラフ情報/ノード/パラメータの再現  
6. Quality Declaration/ドキュメント整備、サンプル群公開

---

## 10. 参考リンク（抜粋）

- Connext .NET API: <https://community.rti.com/static/documentation/connext-dds/current/doc/api/connext_dds/api_csharp/index.html>  
- RTI Connector for .NET: <https://rticommunity.github.io/rticonnextdds-connector-cs/>  
- ROS 2 名称マッピング: <https://design.ros2.org/articles/topic_and_service_names.html>  
- RMW Connext: <https://github.com/ros2/rmw_connextdds> / <https://index.ros.org/p/rmw_connextdds/>  
- DDS 採用背景（ROS on DDS）: <https://design.ros2.org/articles/ros_on_dds.html>  
- RTI Blog（ROS 2 × DDS 事例）: <https://www.rti.com/blog/ros-2-and-dds-interoperability-drives-next-generation-robotics>
